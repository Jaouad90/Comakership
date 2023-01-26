using DAL;
using Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Security.Claims;
using Microsoft.EntityFrameworkCore.Internal;
using System.Linq;
using AutoMapper;
using Service;

namespace ServiceLayer
{    
    public class ComakershipService : BaseService<Comakership>, IComakershipService
    {
        private readonly IComakershipRepository _comakershipRepository;
        private readonly IProgramRepository _programRepository;
        private readonly IPurchaseKeyRepository _purchaseKeyRepository;
        private readonly IMapper _mapper;
        

        public ComakershipService(IComakershipRepository comakershipRepository, IProgramRepository programRepository, IPurchaseKeyRepository purchaseKeyRepository, IMapper mapper) : base(comakershipRepository)
        {
            _comakershipRepository = comakershipRepository;
            _programRepository = programRepository;
            _purchaseKeyRepository = purchaseKeyRepository;
            _mapper = mapper;
        }

        public async Task<bool> AcceptApplication(int teamId, int comakershipId, ClaimsIdentity user) {
            var comakership = await _comakershipRepository.GetComakership(comakershipId);

            var userCompanyId = user.FindFirst("Companyid");

            if (userCompanyId != null && comakership.CompanyId.ToString() == userCompanyId.Value) {
                if (comakership.Applications.Where(a => a.TeamId == teamId).Any()) {

                    return await _comakershipRepository.AddTeamToComakership(teamId, comakership);

                } else {
                    throw new UnauthorizedAccessException("This team doesn't have an application for this comakership");
                }
            } else {
                throw new UnauthorizedAccessException("Can't edit another company's comakerships");
            }            
        }

        public async Task<bool> RejectApplication(int teamId, int comakershipId, ClaimsIdentity user)
        {
            var comakership = await _comakershipRepository.GetComakership(comakershipId);
            var userCompanyId = user.FindFirst("Companyid");

            if (userCompanyId != null && comakership.CompanyId.ToString() == userCompanyId.Value)
            {
                if (comakership.Applications.Where(a => a.TeamId == teamId).Any())
                {
                    return await _comakershipRepository.RemoveTeamApplication(teamId, comakership);
                }
                else
                {
                    throw new UnauthorizedAccessException("This team doesn't have an application for this comakership");
                }
            }
            else
            {
                throw new UnauthorizedAccessException("Can't edit another company's comakerships");
            }
        }


        public async Task<int> CreateComakership(ComakershipPost postedComakership, ClaimsIdentity user)
        {
            // check company id
            var companyid = user.FindFirst("CompanyId").Value;
            if (companyid == null)
            {
                throw new Exception("company id can't be resolved");
            }

            // check program id's
            if (postedComakership.ProgramIds.Count < 1)
            {
                throw new Exception("Please provide at least 1 program id");
            }
            foreach (int programId in postedComakership.ProgramIds)
            {
                var program = await _programRepository.GetSingle(programId);
                if (program != null)
                {
                    continue;
                }
                else throw new Exception("Not all programs are valid");
            }

            // check purchaseKey
            string key = postedComakership.PurchaseKey;
            var purchasekey = await _purchaseKeyRepository.GetSingle(pk => pk.Key == key);
            if (purchasekey == null || purchasekey.Claimed == true)
            {
                throw new Exception("Purchasekey not valid");
            }

            // map and create comakership
            var comakership = _mapper.Map<Comakership>(postedComakership);
            comakership.CompanyId = int.Parse(companyid);

            // For every skill that was posted, create a new ComakershipSkill and add it to the comakerships collection of ComakershipSkill
            // This creates the many-to-many relation between Comakerships and Skills
            List<ComakershipSkill> linkedSkills = new List<ComakershipSkill>();
            foreach (Skill skill in comakership.Skills)
            {
                linkedSkills.Add(new ComakershipSkill
                {
                    Comakership = comakership,
                    Skill = skill
                });
            }
            comakership.LinkedSkills = linkedSkills;

            // For every program that was posted, create a new ComakershipProgram and add it to the comakerships collection of ComakershipProgram
            // This creates the many-to-many relation between Comakerships and Programs
            List<ComakershipProgram> linkedPrograms = new List<ComakershipProgram>();
            foreach (int programId in comakership.ProgramIds)
            {
                var program = await _programRepository.GetSingle(programId);
                linkedPrograms.Add(new ComakershipProgram
                {
                    Comakership = comakership,
                    Program = program
                });
            }
            comakership.LinkedPrograms = linkedPrograms;

            // persist the comakership
            if (await _comakershipRepository.Add(comakership))
            {
                // tag purchaseKey as claimed if is succesfull
                purchasekey.Claimed = true;
                if (await _purchaseKeyRepository.Update(purchasekey))
                {
                    // return id of new comakership
                    return comakership.Id;
                }
                throw new Exception("Error updating key");
            }
            throw new Exception("Error adding comakership to database");
        }

