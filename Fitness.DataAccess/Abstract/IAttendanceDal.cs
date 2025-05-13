using Fitness.Core.DataAccess;
using FitnessManagement.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fitness.DataAccess.Abstract
{
    public interface IAttendanceDal:IEntityRepository<Attendance>
    {
    }
}
