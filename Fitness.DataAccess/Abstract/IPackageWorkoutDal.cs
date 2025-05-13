using Fitness.Core.DataAccess;
using Fitness.Entities.Concrete;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fitness.DataAccess.Abstract
{
    public interface IPackageWorkoutDal: IEntityRepository<PackageWorkout>
    {
    }
}
