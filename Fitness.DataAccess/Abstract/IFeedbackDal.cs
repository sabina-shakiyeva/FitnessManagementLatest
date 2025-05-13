using Fitness.Core.DataAccess;
using Fitness.Entities.Concrete;
using FitnessManagement.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Fitness.DataAccess.Abstract
{
    public interface IFeedbackDal: IEntityRepository<Feedback>
    {
        Task<List<Feedback>> GetAllWithIncludeAsync(Expression<Func<Feedback, bool>> filter, Func<IQueryable<Feedback>, IQueryable<Feedback>> include);
    }
}
