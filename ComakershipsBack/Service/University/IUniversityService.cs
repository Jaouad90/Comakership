using Models;
using Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Service
{
    public interface IUniversityService
    {
        Task<IEnumerable<University>> GetAllUniversitiesAsync();

        Task<University> GetUniversityByIdAsync(int? id);

        Task<bool> EditUniversityAsync(UniversityPutVM universityPutVM);

        Task<bool> DeleteUniversityByIdAsync(int? id);

        Task<IEnumerable<UniversityDomainVM>> GetAllUniversityDomainVMAsync();

        Task<UniversityDomainVM> GetUniversityDomainByIdAsync(int? id);


        Task<bool> SaveUniversityAsync(UniversityPostVM universityPostVM);

        Task<bool> CheckIfUniversityExistsAsync(int id);

        Task<bool> CheckIfUniversitynameExistsAsync(string name);
    }
}
