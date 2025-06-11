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
                new Agent { Name = "Team A Lead", Role = Roles.TeamLead },
                new Agent { Name = "Team A Mid 1", Role = Roles.Mid },
                new Agent { Name = "Team A Mid 2", Role = Roles.Mid },
                new Agent { Name = "Team A Junior", Role = Roles.Junior }
            }
        };

        public static Team TeamB => new Team
        {
            Name = "Team B",
            Agents = new List<Agent>
            {
                //new Agent { Name = "Team B Senior", Role = Roles.Senior },
                //new Agent { Name = "Team B Mid", Role = Roles.Mid },
                new Agent { Name = "Team B Junior 1", Role = Roles.Junior },
                new Agent { Name = "Team B Junior 2", Role = Roles.Junior }
            }
        };

        public static Team TeamC => new Team
        {
            Name = "Team C (Night)",
            Agents = new List<Agent>
            {
                new Agent { Name = "Team C Mid 1", Role = Roles.Mid },
                new Agent { Name = "Team C Mid 2", Role = Roles.Mid }
            }
        };

        public static Team Overflow => new Team
        {
            Name = "Overflow Team",
            IsOverflow = true,
            Agents = new List<Agent>
            {
                new Agent { Name = "OFT 1", Role = Roles.Junior },
                //new Agent { Name = "OFT 2", Role = Roles.Junior },
                //new Agent { Name = "OFT 3", Role = Roles.Junior },
                //new Agent { Name = "OFT 4", Role = Roles.Junior },
                //new Agent { Name = "OFT 5", Role = Roles.Junior },
                //new Agent { Name = "OFT 6", Role = Roles.Junior }
            }
        };
    }
}
