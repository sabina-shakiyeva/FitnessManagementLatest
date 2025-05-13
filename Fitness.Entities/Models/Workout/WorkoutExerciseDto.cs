using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fitness.Entities.Models.Workout
{
    public class WorkoutExerciseDto
    {
        public int EquipmentId { get; set; }
        public string EquipmentName { get; set; }
        public int Repetitions { get; set; }
        public TimeSpan Duration { get; set; }
    }
}
