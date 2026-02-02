using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

public class User
{
    public required int Id { get; set; }
    public string Username { get; set; } = string.Empty; 
    public string PasswordHash { get; set; } = string.Empty;
    public bool IsPremium { get; set; }
}


[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "Admin,PremiumUser")]
public class PaymentController : ControllerBase
{
    private readonly IUserRepository _users;

    public PaymentController(IUserRepository users)
    {
        _users = users;
    }

    [HttpPost("activate-premium/{userId}")]
    public IActionResult ActivatePremium(int userId)
    {
        var user = _users.GetById(userId);
        if (user == null) return NotFound();

        user.IsPremium = true;
        _users.Update(user);

        return Ok(new { message = "User upgraded to premium", user });
    }

    [HttpGet("user/{userId}")]
    public IActionResult GetUserDetails(int userId)
    {
        var user = _users.GetById(userId);
        if (user == null) return NotFound();

        return Ok(user);
    }
}

