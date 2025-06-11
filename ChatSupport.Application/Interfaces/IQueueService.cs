using ChatSupport.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChatSupport.Application.Interfaces
{
    public interface IQueueService
    {
        bool TryEnqueue(ChatSession session);
        ChatSession? Dequeue();
        IReadOnlyCollection<ChatSession> GetActiveSessions();

        bool UpdatePolling(Guid sessionId);
        Dictionary<string, int> GetQueueStatus();
        void RegisterActiveSession(ChatSession session);
        ChatSession? RemoveSession(Guid sessionId);
        bool RemoveInactiveSessions(TimeSpan timeout);
        IReadOnlyDictionary<Guid, ChatSession> ActiveSessions { get; }


    }
}
