using AutoMapper;
using DAL;
using Models;
using Models.ViewModels;
using ServiceLayer;
using ServiceLayer.User;
using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Service
{
    public class CompanyService : BaseService<Company>, ICompanyService
    {
        private ICompanyRepository _companyRepository;
        private readonly IUserRepository _userRepository;
        private readonly IComakershipRepository _comakershipRepository;
        private readonly IUserService _userService;
        private readonly IMapper _mapper;
        private IAzureRepository _azureRepository;

        public CompanyService(ICompanyRepository companyRepository, IUserRepository userRepository, IAzureRepository azureRepository, IComakershipRepository comakershipRepository, IUserService userService, IMapper mapper) : base(companyRepository)
        {
            _companyRepository = companyRepository;
            _userRepository = userRepository;
            _mapper = mapper;
            _azureRepository = azureRepository;
            _comakershipRepository = comakershipRepository;
            _userService = userService;
        }

        public async Task<bool> CheckifCompanyExistsAsync(int id, ClaimsIdentity user = null)
        {
            if (user != null)
            {
                if (!user.HasClaim("UserType", "CompanyUser"))
                {
                    throw new UnauthorizedAccessException("Not authorized");
                }
                if (user.FindFirst("CompanyId").Value != id.ToString())
                {
                    throw new UnauthorizedAccessException("Not authorized");
                }
            }
            return await _companyRepository.CheckifCompanyExistsAsync(id);
        }

        public async Task<bool> CheckIfCompanynameExistsAsync(string name)
        {
            return await _companyRepository.CheckIfCompanynameExistsAsync(name);
        }

        public async Task<bool> DeleteCompanyByIdAsync(int id)
        {
            return await _companyRepository.DeleteCompanyByIdAsync(id);
        }

        public async Task<bool> EditCompanyAsync(CompanyPutVM companyPutVM)
        {
            return await _companyRepository.EditCompanyAsync(companyPutVM);
        }

        public async Task<IEnumerable<ComakershipComplete>> GetAllComakershipsOfCompanyByCompanyId(int id)
        {
            var CompanyComakerships = _mapper.Map<List<ComakershipComplete>>(await _comakershipRepository.GetComakershipsFromCompany(id));
            return CompanyComakerships;
        }

        public async Task<IEnumerable<CompanyVM>> GetAllCompaniesAsync()
        {
            IEnumerable<Company> companyList = await _companyRepository.GetAllCompaniesAsync();

            return _mapper.Map<IEnumerable<CompanyVM>>(companyList);
        }

        public async Task<CompanyVM> GetCompanyByIdAsync(int id)
        {
            CompanyVM company = _mapper.Map<CompanyVM>(await _companyRepository.GetCompanyByIdAsync(id));
            company.Reviews.RemoveAll(u => !u.ForCompany);
            return company;
        }

        public async Task<Guid> UploadToBlobStorage(string B64image, int? companyId = null)
        {
            Guid guid;
            Stream stream;
            try
            {
                byte[] logo = Convert.FromBase64String(B64image);
                guid = Guid.NewGuid();
                stream = new MemoryStream(logo);
            }
            catch (Exception)
            {
                throw new FormatException("Base64 encoded string was malformed.");
            }

            bool hasUploaded = await _azureRepository.uploadImageToAzureAsync(stream, guid, companyId);

            if (hasUploaded)
            {
                return guid;
            }
            return Guid.Empty;
        }

        public async Task<Uri> GetSASUriFromAzureBlobStorage(int id)
        {
            // Retrieving GUID from company
            CompanyLogoVM companyLogoVM = _mapper.Map<CompanyLogoVM>(await _companyRepository.GetCompanyByIdAsync(id));

            // Retrieving Uri for GUID of company
            Uri uri = new Uri(await _azureRepository.retrieveAzureBlobStorageImageUri(companyLogoVM.LogoGuid.ToString(), id));

            return uri;
        }

        public async Task<bool> SaveCompanyAsync(CompanyPostVM companyPostVM)
        {
            Company company = null;
            CompanyUser companyUser = null;

            try
            {
                company = _mapper.Map<Company>(companyPostVM);
                company.RegistrationDate = DateTime.Now;                            // TODO, do this in AutoMapper as default value 
                companyUser = _mapper.Map<CompanyUser>(companyPostVM.CompanyUser);
            }
            catch (Exception)
            {
                // TODO catch in Controller for appropriate messages towards frontend.
                throw;
            }
            // checking if the email is already in use.
            if (await _userService.GetSingle(c => c.Email == companyUser.Email) != null)
            {
                throw new ArgumentException("Something is wrong with the email address or password. Please try again.");
            }

            if (await _companyRepository.Add(company))
            {
                if (companyUser != null)
                {
                    var dbCompany = await _companyRepository.GetCompanyByName(company.Name);

                    companyUser.CompanyId = dbCompany.Id;
                    companyUser.IsCompanyAdmin = true;
                    companyUser.Password = new PasswordHasher().Hash(companyUser.Password);
                    return await _userRepository.Add(companyUser);
                }
                return true;
            }
            return false;
        }

        public async Task<bool> CheckIfUserIsAuthorized(ClaimsIdentity user, int id)
        {
            if (!user.HasClaim("UserType", "CompanyUser"))
            {
                //throw new UnauthorizedAccessException();
                return await Task.FromResult(false);
            }
            if (id.ToString() == user.FindFirst("CompanyId").Value)
            {
                return await Task.FromResult(true);
            }
            return await Task.FromResult(false);
        }

        public async Task<CompanyLogoVM> GetCompanyIdAndGuidByCompanyNameAsync(string companyName)
        {
            return await _companyRepository.GetCompanyIdAndGuidByCompanyNameAsync(companyName);
        }

        public async Task<bool> MoveCompanyLogoFromGenericStorageToCompanyStorage(CompanyLogoVM companyLogoVM)
        {
            return await _azureRepository.MoveCompanyLogo(companyLogoVM.CompanyId, companyLogoVM.LogoGuid);
        }

        public async Task<bool> AddCompanyUserToCompany(CompanyAddUserVM userToAdd, ClaimsIdentity identity, int companyId)
        {
            if (identity.FindFirst("UserType").Value != "CompanyUser" ||
                int.Parse(identity.FindFirst("CompanyId").Value) != companyId ||
                identity.FindFirst("IsCompanyAdmin").Value != "True")
            {

                throw new UnauthorizedAccessException("Not authorised to perform this action");
            }

            var user = await _userRepository.GetSingle(u => u.Email == userToAdd.UserEmail);
            if (user.GetType() == typeof(CompanyUser))
            {
                var companyUser = (CompanyUser)user;

                if (companyUser.CompanyId == null)
                {
                    companyUser.CompanyId = companyId;
                    companyUser.IsCompanyAdmin = userToAdd.MakeAdmin;

                    return await _userRepository.Update(companyUser);
                }
                else
                {
                    throw new Exception("User is already in a company");
                }
            }

            return false;
        }

        public async Task<IEnumerable<CompanyUserVM>> GetAllEmployeesAsync(int companyId, ClaimsIdentity identity)
        {
            IEnumerable<CompanyUser> companyUsers = new List<CompanyUser>();
            IEnumerable<CompanyUserVM> companyUsersVM = new List<CompanyUserVM>();

            //IEnumerable<ComakershipComplete> userComakerships = new List<ComakershipComplete>();

            if (identity.FindFirst("UserType").Value == "CompanyUser" && int.Parse(identity.FindFirst("CompanyId").Value) == companyId)
            {
                companyUsers = await _userRepository.GetEmployeeUsersAsync(c => c.CompanyId == companyId);
                companyUsersVM = _mapper.Map<IEnumerable<CompanyUserVM>>(companyUsers);
                return companyUsersVM;
            }

            if (identity.FindFirst("UserType").Value == "StudentUser")
            {
                // getting a list of all comakerships of the user (currently ComakershipComplete, while a list of integers is sufficient)
                var userComakerships = await _comakershipRepository.GetComakershipsFromUser(int.Parse(identity.FindFirst("UserId").Value));

                foreach (Comakership c in userComakerships)
                {
                    if (c.Company.Id == companyId)
                    {
                        companyUsers = await _userRepository.GetEmployeeUsersAsync(c => c.CompanyId == companyId);
                        companyUsersVM = _mapper.Map<IEnumerable<CompanyUserVM>>(companyUsers);
                        return companyUsersVM;
                    }
                }
            }

            // Throw custom exception when none of the checks succeeds
            throw new UnauthorizedAccessException("Not authorized to view employees of this company.");
        }
    }
}