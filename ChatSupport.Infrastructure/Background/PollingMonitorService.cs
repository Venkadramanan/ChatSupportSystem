using ChatSupport.Application.Interfaces;
using ChatSupport.Domain.Models;
using Microsoft.Extensions.Hosting;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace ChatSupport.Infrastructure.Background
{
    public class PollingMonitorService : BackgroundService
    {
        private readonly IQueueService _queueService;
        private readonly IAssignmentService _assignmentService;

        public PollingMonitorService(IQueueService queueService, IAssignmentService assignmentService)
        {
            _queueService = queueService;
            _assignmentService = assignmentService;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var lastCleanup = DateTime.UtcNow;

            while (!stoppingToken.IsCancellationRequested)
            {
                // 🔄 Attempt to assign chat sessions from queue
                var session = _queueService.Dequeue();
                if (session != null)
                {
                    bool isOfficeHours = IsWithinOfficeHours();
                    Console.WriteLine($"Time Check | Office Hours: {isOfficeHours}");

                    Agent? agent = _assignmentService.AssignAgent(session, isOfficeHours);

                    if (agent != null)
                    {
                        session.AssignedAgentName = agent.Name;
                        _queueService.RegisterActiveSession(session);
                    }
                    else
                    {
                        Console.WriteLine($"No available agents for session {session.Id}. Re-enqueueing...");
                        _queueService.TryEnqueue(session);
                        await Task.Delay(2000, stoppingToken);
                    }
                }
                else
                {
                    await Task.Delay(1000, stoppingToken);
                }

                // 🧹 Inactive session cleanup every 10 seconds
                if ((DateTime.UtcNow - lastCleanup).TotalSeconds >= 10)
                {
                    CleanupInactiveSessions();
                    lastCleanup = DateTime.UtcNow;
                }
            }
        }

        private bool IsWithinOfficeHours()
        {
            var now = DateTime.UtcNow.AddHours(5.5); // IST (or adjust as needed)
            return now.Hour >= 9 && now.Hour < 18;
        }

        private void CleanupInactiveSessions()
        {
            var now = DateTime.UtcNow;
            var timeout = TimeSpan.FromSeconds(30);

            var inactive = _queueService.ActiveSessions
                .Where(kvp => now - kvp.Value.LastPolledAt > timeout)
                .Select(kvp => kvp.Key)
                .ToList();

            foreach (var sessionId in inactive)
            {
                Console.WriteLine($"Removing inactive session {sessionId} due to polling timeout.");
                _assignmentService.RemoveSessionAndReleaseAgent(sessionId);
            }
        }
    }
}