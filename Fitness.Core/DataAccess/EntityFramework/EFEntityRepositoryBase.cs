using Fitness.Core.Abstraction;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Fitness.Core.DataAccess.EntityFramework
{
    public class EfEntityRepositoryBase<TEntity, TContext>
     : IEntityRepository<TEntity>
     where TEntity : class, IEntity, new()
     where TContext : DbContext
    {
        private readonly TContext _context;

        public EfEntityRepositoryBase(TContext context)
        {
            _context = context;
        }
       // public async Task<List<TEntity>> GetList(
       //Expression<Func<TEntity, bool>> filter,
       //Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>> include)
       // {
       //     IQueryable<TEntity> query = _context.Set<TEntity>();

       //     if (include != null)
       //         query = include(query);

       //     if (filter != null)
       //         query = query.Where(filter);

       //     return await query.ToListAsync();
       // }


        public async Task Add(TEntity entity)
        {
            var addedEntity = _context.Entry(entity);
            addedEntity.State = EntityState.Added;
            await _context.SaveChangesAsync();
        }
        public async Task<List<TEntity>> GetList(Expression<Func<TEntity, bool>> filter, Func<IQueryable<TEntity>, IQueryable<TEntity>> include)
        {
            IQueryable<TEntity> query = _context.Set<TEntity>();

            if (include != null)
                query = include(query);

            if (filter != null)
                query = query.Where(filter);

            return await query.ToListAsync();
        }
    //    public async Task<List<TEntity>> GetList(
    //Expression<Func<TEntity, bool>> filter = null,
    //Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>> include = null)
    //    {
    //        IQueryable<TEntity> query = _context.Set<TEntity>();

    //        if (include != null)
    //            query = include(query);

    //        if (filter != null)
    //            query = query.Where(filter);

    //        return await query.ToListAsync();
    //    }

        public async Task<TEntity> Get(Expression<Func<TEntity, bool>> filter, Func<IQueryable<TEntity>, IQueryable<TEntity>> include)
        {
            IQueryable<TEntity> query = _context.Set<TEntity>();
            if (include != null)
                query = include(query);

            return await query.FirstOrDefaultAsync(filter);
        }

        public async Task Delete(TEntity entity)
        {
            var deletedEntity = _context.Entry(entity);
            deletedEntity.State = EntityState.Deleted;
            await _context.SaveChangesAsync();
        }

        public async Task<TEntity> Get(Expression<Func<TEntity, bool>> filter)
        {
            return await _context.Set<TEntity>().FirstOrDefaultAsync(filter);
        }

        public async Task<List<TEntity>> GetList(Expression<Func<TEntity, bool>> filter = null)
        {
            return filter == null ?
                await _context.Set<TEntity>().ToListAsync() :
                await _context.Set<TEntity>().Where(filter).ToListAsync();
        }

        public async Task Update(TEntity entity)
        {
            var updatedEntity = _context.Entry(entity);
            updatedEntity.State = EntityState.Modified;
            await _context.SaveChangesAsync();
        }
        public async Task<TEntity> GetByIdAsync(int id)
        {
            return await _context.Set<TEntity>().FindAsync(id);
        }

        public async Task<bool> AnyAsync(Expression<Func<TEntity, bool>> predicate)
        {
            return await _context.Set<TEntity>().AnyAsync(predicate);
        }
        public async Task<List<TEntity>> GetAllAsync()
        {
            return await _context.Set<TEntity>().ToListAsync();
        }

    }

}
