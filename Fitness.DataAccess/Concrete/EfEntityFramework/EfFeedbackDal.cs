using Fitness.Core.DataAccess.EntityFramework;
using Fitness.DataAccess.Abstract;
using Fitness.Entities.Concrete;
using FitnessManagement.Data;
using FitnessManagement.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Fitness.DataAccess.Concrete.EfEntityFramework
{
    public class EfFeedbackDal : EfEntityRepositoryBase<Feedback, GymDbContext>, IFeedbackDal
    {
        private readonly GymDbContext _context;

        public EfFeedbackDal(GymDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<List<Feedback>> GetAllWithIncludeAsync(
            Expression<Func<Feedback, bool>> filter,
            Func<IQueryable<Feedback>, IQueryable<Feedback>> include)
        {
            IQueryable<Feedback> query = _context.Set<Feedback>().Where(filter);

            if (include != null)
            {
                query = include(query);
            }

            return await query.ToListAsync();
        }
    }

}
