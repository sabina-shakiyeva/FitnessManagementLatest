using Fitness.Core.Abstraction;
using Fitness.Entities.Concrete;

namespace FitnessManagement.Entities
{
    public class Trainer:BaseEntity,IEntity
    {      
        public string Name { get; set; }
        public string Email { get; set; }
        public string IdentityTrainerId { get; set; }
        public byte[] PasswordHash { get; set; }
        public byte[] PasswordSalt { get; set; }
        public string? Speciality { get; set; }
        public string? ImageUrl { get; set; } 
        public bool IsActive { get; set; } = true;

        public bool IsApproved { get; set; } = false;
        public string? Description { get; set; }
        public string? Experience { get; set; }
        public decimal? Salary { get; set; } 
        public string? MobileTelephone { get; set; }
        public ICollection<User> Users { get; set; }

        // public ICollection<Workout> Workouts { get; set; }
        public ICollection<Group> Groups { get; set; }
        public ICollection<Message> Messages { get; set; }
        public ICollection<TrainerSchedule> TrainerSchedules { get; set; }

    }
}
