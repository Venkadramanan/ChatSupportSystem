using ChatSupport.Application.Services;
using ChatSupport.Domain.Constants;
using ChatSupport.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChatSupport.Tests
{
    public class QueueServiceTests
    {
        private Team GetTeam(List<(string role, int count)> agents)
        {
            var team = new Team();
            foreach (var (role, count) in agents)
            {
                for (int i = 0; i < count; i++)
                {
                    team.Agents.Add(new Agent { Role = role });
                }
            }
            return team;
        }

        [Fact]
        public void TryEnqueue_ShouldAccept_WhenUnderLimit()
        {
            var team = GetTeam(new() { (Roles.Mid, 2) }); // 2 * 6 = 12 capacity
            var overflow = GetTeam(new());
            var service = new QueueService(team, overflow, false);

            for (int i = 0; i < team.MaxQueueLength; i++)
            {
                var session = new ChatSession();
                Assert.True(service.TryEnqueue(session));
            }

            // Next session should be rejected
            Assert.False(service.TryEnqueue(new ChatSession()));
        }

        [Fact]
        public void TryEnqueue_ShouldUseOverflow_WhenInOfficeHours()
        {
            var baseTeam = GetTeam(new() { (Roles.Mid, 2) }); // 12
            var overflow = GetTeam(new() { (Roles.Junior, 6) }); // 6 * 4 = 24
            var service = new QueueService(baseTeam, overflow, true);

            int totalCapacity = baseTeam.Capacity + overflow.Capacity; // 12 + 24 = 36
            int maxQueue = (int)(totalCapacity * 1.5); // 54

            for (int i = 0; i < maxQueue; i++)
            {
                Assert.True(service.TryEnqueue(new ChatSession()));
            }

            // Should now reject
            Assert.False(service.TryEnqueue(new ChatSession()));
        }

        [Fact]
        public void Dequeue_ShouldReturnInFifoOrder()
        {
            var team = GetTeam(new() { (Roles.Junior, 2) });
            var overflow = GetTeam(new());
            var service = new QueueService(team, overflow, false);

            var session1 = new ChatSession();
            var session2 = new ChatSession();

            service.TryEnqueue(session1);
            service.TryEnqueue(session2);

            var dequeued1 = service.Dequeue();
            var dequeued2 = service.Dequeue();

            Assert.Equal(session1.Id, dequeued1.Id);
            Assert.Equal(session2.Id, dequeued2.Id);
        }

        [Fact]
        public void QueueLength_ShouldReturnCorrectCount()
        {
            var team = GetTeam(new() { (Roles.Mid, 1) }); // Capacity = 6
            var overflow = GetTeam(new());
            var service = new QueueService(team, overflow, false);

            service.TryEnqueue(new ChatSession());
            service.TryEnqueue(new ChatSession());

            Assert.Equal(2, service.QueueLength);
        }
    }
}
