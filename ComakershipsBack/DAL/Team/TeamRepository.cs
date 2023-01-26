using Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DAL
{
    public class TeamRepository : BaseRepository<Team>, ITeamRepository {

        public TeamRepository(ComakershipsContext _context) :base(_context)
        {            
        }

        // Read one
        public async Task<Team> GetTeam(int id)
        {
            return await _context.Teams
                .Include(t => t.LinkedStudents)
                .Include(t => t.AppliedComakerships).ThenInclude(a => a.Comakership).ThenInclude(c => c.Company)
                .Include(t => t.JoinRequests).ThenInclude(jr => jr.StudentUser)
                .Include(t => t.TeamInvites).ThenInclude(ti => ti.StudentUser)
                .FirstOrDefaultAsync(t => t.Id == id);
        }        
        
        // Read one complete
        public async Task<Team> GetTeamComplete(int id)
        {
            return await _context.Teams
                .Include(t => t.LinkedStudents).ThenInclude(t => t.StudentUser)
                .FirstOrDefaultAsync(t => t.Id == id);
        }

        // Read all
        public async Task<IEnumerable<Team>> GetTeams()
        {
            return await _context.Teams.ToListAsync();
        }      

        public async Task<bool> AddMember(StudentTeam newMember)
        {
            // check if student and team exist
            var student = await _context.StudentUsers.FindAsync(newMember.StudentUserId);
            var team = await _context.Teams.FindAsync(newMember.TeamId);
            if (student != null && team != null)
            {
                _context.StudentTeams.Add(newMember);
                return await _context.SaveChangesAsync() > 0;
            }
            return false;
        }

        public async Task<bool> RemoveMember(Team team, int studentId)
        {
            var studentTeam = await _context.StudentTeams.FindAsync(studentId, team.Id);
            _context.StudentTeams.Remove(studentTeam);
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<bool> ApplyForComakership(Team team, int comakershipId) {
            //if the comakership exists and has no students yet
            if (_context.Comakerships.Any(c => c.Id == comakershipId)) {
                team.AppliedComakerships.Add(new TeamComakership {
                    ComakershipId = comakershipId,
                    TeamId = team.Id
                });

                return await _context.SaveChangesAsync() > 0;
            } else {
                throw new UnauthorizedAccessException("Comakership doesn't exist");
            }
        }

        public async Task<bool> CreateJoinRequest(Team team, int userId)
        {            
            var user = await _context.StudentUsers.FindAsync(userId);
            if (user != null)
            {
                _context.JoinRequests.Add(new JoinRequest
                {
                    StudentUserId = user.Id,
                    TeamId = team.Id
                });
                return await _context.SaveChangesAsync() > 0;
            }
            return false;
        }

        public async Task<bool> CancelJoinRequest(Team team, int userId)
        {
            var joinRequest = await _context.JoinRequests.FindAsync(userId, team.Id);
            
            if (joinRequest != null)
            {
                _context.JoinRequests.Remove(joinRequest);
                return await _context.SaveChangesAsync() > 0;
            }
            return false;
        }

        public async Task<bool> AcceptApplication(Team team, int studentId)
        {
            var joinRequest = await _context.JoinRequests.FindAsync(studentId, team.Id);

            if (joinRequest != null)
            {
                _context.StudentTeams.Add(new StudentTeam { TeamId = team.Id, StudentUserId = studentId });
                _context.JoinRequests.Remove(joinRequest);
                return await _context.SaveChangesAsync() > 0;
            }
            return false;
        }

        public async Task<bool> RejectApplication(Team team, int studentId)
        {
            var joinRequest = await _context.JoinRequests.FindAsync(studentId, team.Id);

            if (joinRequest != null)
            {
                _context.JoinRequests.Remove(joinRequest);
                return await _context.SaveChangesAsync() > 0;
            }
            return false;
        }

        public async Task<bool> CreateTeamInvite(Team team, int studentId)
        {
            var user = await _context.StudentUsers.FindAsync(studentId);
            if (user != null)
            {
                _context.TeamInvites.Add(new TeamInvite
                {
                    StudentUserId = user.Id,
                    TeamId = team.Id
                });
                return await _context.SaveChangesAsync() > 0;
            }
            return false;
        }

        public async Task<IEnumerable<TeamInvite>> FindTeamInvitesByUser(int userId)
        {
            return await _context.TeamInvites.Where(t => t.StudentUserId == userId)
                .Include("Team")
                .ToListAsync();            
        }

        public async Task<bool> AcceptTeamInvite(Team team, int studentId)
        {
            var teamInvite = await _context.TeamInvites.FindAsync(studentId, team.Id);

            if (teamInvite != null)
            {
                _context.StudentTeams.Add(new StudentTeam { TeamId = team.Id, StudentUserId = studentId });
                _context.TeamInvites.Remove(teamInvite);
                return await _context.SaveChangesAsync() > 0;
            }
            return false;
        }

        public async Task<bool> RejectTeamInvite(Team team, int studentId)
        {
            var teamInvite = await _context.TeamInvites.FindAsync(studentId, team.Id);

            if (teamInvite != null)
            {
                _context.TeamInvites.Remove(teamInvite);
                return await _context.SaveChangesAsync() > 0;
            }
            return false;
        }

        public async Task<bool> CancelTeamInviteRequest(Team team, int studentId)
        {
            var teamInvite = await _context.TeamInvites.FindAsync(studentId, team.Id);

            if (teamInvite != null)
            {
                _context.TeamInvites.Remove(teamInvite);
                return await _context.SaveChangesAsync() > 0;
            }
            return false;
        }

        public async Task<IEnumerable<JoinRequest>> JoinRequestsByUserId(int studentId)
        {
            return await _context.JoinRequests.Where(t => t.StudentUserId == studentId)
                            .Include("Team")
                            .ToListAsync();
        }

        public async Task<bool> CancelApplyForComakership(Team team, int comakershipId)
        {
            var comakershipApplication = team.AppliedComakerships.Where(ac => ac.ComakershipId == comakershipId).First();

            if (comakershipApplication != null)
            {
                team.AppliedComakerships.Remove(comakershipApplication);
                return await _context.SaveChangesAsync() > 0;
            }
            return false;
        }
    }
}
