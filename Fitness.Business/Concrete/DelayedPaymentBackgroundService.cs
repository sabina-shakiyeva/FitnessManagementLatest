using Fitness.Business.Abstract;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fitness.Business.Concrete
{
    public class DelayedPaymentBackgroundService: BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;
        public DelayedPaymentBackgroundService(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                var now = DateTime.Now;
                var nextRun = DateTime.Today.AddDays(1).AddHours(2); 
                var delay = nextRun - now;

                if (delay.TotalMilliseconds < 0) 
                {
                    nextRun = nextRun.AddDays(1);
                    delay = nextRun - now;
                }

                await Task.Delay(delay, stoppingToken);

                using (var scope = _serviceProvider.CreateScope())
                {
                    var paymentService = scope.ServiceProvider.GetRequiredService<IPaymentService>();

                    try
                    {
                        await paymentService.CheckDelayedMonthlyPaymentsAsync();
                        Console.WriteLine($"[BackgroundService] Delayed payments checked at {DateTime.Now}");
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"[BackgroundService] Error: {ex.Message}");
                    }
                }
            }
        
    }
    }
}
