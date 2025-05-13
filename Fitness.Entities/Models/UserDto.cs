using Microsoft.AspNetCore.Http;

namespace FitnessManagement.Dtos
{
    public class UserDto
    {
       
        public string Name { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }   
        public bool IsActive { get; set; }
        public bool IsApproved { get; set; }
        public string? Phone { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public IFormFile? ImageUrl { get; set; }
       
    }
}
