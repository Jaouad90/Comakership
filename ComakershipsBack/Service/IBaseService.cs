using Models;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace ServiceLayer {
    public interface IBaseService<T> where T : class, IEntityBase, new() {
        IEnumerable<T> AllIncluding(params Expression<Func<T, object>>[] includeProperties);
        Task<T> GetSingle(int id);
        Task<T> GetSingle(Expression<Func<T, bool>> predicate);
        Task<T> GetSingle(Expression<Func<T, bool>> predicate, params Expression<Func<T, object>>[] includeProperties);
        Task<IEnumerable<T>> FindBy(Expression<Func<T, bool>> predicate);
        Task<bool> Add(T entity);
        Task<bool> Delete(T entity);
        Task<bool> DeleteWhere(Expression<Func<T, bool>> predicate);
        Task<bool> Update(T entity);

    }
}
