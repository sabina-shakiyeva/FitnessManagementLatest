using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fitness.Entities.Models.Group
{
    public class AddUserToGroupDto
    {
        public int GroupId { get; set; }
        public int UserId { get; set; }
    }
}
