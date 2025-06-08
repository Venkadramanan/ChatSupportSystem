using ChatSupport.API.Models;
using ChatSupport.Application.Interfaces;
using ChatSupport.Domain.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ChatSupport.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ChatController : ControllerBase
    {
        private readonly IQueueService _queueService;

        public ChatController(IQueueService queueService)
        {
            _queueService = queueService;
        }

        [HttpPost("start-chat")]
        public IActionResult StartChat([FromBody] StartChatRequest request)
        {
            var session = new ChatSession
            {
                Id = Guid.NewGuid(),
                CustomerName = request.CustomerName,
                CreatedAt = DateTime.UtcNow,
                LastPolledAt = DateTime.UtcNow,
                IsActive = true
            };

            var success = _queueService.TryEnqueue(session);

            return Ok(new
            {
                sessionId = session.Id,
                isQueued = success
            });
        }


        [HttpPost("poll/{sessionId:guid}")]
        public IActionResult Poll(Guid sessionId)
        {
            var updated = _queueService.UpdatePolling(sessionId);
            if (!updated)
                return NotFound("Session not found or inactive.");

            return Ok("Polling timestamp updated.");
        }

        [HttpGet("queue-status")]
        public IActionResult QueueStatus()
        {
            return Ok(_queueService.GetQueueStatus());
        }

        [HttpGet("dequeue")]
        public IActionResult DeQueue()
        {
            return Ok(_queueService.Dequeue());
        }
    }
}
