using Fitness.Core.DataAccess.EntityFramework;
using Fitness.DataAccess.Abstract;
using Fitness.Entities.Concrete;
using FitnessManagement.Data;
using FitnessManagement.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fitness.DataAccess.Concrete.EfEntityFramework
{
    public class EfUserEquipmentUsageDal : EfEntityRepositoryBase<UserEquipmentUsage, GymDbContext>, IUserEquipmentUsageDal
    {
        public EfUserEquipmentUsageDal(GymDbContext context) : base(context)
        {
        }
    }
   
}
