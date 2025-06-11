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

        public bool TryEnqueue(ChatSession session)
        {
            _chatQueue.Enqueue(session);
            session.IsActive = true;

            Console.WriteLine($"Chat enqueued | Session ID: {session.Id} | Customer: {session.CustomerName}");
            return true;
        }

        public ChatSession? Dequeue()
        {
            if (_chatQueue.Count == 0)
            {
                Console.WriteLine("Queue is empty.");
                return null;
            }

            var session = _chatQueue.Dequeue();
            Console.WriteLine($"Dequeued session for assignment | Session ID: {session.Id} | Customer: {session.CustomerName}");
            return session;
        }

        public void RegisterActiveSession(ChatSession session)
        {
            _activeSessions[session.Id] = session;
            Console.WriteLine($"Assigned session | Agent: {session.AssignedAgentName} | Session ID: {session.Id}");
        }

        public bool UpdatePolling(Guid sessionId)
        {
            if (_activeSessions.TryGetValue(sessionId, out var session))
            {
                session.LastPolledAt = DateTime.UtcNow;
                _pollTimestamps[sessionId] = session.LastPolledAt;
                Console.WriteLine($" Poll updated | Session ID: {sessionId}");
                return true;
            }

            Console.WriteLine($" Poll failed | Session ID: {sessionId} not found");
            return false;
        }

        public Dictionary<string, int> GetQueueStatus()
        {
            return _chatQueue
                .GroupBy(s => s.RequestedRole ?? "Unknown")
                .ToDictionary(g => g.Key, g => g.Count());
        }

        public IReadOnlyDictionary<Guid, ChatSession> ActiveSessions => _activeSessions;

        public IReadOnlyCollection<ChatSession> GetActiveSessions()
        {
            return _activeSessions.Values;
        }

        public ChatSession? RemoveSession(Guid sessionId)
        {
            if (_activeSessions.TryGetValue(sessionId, out var session))
            {
                _activeSessions.Remove(sessionId);
                Console.WriteLine($" Session {sessionId} removed.");
                return session;
            }

            return null;
        }



        public bool RemoveInactiveSessions(TimeSpan timeout)
        {
            var threshold = DateTime.UtcNow - timeout;
            var toRemove = _activeSessions
                .Where(kvp => kvp.Value.LastPolledAt < threshold)
                .Select(kvp => kvp.Key)
                .ToList();

            foreach (var id in toRemove)
            {
                _activeSessions.Remove(id);
                Console.WriteLine($"Session {id} removed due to inactivity.");
            }

            return toRemove.Any();
        }

    }
}