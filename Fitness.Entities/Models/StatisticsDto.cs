using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fitness.Entities.Models
{
    public class StatisticsDto
    {
        public int? NumberOfUsers { get; set; }
        public int? NumberOfTrainers { get; set; }
        public int? NumberOfEquipments { get; set; }
        public int? NumberOfPackages { get; set; }
    }
}
