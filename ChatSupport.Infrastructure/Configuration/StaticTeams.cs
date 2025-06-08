using ChatSupport.Domain.Models;
using ChatSupport.Domain.Constants;

namespace ChatSupport.Infrastructure.Configuration
{
    public static class StaticTeams
    {
        public static Team TeamA => new Team
        {
            Name = "Team A",
            Agents = new List<Agent>
            {
                new Agent { Name = "Alice", Role = Roles.TeamLead },
                new Agent { Name = "Bob", Role = Roles.Mid },
                new Agent { Name = "Charlie", Role = Roles.Mid },
                new Agent { Name = "Dave", Role = Roles.Junior }
            }
        };

        public static Team TeamB => new Team
        {
            Name = "Team B",
            Agents = new List<Agent>
            {
                new Agent { Name = "Eve", Role = Roles.Senior },
                new Agent { Name = "Frank", Role = Roles.Mid },
                new Agent { Name = "Grace", Role = Roles.Junior },
                new Agent { Name = "Heidi", Role = Roles.Junior }
            }
        };

        public static Team TeamC => new Team
        {
            Name = "Team C (Night)",
            Agents = new List<Agent>
            {
                new Agent { Name = "Ivan", Role = Roles.Mid },
                new Agent { Name = "Judy", Role = Roles.Mid }
            }
        };

        public static Team Overflow => new Team
        {
            Name = "Overflow Team",
            IsOverflow = true,
            Agents = new List<Agent>
            {
                new Agent { Name = "Kenny", Role = Roles.Junior },
                new Agent { Name = "Liam", Role = Roles.Junior },
                new Agent { Name = "Mallory", Role = Roles.Junior },
                new Agent { Name = "Niaj", Role = Roles.Junior },
                new Agent { Name = "Olivia", Role = Roles.Junior },
                new Agent { Name = "Peggy", Role = Roles.Junior }
            }
        };
    }
}
