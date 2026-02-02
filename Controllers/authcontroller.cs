using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using SubPayment.Models;
using SubPayment.Data;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.IdentityModel.Tokens;
using System.Text;


[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IConfiguration _config;
    private readonly IUserRepository _users;
    public AuthController(IConfiguration config, IUserRepository users)
    {
        _config = config; 
        _users = users;
    }

[HttpPost("login")]
public async Task<IActionResult> Login([FromBody] LoginModel login)
{
    if (login.Username == "admin" && login.Password == "password")
    {
        var user = await _users.GetByUsernameAsync(login.Username);
        if (user == null)
        {
            return Unauthorized("User not found");
        }

        if (user.PasswordHash != login.Password)
        {
            return Unauthorized("Invalid password");
        }

        var claims = new[]
        {
            new Claim(ClaimTypes.Name, login.Username),
            new Claim(
                ClaimTypes.Role,
                user.Username == "admin"
                    ? "Admin"
                    : (user.IsPremium ? "PremiumUser" : "User")
            )
        };

        var key = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(_config["Jwt:Key"]!)
        );
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: _config["Jwt:Issuer"],
            audience: _config["Jwt:Audience"],
            claims: claims,
            expires: DateTime.UtcNow.AddHours(1),
            signingCredentials: creds
        );

        var tokenString = new JwtSecurityTokenHandler().WriteToken(token);

        return Ok(new { message = "Login successful", token = tokenString });
    }

    return Unauthorized("Invalid credentials");
}


[ApiController]
[Route("api/[controller]")]
public class ValuesController : ControllerBase
{
    [HttpGet]
    [Authorize]
    public IActionResult GetValues()
    {
        return Ok(new[] { "Value1", "Value2" });
    }
}
}
