using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fitness.Entities.Models.Trainer
{
    public class TrainerUpdateProfileDto
    {
        public string? Name { get; set; }
        public string? Email { get; set; }

        public string? Speciality { get; set; }
     
        public string? Description { get; set; }
        public string? Experience { get; set; }
        public IFormFile? ImageUrl { get; set; }
        public string? CurrentPassword { get; set; }
        public string? NewPassword { get; set; }
        public decimal? Salary { get; set; } 
        public string? MobileTelephone { get; set; }
    }
}
