using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fitness.Entities.Models.Trainer
{
    public class TrainerScheduleCreateDto
    {
        public int TrainerId { get; set; }
        public int? UserId { get; set; }     
        public int? GroupId { get; set; }
        public DayOfWeek DayOfWeek { get; set; }
        public TimeComponentDto StartTime { get; set; }
        public TimeComponentDto EndTime { get; set; }
      

        public string Description { get; set; }
    }
    public class TimeComponentDto
    {
        public int Hour { get; set; }
        public int Minute { get; set; }
    }
}
