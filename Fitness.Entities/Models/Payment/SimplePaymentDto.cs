using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fitness.Entities.Models.Payment
{
    public class SimplePaymentDto
    {
        public decimal Amount { get; set; }
        public DateTime PaymentDate { get; set; }
    }
}
