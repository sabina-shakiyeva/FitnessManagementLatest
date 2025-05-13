using FitnessManagement.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fitness.Entities.Concrete
{
    public class UserBmiInfo:BaseEntity
    {
      
        public int UserId { get; set; }
        public User User { get; set; }
        public double WeightKg { get; set; }
        public double HeightCm { get; set; }
        public double? Bmi { get; set; }
      
    }
}
