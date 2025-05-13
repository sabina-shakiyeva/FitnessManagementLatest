using Fitness.Core.Abstraction;

namespace FitnessManagement.Entities
{
    public class Admin:BaseEntity,IEntity
    {
        public string Name { get; set; }
        public string Email { get; set; }
        public string IdentityAdminId { get; set; }
        public byte[] PasswordHash { get; set; }
        public byte[] PasswordSalt { get; set; }
        public string? ImageUrl { get; set; }
    }
}
