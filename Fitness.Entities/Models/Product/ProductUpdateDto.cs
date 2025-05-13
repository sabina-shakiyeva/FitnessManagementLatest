using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fitness.Entities.Models.Product
{
	public class ProductUpdateDto
	{
		
		public string? Name { get; set; }
		public string? Description { get; set; }
		public int? PointCost { get; set; }
		public int? Stock { get; set; }
		public IFormFile? ImageUrl { get; set; }
	}
}
