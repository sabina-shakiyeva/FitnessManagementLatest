using System.ComponentModel.DataAnnotations;

namespace Fitness.Entities.Models.Trainer
{
    public class TrainerScheduleDto
    {
        public int Id { get; set; }
        public int? TrainerId { get; set; }
        public int? UserId { get; set; }
        public int GroupId { get; set; }
        public DayOfWeek DayOfWeek { get; set; }
        public TimeOnly StartTime { get; set; }
        public TimeOnly EndTime { get; set; }

        public string Description { get; set; }
    }
}
