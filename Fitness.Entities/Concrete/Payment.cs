using Fitness.Core.Abstraction;

namespace FitnessManagement.Entities
{
    public class Payment:IEntity
    {

        public int Id { get; set; }
        public int UserId { get; set; }
        public int? PackageId { get; set; }
        public User User { get; set; }
        public Package? Package { get; set; }
        public decimal Amount { get; set; }
        public DateTime PaymentDate { get; set; }

    }
}
