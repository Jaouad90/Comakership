using Models;
using Models.ViewModels;
using ServiceLayer;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Service
{
    public interface ICompanyService : IBaseService<Company>
    {
        Task<IEnumerable<CompanyVM>> GetAllCompaniesAsync();

        Task<CompanyVM> GetCompanyByIdAsync(int id);

        Task<bool> CheckIfUserIsAuthorized(ClaimsIdentity user, int id);

        Task<bool> EditCompanyAsync(CompanyPutVM companyPutVM);

        Task<bool> DeleteCompanyByIdAsync(int id);

        Task<bool> SaveCompanyAsync(CompanyPostVM companyPostVM);

        Task<bool> CheckifCompanyExistsAsync(int id, ClaimsIdentity user = null);

        Task<bool> CheckIfCompanynameExistsAsync(string name);

        Task<IEnumerable<ComakershipComplete>> GetAllComakershipsOfCompanyByCompanyId(int companyId);
        Task<Guid> UploadToBlobStorage(string B64image, int? companyId = null);
        Task<Uri> GetSASUriFromAzureBlobStorage(int id);

        Task<CompanyLogoVM> GetCompanyIdAndGuidByCompanyNameAsync(string companyName);

        Task<bool> MoveCompanyLogoFromGenericStorageToCompanyStorage(CompanyLogoVM companyLogoVM);

        Task<bool> AddCompanyUserToCompany(CompanyAddUserVM userToAdd, ClaimsIdentity user, int companyId);

        Task<IEnumerable<CompanyUserVM>> GetAllEmployeesAsync(int companyId, ClaimsIdentity identity);
    }
}
