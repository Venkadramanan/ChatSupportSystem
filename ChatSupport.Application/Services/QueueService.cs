using ChatSupport.Application.Interfaces;
using ChatSupport.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ChatSupport.Application.Services
{
    public class QueueService : IQueueService
    {
        private readonly Queue<ChatSession> _chatQueue = new();
        private readonly Dictionary<Guid, ChatSession> _activeSessions = new();
        private readonly Dictionary<Guid, DateTime> _pollTimestamps = new();

        private readonly IAssignmentService _assignmentService;
        private readonly List<Team> _primaryTeams;
        private readonly Team _overflowTeam;
        private readonly bool _isOfficeHours;

        public QueueService(List<Team> primaryTeams, Team overflowTeam, bool isOfficeHours, IAssignmentService assignmentService)
        {
            _primaryTeams = primaryTeams;
            _overflowTeam = overflowTeam;
            _isOfficeHours = isOfficeHours;
            _assignmentService = assignmentService;
        }

        public bool TryEnqueue(ChatSession session)
        {
            var assignedAgent = _assignmentService.AssignAgent(session);

            if (assignedAgent != null)
            {
                _activeSessions[session.Id] = session;
                Console.WriteLine($"✅ Chat assigned to {assignedAgent.Name} (Role: {assignedAgent.Role}) | Session ID: {session.Id} | Customer: {session.CustomerName}");
                return true;
            }

            session.IsActive = true;
            _chatQueue.Enqueue(session);
            Console.WriteLine($"📥 Chat queued for role: {session.RequestedRole} | Session ID: {session.Id} | Customer: {session.CustomerName}");
            return false;
        }


        public ChatSession? Dequeue()
        {
            if (_chatQueue.Count == 0)
            {
                Console.WriteLine($"📭 Dequeue attempted but queue is empty at {DateTime.UtcNow:HH:mm:ss}");
                return null;
            }

            var session = _chatQueue.Dequeue();
            session.IsActive = false;
            session.LastPolledAt = DateTime.UtcNow;

            Console.WriteLine($"📤 Chat dequeued | Session ID: {session.Id} | Customer: {session.CustomerName} | Requested Role: {session.RequestedRole} | Time: {DateTime.UtcNow:HH:mm:ss}");

            return session;
        }


        public int QueueLength => _chatQueue.Count;

        public IReadOnlyCollection<ChatSession> GetAllSessions() => _chatQueue.ToArray();

        public bool UpdatePolling(Guid sessionId)
        {
            if (_activeSessions.TryGetValue(sessionId, out var session))
            {
                session.LastPolledAt = DateTime.UtcNow;
                _pollTimestamps[sessionId] = session.LastPolledAt;
                Console.WriteLine($"🔄 Poll received for Session ID: {sessionId} | LastPolledAt updated to {session.LastPolledAt}");
                return true;
            }

            Console.WriteLine($"⚠️ Poll failed: Session ID {sessionId} not found or inactive");
            return false;
        }


        public IReadOnlyDictionary<Guid, ChatSession> ActiveSessions => _activeSessions;

        public Dictionary<string, int> GetQueueStatus()
        {
            return _chatQueue
                .GroupBy(s => s.RequestedRole ?? "Unknown") // Use null-coalescing if RequestedRole might be null
                .ToDictionary(g => g.Key, g => g.Count());
        }
    }
}
