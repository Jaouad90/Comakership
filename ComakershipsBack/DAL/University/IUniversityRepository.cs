using Models;
using Models.ViewModels;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DAL
{
    public interface IUniversityRepository
    {
        Task<IEnumerable<University>> GetAllUniversitiesAsync();

        Task<University> GetUniversityByIdAsync(int? id);

        Task<bool> EditUniversityAsync(University university);

        Task<bool> DeleteUniversityByIdAsync(int id);

        Task<IEnumerable<University>> GetAllUniversityDomainVMAsync();

        Task<UniversityDomainVM> GetUniversityDomainByIdAsync(int? id);


        Task<bool> SaveUniversityAsync(University university);

        Task<bool> CheckIfUniversityExistsAsync(int id);

        Task<bool> CheckIfUniversitynameExistsAsync(string name);

        Task<University> GetByDomain(string domain);
    }
}
