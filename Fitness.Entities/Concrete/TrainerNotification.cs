using Fitness.Core.Abstraction;
using FitnessManagement.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fitness.Entities.Concrete
{
    public class TrainerNotification:IEntity
    {
        public int Id { get; set; }

        public int TrainerId { get; set; }
        public int GlobalNotificationId { get; set; }
        public bool IsRead { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public GlobalNotification GlobalNotification { get; set; }
        public Trainer Trainer { get; set; }
    }
}
