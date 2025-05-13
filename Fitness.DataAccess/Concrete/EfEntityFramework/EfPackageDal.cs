using Fitness.Core.DataAccess.EntityFramework;
using Fitness.DataAccess.Abstract;
using FitnessManagement.Data;
using FitnessManagement.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fitness.DataAccess.Concrete.EfEntityFramework
{
    public class EfPackageDal : EfEntityRepositoryBase<Package, GymDbContext>, IPackageDal
    {
        public EfPackageDal(GymDbContext context) : base(context)
        {
        }
    }
}
