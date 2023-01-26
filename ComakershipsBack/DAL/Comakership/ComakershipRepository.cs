using Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.Query.SqlExpressions;
using System.IO;

namespace DAL
{
    public class ComakershipRepository : BaseRepository<Comakership>, IComakershipRepository {

        public ComakershipRepository(ComakershipsContext _context) : base(_context)
        {            
        }

        // Read one
        public async Task<Comakership> GetComakership(int id)
        {
            return await _context.Comakerships
                .Include("Company")
                .Include("Status")
                .Include(c => c.LinkedSkills).ThenInclude(c => c.Skill)
                .Include(c => c.LinkedPrograms).ThenInclude(c => c.Program)
                .Include(c => c.Applications).ThenInclude(a => a.Team).ThenInclude(t => t.LinkedStudents).ThenInclude(ls => ls.StudentUser)
                .Include(c => c.StudentUsers)
                .FirstOrDefaultAsync(c => c.Id == id);
        }

        // Read one, include many-to-many skills
        public async Task<Comakership> GetComakershipComplete(int id)
        {
            return await _context.Comakerships
                .Include("Status")
                .Include("Company")
                .Include("Deliverables")
                .Include(c => c.LinkedSkills).ThenInclude(c => c.Skill)
                .Include(c => c.LinkedPrograms).ThenInclude(c => c.Program)
                .Include(c => c.StudentUsers).ThenInclude(su => su.StudentUser)
                .FirstOrDefaultAsync(c => c.Id == id);
        }

        // Read all
        public async Task<IEnumerable<Comakership>> GetComakerships()
        {
            return await _context.Comakerships
                .Include("Company")
                .Include("Status")
                .Include(c => c.LinkedSkills).ThenInclude(c => c.Skill)
                .Include(c => c.LinkedPrograms).ThenInclude(c => c.Program)
                .ToListAsync();
        }

        // Read all
        public async Task<IEnumerable<Comakership>> FindComakershipsBySkill(string skillName)
        {
            return await _context.ComakershipSkills
                .Include(c => c.Comakership).ThenInclude(x => x.Company)
                .Include(c => c.Comakership).ThenInclude(x => x.LinkedSkills).ThenInclude(y => y.Skill)
                .Include(c => c.Skill)
                .Where(cs => cs.Skill.Name.Contains(skillName))
                .Select(cs => cs.Comakership)
                .ToListAsync();          
        }        

        // Delete
        public async Task<bool> DeleteComakership(Comakership deletedComakership)
        {
            var comakership = await _context.Comakerships.FirstOrDefaultAsync(c => c.Id == deletedComakership.Id);

            if (comakership != null)
            {
                _context.Comakerships.Remove(comakership);
                return await _context.SaveChangesAsync() > 0;
            }
            return false;
        }

        public async Task<bool> AddTeamToComakership(int teamId, Comakership comakership) {
            var team = _context.Teams.Include(t => t.LinkedStudents).FirstOrDefault(t => t.Id == teamId);

            if(team != null) {
                foreach (var user in team.LinkedStudents) {
                    // If the user is not already part of the comakership, add the user
                    if (!comakership.StudentUsers.Any(s => s.StudentUserId == user.StudentUserId))
                    {
                        comakership.StudentUsers.Add(new UserComakership()
                        {
                            StudentUserId = user.StudentUserId,
                            ComakershipId = comakership.Id
                        });
                    }                    
                }
                var application = comakership.Applications.First(a => a.TeamId == teamId);                
                comakership.Applications.Remove(application);

                return await _context.SaveChangesAsync() > 0;
            } else {
                throw new Exception("Team with this id doesn't exist");
            }
        }

        public async Task<bool> RemoveTeamApplication(int teamId, Comakership comakership)
        {                
            var application = comakership.Applications.First(a => a.TeamId == teamId);
            comakership.Applications.Remove(application);

            return await _context.SaveChangesAsync() > 0;           
        }


        public async Task<IEnumerable<Comakership>> GetComakershipsFromUser(int userId) {
            return await _context.Comakerships
                .Where(c => c.StudentUsers.Where(s => s.StudentUserId == userId).Any() == true)
                .Include("Status")
                .Include("Company")
                .Include("Deliverables")
                .Include(c => c.LinkedSkills).ThenInclude(c => c.Skill)
                .Include(c => c.LinkedPrograms).ThenInclude(c => c.Program)
                .Include(c => c.StudentUsers).ThenInclude(su => su.StudentUser)
                .ToListAsync();
        }

        public async Task<IEnumerable<Comakership>> GetComakershipsFromCompany(int companyId)
        {
            return await _context.Comakerships
                .Where(c => c.CompanyId == companyId)
                .Include("Status")
                .Include("Company")
                .Include("Deliverables")
                .Include(c => c.LinkedSkills).ThenInclude(c => c.Skill)
                .Include(c => c.LinkedPrograms).ThenInclude(c => c.Program)
                .Include(c => c.StudentUsers).ThenInclude(su => su.StudentUser)
                .ToListAsync();
        }

        public async Task<bool> RemoveStudent(Comakership comakership, int studentid)
        {
            var studentComakership = comakership.StudentUsers.First(su => su.StudentUserId == studentid);
            comakership.StudentUsers.Remove(studentComakership);

            return await _context.SaveChangesAsync() > 0;
        }
    }
}
