using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using DAL;
using Models;

namespace ServiceLayer
{
    public class TeamService : BaseService<Team>, ITeamService
    {
        private readonly ITeamRepository _teamRepository;
        private readonly IMapper _mapper;
        public TeamService(ITeamRepository teamRepository, IMapper mapper) : base(teamRepository)
        {
            _teamRepository = teamRepository;
            _mapper = mapper;
        }

        public async Task<int> CreateTeam(TeamPost postedTeam, int userId)
        {
            Team newTeam = _mapper.Map<Team>(postedTeam);            
            if (await _teamRepository.Add(newTeam))
            {
                if (await _teamRepository.AddMember(new StudentTeam { StudentUserId = userId, TeamId = newTeam.Id }))
                {
                    return newTeam.Id;
                }
                throw new Exception("Team created, but failed to add user as member.");
            }
            else throw new Exception("Unable to add team due to internal error");            
        }

        public async Task<TeamBasic> GetTeam(int id)
        {
            var team = await _teamRepository.GetTeam(id);
            return _mapper.Map<TeamBasic>(team);
        }

        public async Task<TeamComplete> GetTeamComplete(int id)
        {
            var team = await _teamRepository.GetTeamComplete(id);
            return _mapper.Map<TeamComplete>(team);
        }

        public async Task<IList<TeamBasic>> GetTeams()
        {
            var teams = await _teamRepository.GetTeams();
            return _mapper.Map<List<TeamBasic>>(teams);
        }

        public async Task<bool> UpdateTeam(TeamPut updatedTeam)
        {
            var dbTeam = await _teamRepository.GetTeam(updatedTeam.Id);
            if (dbTeam == null)
            {
                return false;
            }
            _mapper.Map(updatedTeam, dbTeam);
            return await _teamRepository.Update(dbTeam);
        }

        public async Task<bool> ApplyForComakership(int teamId, int comakershipId, ClaimsIdentity user) {
            var team = await _teamRepository.GetTeam(teamId);
            var id = user.FindFirst("UserId").Value;

            //if logged in user is part of the team that is trying to apply to a comakership
            if (team.LinkedStudents.Any(s => s.StudentUserId == int.Parse(id))) {
                return await _teamRepository.ApplyForComakership(team, comakershipId);
            } else {
                throw new UnauthorizedAccessException("You're not part of the team you're trying to apply with");
            }
        }

        public async Task<IEnumerable<TeamComakershipComakershipGet>> GetApplications(int id, ClaimsIdentity user)
        {
            var team = await _teamRepository.GetTeam(id);
            var userId = user.FindFirst("UserId").Value;

            if (team != null && userId != null && team.LinkedStudents.Any(s => s.StudentUserId == int.Parse(userId)))
            {
                return _mapper.Map<List<TeamComakershipComakershipGet>>(team.AppliedComakerships);
            }
            return null;
        }

        public async Task<bool> CreateJoinRequest(int teamId, ClaimsIdentity user)
        {
            var team = await _teamRepository.GetTeam(teamId);
            var userId = user.FindFirst("UserId").Value;

            //if currently logged in user is not already part of the team
            if (!team.LinkedStudents.Any(s => s.StudentUserId == int.Parse(userId)))
            {
                return await _teamRepository.CreateJoinRequest(team, int.Parse(userId));
            }
            else
            {
                throw new Exception("You are already part of this team");
            }            
        }

        public async Task<bool> CancelJoinRequest(int teamId, ClaimsIdentity user)
        {
            var team = await _teamRepository.GetTeam(teamId);
            var userId = user.FindFirst("UserId").Value;

            if (team != null)
            {
                return await _teamRepository.CancelJoinRequest(team, int.Parse(userId));
            }
            return false;         
        }

        public async Task<IEnumerable<JoinRequestGet>> GetJoinRequests(int teamId, ClaimsIdentity user)
        {
            var team = await _teamRepository.GetTeam(teamId);
            var userId = user.FindFirst("UserId").Value;

            if (team != null && userId != null && team.LinkedStudents.Any(s => s.StudentUserId == int.Parse(userId)))
            {
                return _mapper.Map<List<JoinRequestGet>>(team.JoinRequests);
            }
            return null;
        }

        public async Task<bool> AcceptApplication(int teamId, int studentId, ClaimsIdentity user)
        {
            var team = await _teamRepository.GetTeam(teamId);
            var id = user.FindFirst("UserId").Value;

            //if logged in user is part of the team that is trying to apply to a comakership
            if (team.LinkedStudents.Any(s => s.StudentUserId == int.Parse(id)))
            {
                return await _teamRepository.AcceptApplication(team, studentId);
            }
            else
            {
                throw new UnauthorizedAccessException("You're not part of this team");
            }
        }

        public async Task<bool> RejectApplication(int teamId, int studentId, ClaimsIdentity user)
        {
            var team = await _teamRepository.GetTeam(teamId);
            var id = user.FindFirst("UserId").Value;

            //if logged in user is part of the team that is trying to apply to a comakership
            if (team.LinkedStudents.Any(s => s.StudentUserId == int.Parse(id)))
            {
                return await _teamRepository.RejectApplication(team, studentId);
            }
            else
            {
                throw new UnauthorizedAccessException("You're not part of this team");
            }
        }

