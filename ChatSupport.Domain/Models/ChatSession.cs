using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChatSupport.Domain.Models
{
    public class ChatSession
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public string CustomerName { get; set; }
        public DateTime LastPolledAt { get; set; } = DateTime.UtcNow;
        public bool IsActive { get; set; } = true;
        public string AssignedAgentName { get; set; }
        public string RequestedRole { get; set; } = string.Empty; // e.g., "Junior", "Mid", "Senior"


    }
}
