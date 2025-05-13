using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fitness.Entities.Models
{
    public class AdminUpdateDto
    {
       
        public string? Name { get; set; }
        public string? Email { get; set; }
        public IFormFile? ImageUrl { get; set; }
        public string? CurrentPassword { get; set; }
        public string? NewPassword { get; set; }
    }
}
