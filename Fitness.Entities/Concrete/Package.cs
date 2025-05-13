using Fitness.Core.Abstraction;
using Fitness.Entities.Concrete;

namespace FitnessManagement.Entities
{
    public class Package:BaseEntity, IEntity
    {
        public string PackageName { get; set; }
        public decimal Price { get; set; }
        public int DurationInMonths { get; set; }
        public string? Description { get; set; }
        public ICollection<User> Users { get; set; }
        public ICollection<Payment> Payments { get; set; }
        public ICollection<PackageWorkout> PackageWorkouts { get; set; }
    }
}
