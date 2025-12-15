using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace POS_TranVietTraLam_Fresher_DAL.Defines
{
    public interface IGenericRepository<T> where T : class
    {
        Task<IEnumerable<T>> GetAllAsync(
            Expression<Func<T, bool>>? filter = null,
            params Expression<Func<T, object>>[] includes);
        Task<T?> FirstOrDefaultAsync(Expression<Func<T, bool>> predicate);
        Task<T?> GetByIdAsync(object id);
        Task<T?> GetByIdAsync(
            Expression<Func<T, bool>> predicate,
            params Expression<Func<T, object>>[] includes);
        Task AddAsync(T entity);
        Task UpdateAsync(T entity);
        Task DeleteAsync(T entity);
        Task<T?> FindAsync(Expression<Func<T, bool>> predicate);
        Task<List<T>> FindAllAsync(Expression<Func<T, bool>> predicate);
        Task<int> SaveChangesAsync();
        Task AddRangeAsync(IEnumerable<T> entities);
    }
}
