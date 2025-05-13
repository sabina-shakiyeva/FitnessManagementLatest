using Stripe;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fitness.Business.Abstract
{
    public interface IStripeService
    {
        Task<PaymentIntent> CreatePaymentIntentAsync(decimal amount, string currency, string paymentMethodId);
        Task<PaymentIntent> ConfirmPaymentIntentAsync(string paymentIntentId);
    }
}
