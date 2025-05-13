using Fitness.Core.Abstraction;
using FitnessManagement.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fitness.Entities.Concrete
{
    public class PurchaseHistory:IEntity
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int PackageId { get; set; }
        public decimal Amount { get; set; }
        public DateTime PurchaseDate { get; set; }
        public DateTime? PaymentDate { get; set; }  
        public bool IsPaid { get; set; } 
        public bool IsActive { get; set; } 
        public bool IsMonthlyPayment { get; set; }  
        public string PackageName { get; set; }
        public decimal PaidAmount { get; set; } = 0;
        public decimal TotalAmount { get; set; }
        public Package Package { get; set; }
    }

}
