using AutoMapper;
using DAL;
using Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace ServiceLayer.User {
    public class UserService : BaseService<UserBody>, IUserService {
        private readonly IUserRepository userRepo;
        private readonly IUniversityRepository universityRepo;
        private readonly IMapper mapper;

        public UserService(IUserRepository userRepo, IUniversityRepository universityRepo, IMapper mapper) : base(userRepo) {
            this.userRepo = userRepo;
            this.universityRepo = universityRepo;
            this.mapper = mapper;
        }

        public async Task<bool> DeleteUser<T>(int id, ClaimsIdentity identity) where T : UserBody {
            if (id.ToString() != identity.FindFirst("UserId").Value) { //if logged in user is not the user being deleted
                throw new UnauthorizedAccessException("Not allowed to delete someone else");
            }

            return await userRepo.DeleteWhere(u => u.Id == id);
        }

        public async Task<bool> EditUser<T>(UserBody user, int userId) where T : UserBody {
            if (user.Id != userId) { //if logged in user is not the user being edited
                throw new UnauthorizedAccessException("Not allowed to edit someone else");
            }

            if (user is StudentUser) {
                FindUniversityByDomain((StudentUser)user);
            }

            return await userRepo.Update(user);
        }

        public async Task<bool> EditStudentUser(StudentPutVM userPutVm, ClaimsIdentity identity) {
            var dbUser = (StudentUser) await GetUser<StudentUser>(int.Parse(identity.FindFirst("UserId").Value));
            var updatedUser = mapper.Map(userPutVm, dbUser);

            return await userRepo.Update(updatedUser);
        }

        public async Task<bool> EditCompanyUser(CompanyUserPutVM userPutVm, ClaimsIdentity identity) {
            var dbUser = (CompanyUser)await GetUser<CompanyUser>(int.Parse(identity.FindFirst("UserId").Value));
            var updatedUser = mapper.Map(userPutVm, dbUser);

            return await userRepo.Update(updatedUser);
        }

        public async Task<UserBody> GetUser<T>(int id) where T : UserBody {
            return await userRepo.GetUser<T>(id);
        }

        public async Task<bool> IsValidLogin(string email, string password) {
            var user = await userRepo.GetSingle(u => u.Email == email);

            if (user != null) {
                return new PasswordHasher().Check(user.Password, password);
            }

            return false;
        }

        public async Task<bool> SaveUser<T>(UserBody user) where T : UserBody {
            if(user is StudentUser) {
                FindUniversityByDomain((StudentUser)user);
            }
            user.Password = new PasswordHasher().Hash(user.Password);

            return await userRepo.Add(user);
        }

        private async void FindUniversityByDomain(StudentUser user) {
            var emailDomain = user.Email.Substring(user.Email.IndexOf('@'));
            //emailDomain = emailDomain.Trim('@');
            var uni = await universityRepo.GetByDomain(emailDomain) ?? throw new NullReferenceException("This university doesnt exist");

            user.UniversityId = uni.Id ?? 0;
        }

        public async Task<bool> ChangePassword(string oldPassword, string newPassword, string confirmNewPassword, ClaimsIdentity identity) {
            if(!newPassword.Equals(confirmNewPassword)) {
                return false;
            }

            var userId = identity.FindFirst("UserId").Value;
            var user = await userRepo.GetSingle(u => u.Id == int.Parse(userId));

            if (user != null) {
                if (new PasswordHasher().Check(user.Password, oldPassword)) {
                    user.Password = new PasswordHasher().Hash(newPassword);
                    return await userRepo.Update(user);
                }
            }

            return false;
        }

        public async Task<bool> AddSkill(string skill, ClaimsIdentity identity) {
            if (identity.FindFirst("UserType").Value == "StudentUser") {
                var user = (StudentUser)await GetUser<StudentUser>(int.Parse(identity.FindFirst("UserId").Value));

                user.Skills.Add(skill);

                return await userRepo.Update(user);
            }

            return false;
        }

        public async Task<bool> DeleteSkill(string skill, ClaimsIdentity identity) {
            if (identity.FindFirst("UserType").Value == "StudentUser") {
                var user = (StudentUser)await GetUser<StudentUser>(int.Parse(identity.FindFirst("UserId").Value));

                if (user.Skills.Remove(skill)) {
                    return await userRepo.Update(user);
                }
            }

            return false;
        }

        public async Task<bool> EditSkill(string oldSkill, string newSkill, ClaimsIdentity identity) {
            if(await DeleteSkill(oldSkill, identity)) {
                return await AddSkill(newSkill, identity);
            }

            return false;
        }
    }
}
