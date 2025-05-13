using Fitness.Core.Abstraction;
using Fitness.Entities.Concrete;
using System.ComponentModel.DataAnnotations;

namespace FitnessManagement.Entities
{
    public class TrainerSchedule : BaseEntity, IEntity
    {
        public int? UserId { get; set; }
        public User? User { get; set; }

        public int TrainerId { get; set; }
        public Trainer Trainer { get; set; }

        public int? GroupId { get; set; }
        public Group? Group { get; set; }
        public int? PackageId { get; set; }
        public Package? Package { get; set; }


        public DayOfWeek DayOfWeek { get; set; } 

        public TimeOnly StartTime { get; set; }  
        public TimeOnly EndTime { get; set; }

        public string? Description { get; set; }
    }

   


}
