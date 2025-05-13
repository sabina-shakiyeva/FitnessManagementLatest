using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fitness.Entities.Models.Trainer
{
    public class TrainerStatisticsDto
    {
        public int TotalMember { get; set; }
        public int TotalMyMember { get; set; }
        public int TotalEquipment { get; set; }
        public int NumberOfPackages { get; set; }
    }
}
