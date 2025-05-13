using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fitness.Entities.Models.User
{
    public class UserPackageInfoDto
    {
        public string? PackageName { get; set; }
        public decimal? PackagePrice { get; set; }
        public string? TrainerName { get; set; }
        public string? GroupName { get; set; }
    }
}
