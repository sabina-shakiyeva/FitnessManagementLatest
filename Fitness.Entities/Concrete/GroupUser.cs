using Fitness.Core.Abstraction;
using FitnessManagement.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fitness.Entities.Concrete
{
    public class GroupUser:BaseEntity,IEntity
    {
        public int GroupId { get; set; } 
        public Group Group { get; set; } 

        public int UserId { get; set; } 
        public User User { get; set; } 
    }
}
