using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChatSupport.Domain.Models
{
    public class Team
    {
        public string Name { get; set; }
        public List<Agent> Agents { get; set; } = new();
        public bool IsOverflow { get; set; } = false;
        public int Capacity => Agents.Sum(agent => agent.MaxConcurrency);
        public int MaxQueueLength => (int)(Capacity * 1.5);

    }
}
