using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fitness.Entities.Models
{
    public class TrainerUpdateDto
    {
      
        public string? Name { get; set; }
        public string? Email { get; set; }
        
        public string? Speciality { get; set; }
        public bool IsActive { get; set; }
        public bool IsApproved { get; set; }
        public string? Description { get; set; }
        public string? Experience { get; set; }
        public IFormFile? ImageUrl { get; set; }
        public string? CurrentPassword { get; set; }
        public string? NewPassword { get; set; } 
        public decimal? Salary { get; set; }
        //public DateTime? JoinedDate { get; set; }
        public string? MobileTelephone { get; set; }
    }
}
