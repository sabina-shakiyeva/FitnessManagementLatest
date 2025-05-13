using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fitness.Entities.Models.Group
{
    public class GroupCreateDto
    {
        public string Name { get; set; }
        public int PackageId { get; set; }
        public int TrainerId { get; set; }
    }
}
