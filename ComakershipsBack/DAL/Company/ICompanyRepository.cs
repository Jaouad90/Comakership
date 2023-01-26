using Models;
using Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace DAL
{
    public interface ICompanyRepository : IBaseRepository<Company>
    {
        Task<IEnumerable<Company>> GetAllCompaniesAsync();

        Task<Company> GetCompanyByIdAsync(int id);

        Task<bool> EditCompanyAsync(CompanyPutVM companyPutVM);

        Task<bool> DeleteCompanyByIdAsync(int id);

        Task<bool> CheckifCompanyExistsAsync(int id);

        Task<bool> CheckIfCompanynameExistsAsync(string name);

        Task<Company> GetCompanyByName(string name);

        Task<CompanyLogoVM> GetCompanyIdAndGuidByCompanyNameAsync(string companyName);
    }
}
