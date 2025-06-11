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
        private readonly IQueueService _queueService;

        public AssignmentService(List<Team> primaryTeams, Team overflowTeam,IQueueService queueService)
        {
            _primaryTeams = primaryTeams;
            _overflowTeam = overflowTeam;
            _queueService = queueService;

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

        public Agent? AssignAgent(ChatSession session, bool isOfficeHours)
        {
            var rolePriority = new[] { Roles.Junior, Roles.Mid, Roles.Senior, Roles.TeamLead };
            var availableTeams = isOfficeHours ? _primaryTeams : new List<Team>();

            foreach (var role in rolePriority)
            {
                session.RequestedRole = role;

                var candidates = availableTeams
                    .SelectMany(t => t.Agents)
                    .Where(a => a.Role == role && a.CanAcceptMoreChats)
                    .ToList();

                if (candidates.Count > 0)
                {
                    if (!_roundRobinPointers.ContainsKey(role))
                        _roundRobinPointers[role] = 0;

                    int index = _roundRobinPointers[role];
                    var agent = candidates[index % candidates.Count];
                    _roundRobinPointers[role] = (index + 1) % candidates.Count;

                    agent.CurrentChatCount++;
                    return agent;
                }
            }

            // Try overflow team (always allowed)
            var fallback = _overflowTeam.Agents
                .Where(a => a.CanAcceptMoreChats)
                .OrderBy(a => rolePriority.ToList().IndexOf(a.Role)) // Still respect role priority
                .FirstOrDefault();

            if (fallback != null)
            {
                fallback.CurrentChatCount++;
                session.RequestedRole = fallback.Role;
                return fallback;
            }

            return null; // No agent available
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

        public List<Agent> GetAllAgents()
        {
            return _primaryTeams
                .SelectMany(t => t.Agents)
                .Concat(_overflowTeam.Agents)
                .ToList();
        }

        public bool RemoveSessionAndReleaseAgent(Guid sessionId)
        {
            var session = _queueService.RemoveSession(sessionId);

            if (session == null || string.IsNullOrEmpty(session.AssignedAgentName))
                return false;

            var agent = _primaryTeams
                .SelectMany(t => t.Agents)
                .Concat(_overflowTeam.Agents)
                .FirstOrDefault(a => a.Name == session.AssignedAgentName);

            if (agent != null)
            {
                agent.CurrentChatCount = Math.Max(0, agent.CurrentChatCount - 1);
                Console.WriteLine($"Agent {agent.Name} released | CurrentSessions: {agent.CurrentChatCount}");
                return true;
            }

            return false;
        }


    }
}
