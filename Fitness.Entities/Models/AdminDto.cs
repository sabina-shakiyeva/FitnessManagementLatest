﻿using Microsoft.AspNetCore.Http;

namespace FitnessManagement.Dtos
{
    public class AdminDto
    {
        
        public string Name { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public IFormFile? ImageUrl { get; set; }
    }
}