        public async Task<IList<ComakershipBasic>> FindComakershipsBySkill(string skillName)
        {
            var comakerships = await _comakershipRepository.FindComakershipsBySkill(skillName);
            return _mapper.Map<List<ComakershipBasic>>(comakerships);
        }

        public async Task<ComakershipBasic> GetComakership(int id)
        {
            var comakership = await _comakershipRepository.GetComakership(id);
            return _mapper.Map<ComakershipBasic>(comakership);
        }

        public async Task<ComakershipComplete> GetComakershipComplete(int id)
        {
            var comakership = await _comakershipRepository.GetComakershipComplete(id);
            return _mapper.Map<ComakershipComplete>(comakership);
        }

        public async Task<IList<ComakershipBasic>> GetComakerships()
        {
            var comakerships = await _comakershipRepository.GetComakerships();
            return _mapper.Map<List<ComakershipBasic>>(comakerships);
        }

        public async Task<bool> UpdateComakership(ComakershipPut updatedComakership, ClaimsIdentity user)
        {
            var dbComakership = await _comakershipRepository.GetComakership(updatedComakership.Id);
            var userCompanyId = user.FindFirst("Companyid").Value;
            
            if (dbComakership == null || userCompanyId == null || int.Parse(userCompanyId) != dbComakership.CompanyId)
            {
                return false;
            }
            // Change the editted values from putData to comakership from db and store updated comakership
            _mapper.Map(updatedComakership, dbComakership);
            return await _comakershipRepository.Update(dbComakership);           
        }

        public async Task<IEnumerable<ComakershipComplete>> GetComakershipsForUser(ClaimsIdentity identity) {
            var userId = identity.FindFirst("UserId");
            
            if (identity.HasClaim("UserType", "CompanyUser"))
            {
                var companyId = identity.FindFirst("CompanyId");
                var comakerships = await _comakershipRepository.GetComakershipsFromCompany(int.Parse(companyId.Value));
                return _mapper.Map<List<ComakershipComplete>>(comakerships);
            }
            if(userId != null) {
                var comakerships = await _comakershipRepository.GetComakershipsFromUser(int.Parse(userId.Value));
                return _mapper.Map<List<ComakershipComplete>>(comakerships);
            }

            return null;
        }

        public async Task<IEnumerable<TeamComakershipTeamGet>> GetApplications(int id, ClaimsIdentity user)
        {
            var comakership = await _comakershipRepository.GetComakership(id);
            var userCompanyId = user.FindFirst("Companyid").Value;            

            if (comakership != null && userCompanyId != null && int.Parse(userCompanyId) == comakership.CompanyId)
            {
                return _mapper.Map<List<TeamComakershipTeamGet>>(comakership.Applications);
            }
            return null;
        }

        public async Task<bool> LeaveComakership(int comakershipId, ClaimsIdentity user)
        {
            var comakership = await _comakershipRepository.GetComakership(comakershipId);
            var userId = user.FindFirst("UserId").Value;

            //if logged in user is part of the team he/she is leaving
            if (comakership != null && comakership.StudentUsers.Any(s => s.StudentUserId == int.Parse(userId)))
            {
                return await _comakershipRepository.RemoveStudent(comakership, int.Parse(userId));
            }
            else
            {
                throw new UnauthorizedAccessException("You're not part of this comakership");
            }
        }

        public async Task<bool> KickStudent(int comakershipId, int studentId, ClaimsIdentity user)
        {
            var comakership = await _comakershipRepository.GetComakership(comakershipId);
            var userCompanyId = user.FindFirst("Companyid").Value;

            if (userCompanyId != null && comakership != null && comakership.CompanyId.ToString() == userCompanyId)
            {
                if (comakership.StudentUsers.Any(s => s.StudentUserId == studentId))
                {
                    return await _comakershipRepository.RemoveStudent(comakership, studentId);
                }
                else
                {
                    throw new Exception("That student isn't working on this comakership");
                }
            }
            else
            {
                throw new UnauthorizedAccessException("Can't edit another company's comakerships");
            }
        }
    }
}
