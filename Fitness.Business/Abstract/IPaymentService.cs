using Fitness.Entities.Models;
using Fitness.Entities.Models.Payment;
using Fitness.Entities.Models.User;
using FitnessManagement.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fitness.Business.Abstract
{
    public interface IPaymentService
    {
        Task<string> PurchasePackageAsync(string identityUserId, int packageId, PaymentDto paymentDto);
        Task CheckDelayedMonthlyPaymentsAsync();

        Task<string> ProcessMonthlyPaymentAsync(string identityUserId, Payment2Dto paymentDto);
        Task<List<DelayedUserDto>> GetDelayedBlockedUsersAsync();
        Task<List<SimplePaymentDto>> GetUserPaymentsAsync(string identityUserId);
        Task<string> TopUpBalanceAsync(string identityUserId, BalanceDto balanceDto);
        Task<List<AdminPaymentDto>> GetAllPaymentsForAdminAsync(string? search);

    }
}
