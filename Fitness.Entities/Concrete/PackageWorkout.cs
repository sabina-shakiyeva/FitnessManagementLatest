using Fitness.Core.Abstraction;
using FitnessManagement.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fitness.Entities.Concrete
{
    public class PackageWorkout:IEntity
    {
        public int PackageId { get; set; }
        public Package? Package { get; set; }

        public int WorkoutPlanId { get; set; }
        public WorkoutPlan? WorkoutPlan { get; set; }
    }
}
