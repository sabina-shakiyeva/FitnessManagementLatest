using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fitness.Entities.Models
{
    public class UserPackageTrainerDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string? ImageUrl { get; set; }
        public string? Phone { get; set; }
        public string? PackageName { get; set; }
        public decimal? PackagePrice { get; set; }
        public string? TrainerName { get; set; }

    }
}
