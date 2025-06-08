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
        int QueueLength { get; }
        IReadOnlyCollection<ChatSession> GetAllSessions();
        bool UpdatePolling(Guid sessionId);
        Dictionary<string, int> GetQueueStatus();


    }
}
