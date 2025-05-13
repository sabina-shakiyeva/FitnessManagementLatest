using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fitness.Entities.Models
{
    public class TopUserDto
    {
        public int UserId { get; set; }
        public string? ImageUrl { get; set; }
        public string Name { get; set; }
        public int Point { get; set; }
    }
}
