using Fitness.Core.Abstraction;
using FitnessManagement.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fitness.Entities.Concrete
{
    public class Feedback:IEntity
    {
        public int Id { get; set; }

        public int TrainerId { get; set; }
        public int UserId { get; set; }

        public string Comment { get; set; } = string.Empty;
        public bool IsPositive { get; set; }
        public DateTime GivenAt { get; set; } = DateTime.UtcNow;

        public Trainer Trainer { get; set; }
        public User User { get; set; }
    }
}