        public async Task<bool> CreateTeamInvite(int teamId, int studentId, ClaimsIdentity user)
        {        
            var team = await _teamRepository.GetTeam(teamId);
            var id = user.FindFirst("UserId").Value;

            //if logged in user is part of the team that is inviting
            if (team.LinkedStudents.Any(s => s.StudentUserId == int.Parse(id)))
            {              
                //if the invited player is not already part of the team
                if (!team.LinkedStudents.Any(s => s.StudentUserId == studentId))
                {
                    return await _teamRepository.CreateTeamInvite(team, studentId);
                }
                else
                {
                    throw new Exception("That student is already part of the team");
                }                
            }
            else
            {
                throw new UnauthorizedAccessException("You're not part of this team");
            }
        }

        public async Task<IEnumerable<TeamInviteGet>> GetTeamInvites(ClaimsIdentity user)
        {
            var id = user.FindFirst("UserId").Value;
            var teaminvites = await _teamRepository.FindTeamInvitesByUser(int.Parse(id));

            return _mapper.Map<List<TeamInviteGet>>(teaminvites);
        }

        public async Task<bool> AcceptTeamInvite(int teamId, ClaimsIdentity user)
        {
            var team = await _teamRepository.GetTeam(teamId);
            var id = user.FindFirst("UserId").Value;

            return await _teamRepository.AcceptTeamInvite(team, int.Parse(id));           
        }

        public async Task<bool> RejectTeamInvite(int teamId, ClaimsIdentity user)
        {
            var team = await _teamRepository.GetTeam(teamId);
            var id = user.FindFirst("UserId").Value;

            return await _teamRepository.RejectTeamInvite(team, int.Parse(id));           
        }

        public async Task<bool> CancelTeamInviteRequest(int teamId, int studentid, ClaimsIdentity user)
        {
            var team = await _teamRepository.GetTeam(teamId);
            var userId = user.FindFirst("UserId").Value;

            //if logged in user is part of the team that is inviting
            if (team != null && team.LinkedStudents.Any(s => s.StudentUserId == int.Parse(userId)))
            {
                return await _teamRepository.CancelTeamInviteRequest(team, studentid);
            }
            else
            {
                throw new UnauthorizedAccessException("You're not part of this team");
            }          
        }

        public async Task<IEnumerable<TeamInviteGet>> GetSentTeamInvites(int teamId, ClaimsIdentity user)
        {
            var team = await _teamRepository.GetTeam(teamId);
            var userId = user.FindFirst("UserId").Value;

            if (team != null && team.LinkedStudents.Any(s => s.StudentUserId == int.Parse(userId)))
            {
                return _mapper.Map<List<TeamInviteGet>>(team.TeamInvites);
            }
            return null;
        }

        public async Task<IEnumerable<JoinRequestGet>> GetSentJoinRequests(ClaimsIdentity user)
        {            
            var userId = user.FindFirst("UserId").Value;
            var joinRequests = await _teamRepository.JoinRequestsByUserId(int.Parse(userId));

            if (joinRequests != null)
            {
                return _mapper.Map<List<JoinRequestGet>>(joinRequests);
            }
            return null;
        }

        public async Task<bool> LeaveTeam(int teamId, ClaimsIdentity user)
        {
            var team = await _teamRepository.GetTeam(teamId);
            var userId = user.FindFirst("UserId").Value;

            //if logged in user is part of the team he/she is leaving
            if (team != null && team.LinkedStudents.Any(s => s.StudentUserId == int.Parse(userId)))
            {
                return await _teamRepository.RemoveMember(team, int.Parse(userId));
            }
            else
            {
                throw new UnauthorizedAccessException("You're not part of this team");
            }
        }

        public async Task<bool> KickMember(int teamId, int studentid, ClaimsIdentity user)
        {
            var team = await _teamRepository.GetTeam(teamId);
            var userId = user.FindFirst("UserId").Value;

            //if logged in user is part of the team that is kicking
            if (team != null && team.LinkedStudents.Any(s => s.StudentUserId == int.Parse(userId)))
            {
                if (team.LinkedStudents.Any(s => s.StudentUserId == studentid))
                {
                    return await _teamRepository.RemoveMember(team, studentid);
                }
                else throw new KeyNotFoundException("That student isn't part of this team");                
            }
            else throw new UnauthorizedAccessException("You're not part of this team");            
        }

        public async Task<bool> CancelApplyForComakership(int teamId, int comakershipId, ClaimsIdentity user)
        {
            var team = await _teamRepository.GetTeam(teamId);
            var userId = user.FindFirst("UserId").Value;

            //if logged in user is part of the team
            if (team.LinkedStudents.Any(s => s.StudentUserId == int.Parse(userId)))
            {
                return await _teamRepository.CancelApplyForComakership(team, comakershipId);
            }
            else
            {
                throw new UnauthorizedAccessException("You're not part of that team");
            }
        }
    }
}
