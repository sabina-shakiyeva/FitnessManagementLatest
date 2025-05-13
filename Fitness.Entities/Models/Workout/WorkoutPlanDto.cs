using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fitness.Entities.Models.Workout
{
    public class WorkoutPlanDto
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public int TrainerId { get; set; }

        public List<WorkoutDayDto> Days { get; set; }
        public List<int> PackageIds { get; set; }
    }
}
