using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace YourApp.Controllers
{
    public interface IUserRepository
    {
        Task<PaymentUser> GetByIdAsync(int id);
        Task UpdateAsync(PaymentUser user);
    }

    public class PaymentCustomer
    {
        public required string Email { get; set; }
        public required string PhoneNumber { get; set; }
        public int Id { get; set; }
    }

    public class PaymentEvent
    {
        public required string Status { get; set; }
        public required string TxRef { get; set; }
        public int Id { get; set; }
        public required PaymentCustomer Customer { get; set; }
    }

    public class PaymentUser
    {
        public int Id { get; set; }
        public required string PhoneNumber { get; set; }
        public bool IsPremium { get; set; }
    }

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
        public async Task<IActionResult> HandlePayment(
            [FromBody] PaymentEvent evt,
            [FromServices] AfricasTalkingService sms)
        {
            var user = await _users.GetByIdAsync(evt.Customer.Id);
            if (user == null)
                return NotFound();

            if (evt.Status.Equals("successful", StringComparison.OrdinalIgnoreCase))
            {
                user.IsPremium = true;
                await _users.UpdateAsync(user);

                try
                {
                    await sms.SendSmsAsync(user.PhoneNumber,
                        "Payment successful! Premium activated.");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"SMS error: {ex.Message}");
                }
            }
            else
            {
                try
                {
                    await sms.SendSmsAsync(user.PhoneNumber,
                        "Payment failed. Please try again.");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"SMS error: {ex.Message}");
                }
            }

            return Ok();
        }
    }
}
