using Microsoft.AspNetCore.Mvc;
using moabix.QueueManager;

namespace moabix.Controllers
{
    public class QueueController : Controller
    {
        public readonly IRabbitManager _rabbitManager;
        public QueueController(IRabbitManager rabbitManager) {
            _rabbitManager = rabbitManager;
        }

        [HttpPost]
        public IActionResult SendMessageToQueue([FromBody] string message)
        {
            try
            {
                _rabbitManager.PublishMessage(message);
                return Ok("Message sent to the queue.");
            }
            catch (Exception ex)
            {
                // Log the exception or handle it appropriately
                return StatusCode(500, "An error occurred while sending the message to the queue.");
            }
        }
    }
}
