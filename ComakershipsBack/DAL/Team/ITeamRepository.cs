using Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DAL
{
    public interface ITeamRepository : IBaseRepository<Team>
    {
        Task<IEnumerable<Team>> GetTeams();

        Task<Team> GetTeam(int id);

        Task<Team> GetTeamComplete(int id);

        Task<bool> AddMember(StudentTeam newMember);

        Task<bool> RemoveMember(Team team, int studentId);

        Task<bool> ApplyForComakership(Team team, int comakershipId);

        Task<bool> CreateJoinRequest(Team team, int studentId);
        Task<bool> CancelJoinRequest(Team team, int studentId);
        Task<bool> AcceptApplication(Team team, int studentId);
        Task<bool> RejectApplication(Team team, int studentId);
        Task<bool> CreateTeamInvite(Team team, int studentId);
        Task<IEnumerable<TeamInvite>> FindTeamInvitesByUser(int userId);
        Task<bool> AcceptTeamInvite(Team team, int userId);
        Task<bool> RejectTeamInvite(Team team, int userId);
        Task<bool> CancelTeamInviteRequest(Team team, int studentId);
        Task<IEnumerable<JoinRequest>> JoinRequestsByUserId(int studentId);
        Task<bool> CancelApplyForComakership(Team team, int comakershipId);
    }
}
