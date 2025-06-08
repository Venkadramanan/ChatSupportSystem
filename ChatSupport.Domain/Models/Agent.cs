using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChatSupport.Domain.Models
{
    public class Agent
    {
        public string Name { get; set; }
        public string Role { get; set; } // Junior, Mid, Senior, TeamLead
        public int CurrentChatCount { get; set; } = 0;

        public double EfficiencyMultiplier => Role switch
        {
            Constants.Roles.Junior => 0.4,
            Constants.Roles.Mid => 0.6,
            Constants.Roles.Senior => 0.8,
            Constants.Roles.TeamLead => 0.5,
            _ => throw new ArgumentException("Invalid role")
        };

        public int MaxConcurrency => (int)(10 * EfficiencyMultiplier);

        public bool CanAcceptMoreChats => CurrentChatCount < MaxConcurrency;
    }
}
