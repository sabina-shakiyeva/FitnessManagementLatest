using Fitness.Business.Abstract;
using Fitness.DataAccess.Abstract;
using Fitness.Entities.Concrete;
using Fitness.Entities.Models;
using Fitness.Entities.Models.Payment;
using Fitness.Entities.Models.PurchaseHistory;
using Fitness.Entities.Models.User;
using FitnessManagement.Dtos;
using FitnessManagement.Entities;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fitness.Business.Concrete
{

    public class PaymentService : IPaymentService
    {
        private readonly IPaymentDal _paymentDal;
        private readonly IUserDal _userDal;
        private readonly IPackageDal _packageDal;
        private readonly IPurchaseHistoryDal _purchaseHistoryDal;
        private readonly IPurchaseHistoryService _purchaseHistoryService;
        private readonly IBalanceTopUp _balanceTopUpDal;

        public PaymentService(IPaymentDal paymentDal, IUserDal userDal, IPackageDal packageDal, IPurchaseHistoryDal purchaseHistoryDal, IPurchaseHistoryService purchaseHistoryService, IBalanceTopUp balanceTopUpDal)
        {
            _paymentDal = paymentDal;
            _userDal = userDal;
            _packageDal = packageDal;
            _purchaseHistoryDal = purchaseHistoryDal;
            _purchaseHistoryService = purchaseHistoryService;
            _balanceTopUpDal = balanceTopUpDal;
        }



        public async Task<string> TopUpBalanceAsync(string identityUserId, BalanceDto balanceDto)
        {
            var user = await _userDal.Get(u => u.IdentityUserId == identityUserId);
            if (user == null)
                throw new Exception("User not found.");

            if (balanceDto.CardNumber.StartsWith("1111"))
                throw new Exception("Invalid card.");

            user.Balance = (user.Balance ?? 0) + balanceDto.Amount;
            await _userDal.Update(user);

            await _balanceTopUpDal.Add(new BalanceTopUp
            {
                UserId = user.Id,
                Amount = balanceDto.Amount,
                TopUpDate = DateTime.Now,
                Description = "Balance top-up via card"
            });

            return $"Balance successfully added. New Balance: {user.Balance} AZN";
        }


        public async Task<string> PurchasePackageAsync(string identityUserId, int packageId, PaymentDto paymentDto)
        {
            var user = await _userDal.Get(u => u.IdentityUserId == identityUserId);
            if (user == null)
                throw new Exception("User not found.");

            if (user.IsActive && user.PackageEndDate.HasValue && user.PackageEndDate.Value < DateTime.Now)
            {
                user.IsActive = false;
                user.PackageId = null;
                user.PackageStartDate = null;
                user.PackageEndDate = null;
                await _userDal.Update(user);
            }

            if (user.IsActive)
                return "User already has an active package.";

            var package = await _packageDal.Get(p => p.Id == packageId);
            if (package == null)
                throw new Exception("Package not found.");

            if (paymentDto.CardNumber.StartsWith("1111"))
                throw new Exception("Invalid card.");

            bool isMonthly = paymentDto.IsMonthlyPayment;
            decimal monthlyAmount = Math.Round(package.Price / package.DurationInMonths, 2);
            decimal totalAmount = package.Price;

            if (isMonthly && paymentDto.Amount < monthlyAmount)
            {
                return $"Initial monthly payment must be at least {monthlyAmount} AZN.";
            }
            if ((user.Balance ?? 0)< paymentDto.Amount)
            {
                throw new Exception($"Insufficient balance. Your balance is {user.Balance??0} AZN but required is {paymentDto.Amount} AZN.");
            }
            user.Balance ??= 0;
            user.Balance -= paymentDto.Amount;
            await _userDal.Update(user);

            await _paymentDal.Add(new Payment
            {
                UserId = user.Id,
                PackageId = packageId,
                Amount = paymentDto.Amount,
                PaymentDate = DateTime.Now
            });

            var baseDate = DateTime.Now;

            for (int i = 0; i < package.DurationInMonths; i++)
            {
                var isFirstMonth = i == 0;
                DateTime? paymentDate = isFirstMonth && !isMonthly
    ? DateTime.Now
    : (isFirstMonth && paymentDto.Amount >= monthlyAmount ? DateTime.Now : null);

                var paidAmount = isFirstMonth ? (isMonthly ? paymentDto.Amount : totalAmount) : 0;

                var isPaid = isFirstMonth ? (!isMonthly || paymentDto.Amount >= monthlyAmount) : false;

                await _purchaseHistoryService.AddPurchaseHistoryAsync(new PurchaseHistoryAddDto
                {
                    UserId = user.Id,
                    PackageId = packageId,
                    Amount = monthlyAmount,
                    PurchaseDate = baseDate.AddMonths(i),
                    PaymentDate = paymentDate,
                    IsPaid = isPaid,
                    IsActive = true,
                    IsMonthlyPayment = isMonthly,
                    PackageName = package.PackageName,
                    PaidAmount = paidAmount,
                    TotalAmount = isMonthly ? monthlyAmount : totalAmount
                });
            }


            user.PackageId = packageId;
            user.IsActive = true;
            user.PackageStartDate = baseDate;
            user.PackageEndDate = baseDate.AddMonths(package.DurationInMonths);
            await _userDal.Update(user);

            return "Package successfully purchased.";
        }


        public async Task<string> ProcessMonthlyPaymentAsync(string identityUserId, Payment2Dto paymentDto)
        {
            var user = await _userDal.Get(u => u.IdentityUserId == identityUserId);
            if (user == null || !user.IsActive)
                throw new Exception("User not active or not found.");

            var package = await _packageDal.Get(p => p.Id == paymentDto.PackageId);
            if (package == null || user.PackageId != package.Id)
                throw new Exception("Invalid package for payment.");

            decimal amountToPay = paymentDto.Amount;

            if ((user.Balance ?? 0)< amountToPay)
                throw new Exception($"Insufficient balance. Your balance is {user.Balance??0} AZN but required is {amountToPay} AZN.");

            user.Balance ??= 0;
            user.Balance -= amountToPay;
            await _userDal.Update(user);

            var unpaidList = (await _purchaseHistoryService.GetPurchaseHistoryByUserIdAsync(user.Id))
                .Where(p => p.PackageId == package.Id && p.IsMonthlyPayment && !p.IsPaid)
                .OrderBy(p => p.PurchaseDate)
                .ToList();

            foreach (var history in unpaidList)
            {
                if (amountToPay <= 0) break;
                decimal remaining = history.TotalAmount - history.PaidAmount;
                decimal payNow = Math.Min(remaining, amountToPay);

                history.PaidAmount += payNow;
                amountToPay -= payNow;

                if (history.PaidAmount >= history.TotalAmount)
                {
                    history.IsPaid = true;
                    history.PaymentDate = DateTime.Now;
                }

                await _purchaseHistoryService.UpdatePurchaseHistoryAsync(new PurchaseHistoryUpdateDto
                {
                    Id = history.Id,
                    Amount = history.Amount,
                    PurchaseDate = history.PurchaseDate,
                    PaymentDate = history.PaymentDate ?? DateTime.MinValue,
                    IsPaid = history.IsPaid,
                    IsActive = history.IsActive,
                    IsMonthlyPayment = history.IsMonthlyPayment,
                    PackageName = history.PackageName,
                    PaidAmount = history.PaidAmount,
                    TotalAmount = history.TotalAmount
                });

                await _paymentDal.Add(new Payment
                {
                    UserId = user.Id,
                    PackageId = package.Id,
                    Amount = payNow,
                    PaymentDate = DateTime.Now
                });
            }

            return "Payment processed.";
        }


        public async Task<List<DelayedUserDto>> GetDelayedBlockedUsersAsync()
        {
            var blockedUsers = await _userDal.GetList(u => u.IsBlocked == true);
            return blockedUsers.Select(u => new DelayedUserDto
            {
                UserId = u.Id,
                FullName = u.Name,
                Email = u.Email,
                BlockedDate = u.PackageEndDate
            }).ToList();
        }
        //userin odediyi odenisleri gore bileceyik
        public async Task<List<SimplePaymentDto>> GetUserPaymentsAsync(string identityUserId)
        {
            var user = await _userDal.Get(u => u.IdentityUserId == identityUserId);
            if (user == null)
                throw new Exception("User not found.");

            var payments = await _paymentDal.GetList(p => p.UserId == user.Id);

            return payments
                .OrderByDescending(p => p.PaymentDate)
                .Select(p => new SimplePaymentDto
                {
                    Amount = p.Amount,
                    PaymentDate = p.PaymentDate
                })
                .ToList();
        }

        public async Task CheckDelayedMonthlyPaymentsAsync()
        {
            var now = DateTime.Now;

            var delayed = await _purchaseHistoryDal.GetList(ph =>
                ph.IsMonthlyPayment &&
                !ph.IsPaid &&
                ph.PurchaseDate.AddDays(5) < now &&
                ph.PaidAmount < (ph.TotalAmount / ph.Package.DurationInMonths));

            foreach (var item in delayed)
            {
                var user = await _userDal.Get(u => u.Id == item.UserId);
                if (user != null)
                {
                    user.IsActive = false;
                    user.IsBlocked = true;
                    user.PackageId = null;
                    user.PackageStartDate = null;
                    user.PackageEndDate = null;
                    await _userDal.Update(user);
                }
            }
        }

        public async Task<List<AdminPaymentDto>> GetAllPaymentsForAdminAsync(string? search)
        {
            var payments = await _paymentDal.GetList();
            var users = await _userDal.GetList();
            var userDict = users.ToDictionary(u => u.Id, u => u.Name);

            var result = payments
                .OrderByDescending(p => p.PaymentDate)
                .Select(p => new AdminPaymentDto
                {
                    UserId = p.UserId,
                    UserName = userDict.ContainsKey(p.UserId) ? userDict[p.UserId] : "Unknown",
                    Amount = p.Amount,
                    PaymentDate = p.PaymentDate,
                    PackageId = p.PackageId,
                })
                .ToList();

            if (!string.IsNullOrEmpty(search))
            {
                search = search.ToLower();
                result = result
                    .Where(p => p.UserName.ToLower().Contains(search))
                    .ToList();
            }

            return result;
        }





    }

}



















