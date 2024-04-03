using Microsoft.AspNetCore.Mvc;
using moabix.Models;
using moabix.Services.QueueManager;

namespace moabix.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class PaymentController : Controller
    {
        public readonly IPaymentsManager _paymentsManager;
        public PaymentController(IPaymentsManager rabbitManager) {
            _paymentsManager = rabbitManager;
        }

        [HttpPost]
        public IActionResult SendMessageToQueue([FromBody] Payment payment)
        {
            try
            {
                _paymentsManager.PublishPayment(payment);
                return Ok("Message sent to the queue.");
            }
            catch (Exception ex)
            {
                // Log the exception or handle it appropriately
                return StatusCode(500, "An error occurred while sending the message to the queue.");
            }
        }

        [HttpDelete]
        public IActionResult CleanPaymentQueue()
        {
            try
            {
                _paymentsManager.CleanPaymentQueue();
                return Ok("Queue deleted");
            }
            catch (Exception ex)
            {
                // Log the exception or handle it appropriately
                return StatusCode(500, "An error occurred while sending the message to the queue.");
            }
        }
    }
}
