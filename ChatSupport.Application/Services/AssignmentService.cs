using ChatSupport.Application.Interfaces;
using ChatSupport.Domain.Constants;
using ChatSupport.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ChatSupport.Application.Services
{
    public class AssignmentService : IAssignmentService
    {
        private readonly List<Team> _primaryTeams;
        private readonly Team _overflowTeam;
        private readonly Dictionary<string, int> _roundRobinPointers = new();

        public AssignmentService(List<Team> primaryTeams, Team overflowTeam)
        {
            _primaryTeams = primaryTeams;
            _overflowTeam = overflowTeam;

            foreach (var team in _primaryTeams.Append(_overflowTeam))
            {
                foreach (var role in new[] { Roles.Junior, Roles.Mid, Roles.Senior, Roles.TeamLead })
                {
                    var agents = team.Agents.Where(a => a.Role == role).ToList();
                    if (agents.Count > 0)
                    {
                        _roundRobinPointers.TryAdd($"{team.Name}:{role}", 0);
                    }
                }
            }
        }

        public Agent? AssignAgent(ChatSession session)
        {
            // 1. Try primary teams in order (e.g., Team A, then Team B)
            foreach (var team in _primaryTeams)
            {
                var agent = TryAssignFromTeam(team, session);
                if (agent != null)
                    return agent;
            }

            // 2. Try overflow team
            return TryAssignFromTeam(_overflowTeam, session);
        }

        private Agent? TryAssignFromTeam(Team team, ChatSession session)
        {
            foreach (var role in new[] { Roles.Junior, Roles.Mid, Roles.Senior, Roles.TeamLead })
            {
                var candidates = team.Agents.Where(a => a.Role == role && a.CanAcceptMoreChats).ToList();

                if (candidates.Count == 0)
                    continue;

                string key = $"{team.Name}:{role}";
                int index = _roundRobinPointers[key];
                var agent = candidates[index % candidates.Count];
                _roundRobinPointers[key] = (index + 1) % candidates.Count;

                agent.CurrentChatCount++;
                session.AssignedAgentName = agent.Name;

                return agent;
            }

            return null;
        }
    }
}
