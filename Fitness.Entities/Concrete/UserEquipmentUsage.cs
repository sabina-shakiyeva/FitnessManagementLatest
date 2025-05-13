using Fitness.Core.Abstraction;
using FitnessManagement.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fitness.Entities.Concrete
{
    public class UserEquipmentUsage:BaseEntity,IEntity
    {
        public int UserId { get; set; }
        public int EquipmentId { get; set; }

        public int? DurationInMinutes { get; set; } 
        public int? Repetition { get; set; } 
        public DateTime Date { get; set; }= DateTime.Now;

        public User User { get; set; }
        public Equipment Equipment { get; set; }
    }
}
