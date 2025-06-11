using ChatSupport.API.Models;
using ChatSupport.Application.Interfaces;
using ChatSupport.Application.Services;
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
        private readonly IAssignmentService _assignmentService;

        public ChatController(IQueueService queueService, IAssignmentService assignmentService)
        {
            _queueService = queueService;
            _assignmentService = assignmentService;
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

        [HttpGet("agent-load")]
        public IActionResult GetAgentLoad()
        {
            var agents = _assignmentService.GetAllAgents();

            var result = agents.Select(a => new AgentLoadDto
            {
                Name = a.Name,
                Role = a.Role,
                CurrentChatCount = a.CurrentChatCount,
                MaxConcurrency = a.MaxConcurrency,
                CanAcceptMoreChats = a.CanAcceptMoreChats
            });

            return Ok(result);
        }

        [HttpGet("active-sessions")]
        public IActionResult GetActiveSessions()
        {
            var sessions = _queueService.GetActiveSessions();
            return Ok(sessions.Select(s => new {
                s.Id,
                s.CustomerName,
                s.AssignedAgentName,
                s.LastPolledAt
            }));
        }

        [HttpDelete("remove-session/{sessionId:guid}")]
        public IActionResult RemoveSession(Guid sessionId)
        {
            var removed = _assignmentService.RemoveSessionAndReleaseAgent(sessionId);
            return removed ? Ok("Session removed.") : NotFound("Session not found.");
        }

    }
}
