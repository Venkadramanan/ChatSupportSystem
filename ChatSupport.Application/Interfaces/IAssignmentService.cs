using ChatSupport.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChatSupport.Application.Interfaces
{
    public interface IAssignmentService
    {
        Agent? AssignAgent(ChatSession session);
    }
}
