namespace FitnessManagement.Dtos
{
    public class PackageDto
    {
      
        public string PackageName { get; set; }
        public decimal Price { get; set; }
        public int DurationInMonths { get; set; }
        public string? Description { get; set; }
    }
}
