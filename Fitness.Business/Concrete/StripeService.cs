using Fitness.Business.Abstract;
using Fitness.Entities.Concrete;
using Microsoft.Extensions.Options;
using Stripe;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fitness.Business.Concrete
{
    public class StripeService:IStripeService
    {
        private readonly StripeSettings _stripeSettings;
        public StripeService(IOptions<StripeSettings> stripeSettings)
        {
            _stripeSettings = stripeSettings.Value;
            StripeConfiguration.ApiKey = _stripeSettings.SecretKey;
        }

        public async Task<PaymentIntent> CreatePaymentIntentAsync(decimal amount, string currency, string paymentMethodId)
        {
            var options = new PaymentIntentCreateOptions
            {
                Amount = (long)(amount * 100), // Stripe ödənişləri yalnız cent ilə işləyir
                Currency = currency,
                PaymentMethod = paymentMethodId,
                Confirm = true
            };

            var service = new PaymentIntentService();
            return await service.CreateAsync(options);
        }

        public async Task<PaymentIntent> ConfirmPaymentIntentAsync(string paymentIntentId)
        {
            var service = new PaymentIntentService();
            return await service.ConfirmAsync(paymentIntentId);
        }
    }
}
