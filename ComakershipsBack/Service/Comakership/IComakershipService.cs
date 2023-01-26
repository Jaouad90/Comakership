using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Models;

namespace ServiceLayer
{
    public interface IComakershipService : IBaseService<Comakership>
    {
        Task<IList<ComakershipBasic>> GetComakerships();

        Task<ComakershipBasic> GetComakership(int id);

        Task<ComakershipComplete> GetComakershipComplete(int id);

        Task<IList<ComakershipBasic>> FindComakershipsBySkill(string skillName);

        Task<int> CreateComakership(ComakershipPost postedComakership, ClaimsIdentity user);

        Task<bool> UpdateComakership(ComakershipPut updatedComakership, ClaimsIdentity user);

        Task<bool> AcceptApplication(int teamId, int comakershipId, ClaimsIdentity user);

        Task<IEnumerable<ComakershipComplete>> GetComakershipsForUser(ClaimsIdentity identity);

        Task<IEnumerable<TeamComakershipTeamGet>> GetApplications(int id, ClaimsIdentity identity);

        Task<bool> RejectApplication(int teamid, int comakershipid, ClaimsIdentity user);

        Task<bool> LeaveComakership(int id, ClaimsIdentity user);

        Task<bool> KickStudent(int comakershipid, int studentid, ClaimsIdentity user);
    }
}
