using Fitness.Core.DataAccess.EntityFramework;
using Fitness.DataAccess.Abstract;
using Fitness.Entities.Concrete;
using FitnessManagement.Data;
using FitnessManagement.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fitness.DataAccess.Concrete.EfEntityFramework
{
    public class EfWorkoutPlanDal : EfEntityRepositoryBase<WorkoutPlan, GymDbContext>, IWorkoutPlanDal
    {
        private readonly GymDbContext _context;
        public EfWorkoutPlanDal(GymDbContext context) : base(context)
        {
            _context = context;
        }
        public async Task<List<WorkoutPlan>> GetAllWithIncludesAsync()
        {
            return await _context.WorkoutPlans
                .Include(p => p.WorkoutDays)
                    .ThenInclude(d => d.Exercises)
                .Include(p => p.PackageWorkouts)
                .ToListAsync();
        }
    }
}
