using Microsoft.AspNetCore.Http;

namespace FitnessManagement.Dtos
{
    public class EquipmentDto
    {
       
        public string Name { get; set; }
        public string? Description { get; set; } 
        public bool IsAvailable { get; set; }
        public decimal? Price { get; set; }
        public decimal? Unit { get; set; }
        public IFormFile? ImageUrl { get; set; }
    }
}
