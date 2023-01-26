using Models;
using Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace DAL {
    public interface IUserRepository : IBaseRepository<UserBody> {

        Task<UserBody> GetUser<T>(int id) where T : UserBody;
        Task<IEnumerable<CompanyUser>> GetEmployeeUsersAsync(Expression<Func<CompanyUser, bool>> predicate);
    }
}
