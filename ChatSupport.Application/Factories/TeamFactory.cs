using ChatSupport.Domain.Constants;
using ChatSupport.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChatSupport.Application.Factories
{
    public static class TeamFactory
    {
        public static List<Team> BuildAllTeams()
        {
            return new List<Team>
        {
            CreateTeamA(),
            CreateTeamB(),
            CreateTeamC(),
            CreateOverflowTeam()
        };
        }

        public static Team CreateTeamA()
        {
            return new Team
            {
                Name = "Team A",
                Agents = new List<Agent>
            {
                new Agent { Name = "Venkad", Role = Roles.TeamLead },
                new Agent { Name = "Jhon", Role = Roles.Mid },
                new Agent { Name = "Muthu", Role = Roles.Mid },
                new Agent { Name = "Dhana", Role = Roles.Junior }
            }
            };
        }

        public static Team CreateTeamB()
        {
            return new Team
            {
                Name = "Team B",
                Agents = new List<Agent>
            {
                new Agent { Name = "Rumesh", Role = Roles.Senior },
                new Agent { Name = "Sanju", Role = Roles.Mid },
                new Agent { Name = "KN", Role = Roles.Junior },
                new Agent { Name = "Nandi", Role = Roles.Junior }
            }
            };
        }

        public static Team CreateTeamC()
        {
            return new Team
            {
                Name = "Team C (Night)",
                Agents = new List<Agent>
            {
                new Agent { Name = "Ivy", Role = Roles.Mid },
                new Agent { Name = "Jack", Role = Roles.Mid }
            }
            };
        }

        public static Team CreateOverflowTeam()
        {
            return new Team
            {
                Name = "Overflow",
                Agents = new List<Agent>
            {
                new Agent { Name = "O1", Role = Roles.Junior },
                new Agent { Name = "O2", Role = Roles.Junior },
                new Agent { Name = "O3", Role = Roles.Junior },
                new Agent { Name = "O4", Role = Roles.Junior },
                new Agent { Name = "O5", Role = Roles.Junior },
                new Agent { Name = "O6", Role = Roles.Junior }
            }
            };
        }
    }
}
