using Models;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace ServiceLayer.User {
    public interface IUserService : IBaseService<UserBody> {

        Task<UserBody> GetUser<T>(int id) where T : UserBody;

        Task<bool> SaveUser<T>(UserBody user) where T : UserBody;

        Task<bool> EditUser<T>(UserBody user, int userId) where T : UserBody;

        Task<bool> EditStudentUser(StudentPutVM user, ClaimsIdentity identity);

        Task<bool> EditCompanyUser(CompanyUserPutVM user, ClaimsIdentity identity);

        Task<bool> DeleteUser<T>(int id, ClaimsIdentity identity) where T : UserBody;

        Task<bool> IsValidLogin(string email, string password);

        Task<bool> ChangePassword(string oldPassword, string newPassword, string confirmNewPassword, ClaimsIdentity identity);

        Task<bool> AddSkill(string skill, ClaimsIdentity identity);

        Task<bool> DeleteSkill(string skill, ClaimsIdentity identity);

        Task<bool> EditSkill(string oldSkill, string newSkill, ClaimsIdentity identity);
    }
}
