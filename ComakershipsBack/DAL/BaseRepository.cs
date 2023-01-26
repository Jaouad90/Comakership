using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace DAL
{
    //TODO write tests
    public class BaseRepository<T> : IBaseRepository<T> where T : class, IEntityBase, new()
    {
        protected ComakershipsContext _context;

        public BaseRepository(ComakershipsContext context)
        {
            _context = context;
        }

        public virtual async Task<bool> Add(T entity) {
            await _context.Set<T>().AddAsync(entity);
            return await _context.SaveChangesAsync() > 0;
        }

        public virtual IEnumerable<T> AllIncluding(params Expression<Func<T, object>>[] includeProperties) {
            IQueryable<T> query = _context.Set<T>();
            foreach (var includeProperty in includeProperties) {
                query = query.Include(includeProperty);
            }
            return query.AsEnumerable();
        }

        public virtual async Task<bool> Delete(T entity) {
            EntityEntry dbEntityEntry = _context.Entry(entity);
            dbEntityEntry.State = EntityState.Deleted;

            return await _context.SaveChangesAsync() > 0;
        }

        public virtual async Task<bool> DeleteWhere(Expression<Func<T, bool>> predicate) {
            IEnumerable<T> entities = _context.Set<T>().Where(predicate);

            foreach (var entity in entities) {
                _context.Entry<T>(entity).State = EntityState.Deleted;
            }

            return await _context.SaveChangesAsync() > 0;
        }

        public virtual async Task<IEnumerable<T>> FindBy(Expression<Func<T, bool>> predicate) {
            return await _context.Set<T>().Where(predicate).ToListAsync();
        }

        public virtual async Task<T> GetSingle(int id) {
            return await _context.Set<T>().Where(x => x.Id == id).FirstOrDefaultAsync();
        }

        public virtual async Task<T> GetSingle(Expression<Func<T, bool>> predicate) {
            return await _context.Set<T>().FirstOrDefaultAsync(predicate);
        }

        public virtual async Task<T> GetSingle(Expression<Func<T, bool>> predicate, params Expression<Func<T, object>>[] includeProperties) {
            IQueryable<T> query = _context.Set<T>();
            foreach (var includeProperty in includeProperties) {
                query = query.Include(includeProperty);
            }
            return await query.Where(predicate).FirstOrDefaultAsync();
        }

        public virtual async Task<bool> Update(T entity) {
            EntityEntry dbEntityEntry = _context.Entry<T>(entity);
            dbEntityEntry.State = EntityState.Modified;

            return await _context.SaveChangesAsync() > 0;
        }
    }
}
