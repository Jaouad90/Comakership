using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Models;

namespace ServiceLayer
{
    public interface ITeamService : IBaseService<Team>
    {
        Task<IList<TeamBasic>> GetTeams();

        Task<TeamBasic> GetTeam(int id);

        Task<TeamComplete> GetTeamComplete(int id);

        Task<int> CreateTeam(TeamPost newTeam, int userId);

        Task<bool> UpdateTeam(TeamPut updatedTeam);

        Task<bool> ApplyForComakership(int teamId, int comakershipId, ClaimsIdentity user);

        Task<IEnumerable<TeamComakershipComakershipGet>> GetApplications(int id, ClaimsIdentity identity);

        Task<bool> CreateJoinRequest(int teamid, ClaimsIdentity user);

        Task<bool> CancelJoinRequest(int teamid, ClaimsIdentity user);

        Task<IEnumerable<JoinRequestGet>> GetJoinRequests(int teamid, ClaimsIdentity user);

        Task<bool> AcceptApplication(int teamid, int studentid, ClaimsIdentity user);
        Task<bool> RejectApplication(int teamid, int studentid, ClaimsIdentity user);
        Task<bool> CreateTeamInvite(int teamid, int studentid, ClaimsIdentity user);
        Task<IEnumerable<TeamInviteGet>> GetTeamInvites(ClaimsIdentity user);
        Task<bool> AcceptTeamInvite(int teamid, ClaimsIdentity user);
        Task<bool> RejectTeamInvite(int teamid, ClaimsIdentity user);
        Task<bool> CancelTeamInviteRequest(int teamid, int studentid, ClaimsIdentity user);
        Task<IEnumerable<TeamInviteGet>> GetSentTeamInvites(int teamid, ClaimsIdentity user);
        Task<IEnumerable<JoinRequestGet>> GetSentJoinRequests(ClaimsIdentity user);
        Task<bool> LeaveTeam(int teamid, ClaimsIdentity user);
        Task<bool> KickMember(int teamid, int studentid, ClaimsIdentity user);
        Task<bool> CancelApplyForComakership(int teamid, int comakershipid, ClaimsIdentity user);
    }
}
