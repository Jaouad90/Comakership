using Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Models;

namespace DAL
{
    public interface IComakershipRepository : IBaseRepository<Comakership>
    {
        Task<IEnumerable<Comakership>> GetComakerships();

        Task<Comakership> GetComakership(int id);

        Task<Comakership> GetComakershipComplete(int id);

        Task<IEnumerable<Comakership>> FindComakershipsBySkill(string skillName);

        Task<bool> DeleteComakership(Comakership deletedModel);

        Task<bool> AddTeamToComakership(int teamId, Comakership comakership);

        Task<IEnumerable<Comakership>> GetComakershipsFromUser(int userId);
        Task<IEnumerable<Comakership>> GetComakershipsFromCompany(int companyId);

        Task<bool> RemoveTeamApplication(int teamId, Comakership comakership);

        Task<bool> RemoveStudent(Comakership comakership, int studentid);
    }
}
