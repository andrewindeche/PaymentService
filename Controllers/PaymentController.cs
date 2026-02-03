using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using SubPayment.Data;
using SubPayment.Models;


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
    public async Task<IActionResult> ActivatePremium(int userId)
    {
        var user = await _users.GetByIdAsync(userId);
        if (user == null) return NotFound();

        user.IsPremium = true;
        await _users.UpdateAsync(user);

        return Ok(new { message = "User upgraded to premium", user });
    }

    [HttpGet("user/{userId}")]
    public async Task<IActionResult> GetUserDetails(int userId)
    {
        var user = await _users.GetByIdAsync(userId);
        if (user == null) return NotFound();

        return Ok(user);
    }
}

