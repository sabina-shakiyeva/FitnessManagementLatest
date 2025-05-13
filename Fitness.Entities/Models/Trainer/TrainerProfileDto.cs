using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fitness.Entities.Models.Trainer
{
	public class TrainerProfileDto
	{
		public int Id { get; set; }
		public string Name { get; set; }
		public string Email { get; set; }
		public string? ImageUrl { get; set; }
		public string? Experience { get; set; }
		public decimal? Salary { get; set; }
		public DateTime CreatedDate { get; set; } = DateTime.UtcNow;//frontu yazanda joined date kimi qeyd et bunu
		public string? MobileTelephone { get; set; }
	}
}
