using Fitness.Core.Abstraction;
using FitnessManagement.Dtos;
using FitnessManagement.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fitness.Entities.Concrete
{
    public class WorkoutPlan:IEntity
    {
        public int Id { get; set; }
        public string Title { get; set; }            
        public string Description { get; set; }
        public int TrainerId { get; set; }
        public Trainer? Trainer { get; set; }

        public ICollection<WorkoutDay>? WorkoutDays { get; set; }
        public ICollection<PackageWorkout>? PackageWorkouts { get; set; }

    }
}
