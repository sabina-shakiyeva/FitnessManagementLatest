using Fitness.Core.Abstraction;
using Fitness.Entities.Concrete;
using Microsoft.AspNetCore.Identity;

namespace FitnessManagement.Entities
{
    public class User:BaseEntity,IEntity
    {
       

        public string Name { get; set; }
        public string Email { get; set; }
        public string IdentityUserId { get; set; }
        public byte[] PasswordHash { get; set; }
        public byte[] PasswordSalt { get; set; }
        public bool IsActive { get; set; } = true;

        public bool IsApproved { get; set; } = false;

        public string? Phone { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public string? ImageUrl { get; set; }
        public int? PackageId { get; set; }
        public int? TrainerId { get; set; }
        public int Point { get; set; } = 0;
        public Trainer Trainer { get; set; } 
        public Package Package { get; set; }
        public bool? IsBlocked { get; set; }
        public DateTime? PackageStartDate { get; set; }
        public DateTime? PackageEndDate { get; set; }
        public decimal? Balance { get; set; }
        public ICollection<GroupUser> GroupUsers { get; set; }
      //  public ICollection<UserWorkout> UserWorkouts { get; set; }
        public ICollection<Payment> Payments { get; set; }
        public ICollection<Message> Messages { get; set; }

        public ICollection<UserBmiInfo> BmiInfos { get; set; }
        public ICollection<UserEquipmentUsage> EquipmentUsages { get; set; }
        public virtual ICollection<CartItem> CartItems { get; set; }




    }

}
