using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fitness.Entities.Models.Group
{
    public class GroupGetDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int PackageId { get; set; }
        public string PackageName { get; set; }
        public int TrainerId { get; set; }
        public string TrainerName { get; set; }
    }
}
