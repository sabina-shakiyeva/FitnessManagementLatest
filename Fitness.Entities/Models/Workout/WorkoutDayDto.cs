using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fitness.Entities.Models.Workout
{
    public class WorkoutDayDto
    {
        public int DayNumber { get; set; }
        public List<WorkoutExerciseDto> Exercises { get; set; }
    }
}
