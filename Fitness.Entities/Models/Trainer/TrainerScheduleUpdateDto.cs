using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fitness.Entities.Models.Trainer
{
    public class TrainerScheduleUpdateDto
    {
        public int Id { get; set; }
        public DayOfWeek DayOfWeek { get; set; }
       
        public TimeComponentDto StartTime { get; set; }
        public TimeComponentDto EndTime { get; set; }

        public string Description { get; set; }
    }
}
