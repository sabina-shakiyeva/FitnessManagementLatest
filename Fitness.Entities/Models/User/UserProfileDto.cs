using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fitness.Entities.Models.User
{
	public class UserProfileDto
	{
		public int Id { get; set; }
		public string Name { get; set; }
		public string Email { get; set; }
		public string? Phone { get; set; }
		public string? PackageName { get; set; }
        public string? TrainerName { get; set; }
        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;//joined date
		public DateTime? DateOfBirth { get; set; }
		public string? ImageUrl { get; set; }
		public int? TrainerId { get; set; }//traner name-i gorsenmelidi
		public int? PackageId { get; set; }//package name-i
        public int? Point { get; set; }
    }
}
