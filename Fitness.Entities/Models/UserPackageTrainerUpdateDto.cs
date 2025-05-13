using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fitness.Entities.Models
{
    public class UserPackageTrainerUpdateDto
    {
        public string? Name { get; set; }
        public string? Phone { get; set; }
        public int? PackageId { get; set; }
        public int? TrainerId { get; set; }
        public IFormFile? ImageUrl { get; set; }
    }
}
