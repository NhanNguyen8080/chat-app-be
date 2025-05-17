using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace ChatService.Repository.Repository
{
    public interface IGenericRepository<T> where T : class
    {
        Task<IEnumerable<T>> GetAll();
        Task<IQueryable<T>> Get(
                Expression<Func<T, bool>> filter = null,
                Func<IQueryable<T>, IOrderedQueryable<T>> orderBy = null,
                string includeProperties = "",
                int? pageIndex = null, int? pageSize = null);
        Task<T> GetByID(object id);
        Task InsertAsync(T entity);
        Task DeleteAsync(object id);
        Task DeleteAsync(T entityToDelete);
        Task UpdateAsync(T entityToUpdate);
    }
}
