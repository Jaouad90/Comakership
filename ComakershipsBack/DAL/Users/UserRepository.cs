using Models;
using Models.ViewModels;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.Metadata;
using AutoMapper.Configuration.Annotations;
using System.Linq.Expressions;

namespace DAL
{
    public class UserRepository : BaseRepository<UserBody>, IUserRepository {

        public UserRepository(ComakershipsContext _context) : base(_context){
        }

        public override async Task<bool> Delete(UserBody entity) {
            entity.Deleted = true;

            return await _context.SaveChangesAsync() > 0;
        }

        public override async Task<bool> DeleteWhere(Expression<Func<UserBody, bool>> predicate) {
            IEnumerable<UserBody> entities = _context.Set<UserBody>().Where(predicate);

            foreach (var entity in entities) {
                entity.Deleted = true;
            }

            return await _context.SaveChangesAsync() > 0;
        }

        //if you want to get a user including some of its foreign properties, use this
        public async Task<UserBody> GetUser<T>(int id) where T : UserBody {
            var user = _context.Users.OfType<T>().Where(u => u.Id == id);

            //TODO: do this automatically
            if(typeof(T) == typeof(StudentUser)) {
                return await ((IQueryable<StudentUser>)user)
                    .Include(x => x.Reviews)
                    .Include(u => u.University)
                    .Include(u => u.LinkedTeams).ThenInclude(u => u.Team)
                    .FirstOrDefaultAsync();
            }

            if (typeof(T) == typeof(CompanyUser)) {
                return await ((IQueryable<CompanyUser>)user).Include(u => u.Company).FirstOrDefaultAsync();
            }

            return await user.FirstOrDefaultAsync();
        }

        // TODO rewrite this to accept types instead of a CompanyUser
        public async Task<IEnumerable<CompanyUser>> GetEmployeeUsersAsync(Expression<Func<CompanyUser, bool>> predicate)
        {
            IEnumerable<CompanyUser> entities =  await _context.CompanyUsers.Where(predicate).ToListAsync();
            return entities;
        }
    }
}
