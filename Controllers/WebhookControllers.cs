using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/[controller]")]
public class WebhookController : ControllerBase
{
    private readonly IUserRepository _users;

    public WebhookController(IUserRepository users)
    {
        _users = users;
    }

    [HttpPost("flutterwave")]
    public IActionResult HandleEvent([FromBody] PaymentEvent evt)
    {
        if (evt.Status == "successful")
        {
            var user = _users.GetById(evt.CustomerId);
            if (user != null)
            {
                user.IsPremium = true;
                _users.Update(user);
            }
        }

        return Ok();
    }
}
