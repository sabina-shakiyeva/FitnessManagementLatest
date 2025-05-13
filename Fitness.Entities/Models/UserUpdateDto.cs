using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fitness.Entities.Models
{
    public class UserUpdateDto
    {
       
        public string? Name { get; set; }
        public string? Email { get; set; }
        public bool IsActive { get; set; }
        public bool IsApproved { get; set; }
        public bool IsBlocked { get; set; }
        public string? Phone { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public IFormFile? ImageUrl { get; set; }

        public int? PackageId { get; set; }
        public int? TrainerId { get; set; }
        public string? CurrentPassword { get; set; }
        public string? NewPassword { get; set; }

    }
}
