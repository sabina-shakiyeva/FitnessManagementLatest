using Fitness.Business.Abstract;
using Fitness.DataAccess.Abstract;
using Fitness.Entities.Concrete;
using Fitness.Entities.Models.PurchaseHistory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fitness.Business.Concrete
{
    public class PurchaseHistoryService: IPurchaseHistoryService
    {
        private readonly IPurchaseHistoryDal _purchaseHistoryDal;
        public PurchaseHistoryService(IPurchaseHistoryDal purchaseHistoryDal)
        {
            _purchaseHistoryDal = purchaseHistoryDal;
        }
        public async Task UpdatePurchaseHistoryAsync(PurchaseHistoryUpdateDto dto)
        {
            var purchaseHistory = await _purchaseHistoryDal.Get(p => p.Id == dto.Id);
            if (purchaseHistory == null)
                throw new Exception("Purchase history not found.");

            purchaseHistory.Amount = dto.Amount;
            purchaseHistory.PurchaseDate = dto.PurchaseDate;
            purchaseHistory.PaymentDate = dto.PaymentDate;
            purchaseHistory.IsPaid = dto.IsPaid;
            purchaseHistory.IsActive = dto.IsActive;
            purchaseHistory.IsMonthlyPayment = dto.IsMonthlyPayment;
            purchaseHistory.PackageName = dto.PackageName;
            purchaseHistory.PaidAmount = dto.PaidAmount;
            purchaseHistory.TotalAmount = dto.TotalAmount;


            await _purchaseHistoryDal.Update(purchaseHistory);
        }

        public async Task AddPurchaseHistoryAsync(PurchaseHistoryAddDto dto)
        {
            var purchaseHistory = new PurchaseHistory
            {
                UserId = dto.UserId,
                PackageId = dto.PackageId,
                Amount = dto.Amount,
                PurchaseDate = dto.PurchaseDate,
                PaymentDate = dto.PaymentDate,
                IsPaid = dto.IsPaid,
                IsActive = dto.IsActive,
                IsMonthlyPayment = dto.IsMonthlyPayment,
                PackageName = dto.PackageName,
                PaidAmount = dto.PaidAmount,
                TotalAmount = dto.TotalAmount

            };

            await _purchaseHistoryDal.Add(purchaseHistory);
        }

        public async Task<List<PurchaseHistoryGetDto>> GetPurchaseHistoryByUserIdAsync(int userId)
        {
            var list = await _purchaseHistoryDal.GetList(p => p.UserId == userId);

            return list.Select(p => new PurchaseHistoryGetDto
            {
                Id = p.Id,
                UserId = p.UserId,
                PackageId = p.PackageId,
                Amount = p.Amount,
                PurchaseDate = p.PurchaseDate,
                PaymentDate = p.PaymentDate,
                IsPaid = p.IsPaid,
                IsActive = p.IsActive,
                IsMonthlyPayment = p.IsMonthlyPayment,
                PackageName = p.PackageName,
                PaidAmount = p.PaidAmount,
                TotalAmount = p.TotalAmount

            }).ToList();
        }
        public async Task<List<PurchaseHistoryGetDto>> GetAllPurchaseHistoriesAsync()
        {
            var list = await _purchaseHistoryDal.GetList();

            return list.Select(p => new PurchaseHistoryGetDto
            {
                Id = p.Id,
                UserId = p.UserId,
                PackageId = p.PackageId,
                Amount = p.Amount,
                PurchaseDate = p.PurchaseDate,
                PaymentDate = p.PaymentDate,
                IsPaid = p.IsPaid,
                IsActive = p.IsActive,
                IsMonthlyPayment = p.IsMonthlyPayment,
                PackageName = p.PackageName
            }).ToList();
        }

   
    }
  
}
