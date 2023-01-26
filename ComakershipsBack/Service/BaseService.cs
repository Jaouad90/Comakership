using DAL;
using Models;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace ServiceLayer {
    public class BaseService<T> : IBaseService<T> where T : class, IEntityBase, new() {
        private readonly IBaseRepository<T> _repo;

        public BaseService(IBaseRepository<T> _repo) {
            this._repo = _repo;
        }

        public async Task<bool> Add(T entity) {
            return await _repo.Add(entity);
        }

        public IEnumerable<T> AllIncluding(params Expression<Func<T, object>>[] includeProperties) {
            return _repo.AllIncluding(includeProperties);
        }

        public async Task<bool> Delete(T entity) {
            return await _repo.Delete(entity);
        }

        public async Task<bool> DeleteWhere(Expression<Func<T, bool>> predicate) {
            return await _repo.DeleteWhere(predicate);
        }

        public async Task<IEnumerable<T>> FindBy(Expression<Func<T, bool>> predicate) {
            return await _repo.FindBy(predicate);
        }

        public async Task<T> GetSingle(int id) {
            return await _repo.GetSingle(id);
        }

        public async Task<T> GetSingle(Expression<Func<T, bool>> predicate) {
            return await _repo.GetSingle(predicate);
        }

        public async Task<T> GetSingle(Expression<Func<T, bool>> predicate, params Expression<Func<T, object>>[] includeProperties) {
            return await _repo.GetSingle(predicate, includeProperties);
        }

        public async Task<bool> Update(T entity) {
            return await _repo.Update(entity);
        }
    }
}
