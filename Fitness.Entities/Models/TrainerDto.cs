using Microsoft.AspNetCore.Http;

namespace FitnessManagement.Dtos
{
    public class TrainerDto
    {
        public string Name { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string? Speciality { get; set; }
        public bool IsActive { get; set; }
        public bool IsApproved { get; set; }
        public string? Description { get; set; }
        public string? Experience { get; set; }
        public IFormFile? ImageUrl { get; set; }
        public decimal? Salary { get; set; }
        //public DateTime? JoinedDate { get; set; }
        public string? MobileTelephone { get; set; }
    }
}
