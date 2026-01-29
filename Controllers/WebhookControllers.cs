using Microsoft.AspNetCore.Mvc;

public interface IPaymentEvent
{
    string Status { get; set; }
    int CustomerId { get; set; }
}

public class PaymentEvent : IPaymentEvent
{
    public required string Status { get; set; }
    public int CustomerId { get; set; }
}

[ApiController]
[Route("api/[controller]")]
public class WebhookController : ControllerBase
{
    private readonly IUserRepository users;

    public WebhookController(IUserRepository users)
    {
        this.users = users;
    }
    
    [HttpPost("flutterwave")]
    public IActionResult HandleEvent([FromBody] PaymentEvent evt)
    {
        if (evt.Status == "successful")
        {
            var user = users.GetById(evt.CustomerId.ToString());
            if (user != null)
            {
                user.IsPremium = true;
                users.Update(user);
            }
        }

        return Ok();
    }
}
