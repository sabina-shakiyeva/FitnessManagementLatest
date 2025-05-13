using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fitness.Entities.Models.PurchaseHistory
{
    public class PurchaseHistoryGetDto
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int PackageId { get; set; }
        public decimal Amount { get; set; }
        public DateTime PurchaseDate { get; set; }
        public DateTime? PaymentDate { get; set; }
        public bool IsPaid { get; set; }
        public bool IsActive { get; set; }
        public string PackageName { get; set; }
        public bool IsMonthlyPayment { get; set; }
        public decimal PaidAmount { get; set; }
        public decimal TotalAmount { get; set; }
    }
}
