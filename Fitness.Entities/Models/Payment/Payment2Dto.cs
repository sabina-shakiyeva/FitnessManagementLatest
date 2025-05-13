using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fitness.Entities.Models.Payment
{
    public class Payment2Dto
    {
        public int PackageId { get; set; }
        public string CardNumber { get; set; }
        public string CardHolder { get; set; }
        public string ExpiryDate { get; set; }
        public string CVV { get; set; }
        public bool IsMonthlyPayment { get; set; }
        public decimal Amount { get; set; }
        
    }
}
