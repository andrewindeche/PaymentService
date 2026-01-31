using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

public class User
{
    public required string Id { get; set; }
    public bool IsPremium { get; set; }
}

public interface IUserRepository
{
    User GetById(string userId);
    void Update(User user);
}

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class PaymentController : ControllerBase
{
    private readonly IUserRepository _users;

    public PaymentController(IUserRepository users)
    {
        _users = users;
    }

    [HttpPost("activate-premium/{userId}")]
    public IActionResult ActivatePremium(string userId)
    {
        var user = _users.GetById(userId);
        if (user == null) return NotFound();

        user.IsPremium = true;
        _users.Update(user);

        return Ok(new { message = "User upgraded to premium", user });
    }

    [HttpGet("user/{userId}")]
    public IActionResult GetUserDetails(string userId)
    {
        var user = _users.GetById(userId);
        if (user == null) return NotFound();

        return Ok(user);
    }
}

