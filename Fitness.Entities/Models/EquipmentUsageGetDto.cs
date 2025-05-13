using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fitness.Entities.Models
{
    public class EquipmentUsageGetDto
    {
        public int UserId { get; set; }
        public int EquipmentId { get; set; }
        public int? DurationInMinutes { get; set; }
        public int? Repetition { get; set; }
        public string? EquipmentName { get; set; }
        public string? UserName { get; set; }

        public DateTime? Date { get; set; } = DateTime.Now;
    }
}
