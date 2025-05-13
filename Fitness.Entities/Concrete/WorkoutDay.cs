using Fitness.Core.Abstraction;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fitness.Entities.Concrete
{
    public class WorkoutDay:IEntity
    {
        public int Id { get; set; }
        public int WorkoutPlanId { get; set; }
        public WorkoutPlan? WorkoutPlan { get; set; }

        public int DayNumber { get; set; } 
        public ICollection<WorkoutExercise>? Exercises { get; set; }
    }
}
