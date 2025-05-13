using Fitness.Core.Abstraction;
using FitnessManagement.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fitness.Entities.Concrete
{
    public class Group: BaseEntity, IEntity
    {
       
        public string Name { get; set; }
        public int PackageId { get; set; } 
        public Package Package { get; set; }
        public int TrainerId { get; set; }
        public Trainer Trainer { get; set; }
        public ICollection<GroupUser> GroupUsers { get; set; } 
        public ICollection<TrainerSchedule> TrainerSchedules { get; set; }
    }
}
