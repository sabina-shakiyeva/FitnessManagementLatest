using Fitness.Core.Abstraction;
using Microsoft.EntityFrameworkCore.Query;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Fitness.Core.DataAccess
{
    public interface IEntityRepository<T> where T : class, IEntity, new()
    {
       
        Task<T> Get(Expression<Func<T, bool>> filter);
        Task<List<T>> GetList(Expression<Func<T, bool>> filter = null);
        Task<List<T>> GetList(Expression<Func<T, bool>> filter, Func<IQueryable<T>, IQueryable<T>> include);
        //Task<List<T>> GetList(Expression<Func<T, bool>> filter = null, Func<IQueryable<T>, IIncludableQueryable<T, object>> include = null);
    //    Task<List<T>> GetList(
    //Expression<Func<T, bool>> filter = null,
    //Func<IQueryable<T>, IIncludableQueryable<T, object>> include = null);

        Task<T> Get(Expression<Func<T, bool>> filter, Func<IQueryable<T>, IQueryable<T>> include);

        Task Add(T entity);
        Task Update(T entity);
        Task Delete(T entity);
        Task<T> GetByIdAsync(int id);
        Task<bool> AnyAsync(Expression<Func<T, bool>> predicate);
        Task<List<T>> GetAllAsync();
    }

}
