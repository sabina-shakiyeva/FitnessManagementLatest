using Fitness.Business.Abstract;
using Fitness.DataAccess.Abstract;
using Fitness.Entities.Concrete;
using Fitness.Entities.Models.Puchase;
using FitnessManagement.Entities;
using FitnessManagement.Services;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fitness.Business.Concrete
{
    public class PurchaseService:IPurchaseService
    {
        private readonly IPurchaseRequestDal _purchaseDal;
        private readonly IUserDal _userDal;
        private readonly IProductDal _productDal;
        private readonly IFileService _fileService;

        public PurchaseService(IPurchaseRequestDal purchaseDal, IUserDal userDal, IProductDal productDal, IFileService fileService)
        {
            _purchaseDal = purchaseDal;
            _userDal = userDal;
            _productDal = productDal;
            _fileService = fileService;
        }
        public async Task<List<PurchaseProductDto>> GetAllPurchasesAsync()
        {
            var purchases = await _purchaseDal.GetList(
             filter: null,
             include: query => query
                 .Include(p => p.Product)
                 .Include(p => p.User)
         );
            var result = purchases.Select(p => new PurchaseProductDto
            {
                Id = p.Id,
                UserId = p.UserId,
                ProductId = p.ProductId,
                Quantity = p.Quantity,
                ProductName = p.Product?.Name,
                UserName = p.User?.Name,
                Price = (p.Product?.PointCost ?? 0) * p.Quantity,
                RequestedAt = p.RequestedAt,
                UserPoint = p.User?.Point ?? 0,
                userImageUrl = p.User?.ImageUrl != null ? _fileService.GetFileUrl(p.User.ImageUrl) : null,
                productImageUrl = p.Product?.ImageUrl != null ? _fileService.GetFileUrl(p.Product.ImageUrl) : null
            }).ToList();

            return result;
        }
        public async Task<bool> DeletePurchaseAsync(int id)
        {
            var purchase = await _purchaseDal.Get(p => p.Id == id);
            if (purchase == null)
                return false;

            await _purchaseDal.Delete(purchase);
            return true;
        }
        public async Task ApprovePurchaseAsync(int requestId)
        {
            var request = await _purchaseDal.Get(r => r.Id == requestId);
            if (request == null) throw new Exception("Request not found.");

            if (request.IsApproved == true)
                throw new Exception("Request already approved.");

            var user = await _userDal.Get(u => u.Id == request.UserId);
            var product = await _productDal.Get(p => p.Id == request.ProductId);
            if (user == null || product == null) throw new Exception("User or Product not found.");

            var totalCost = product.PointCost * request.Quantity;

            if (user.Point < totalCost)
                throw new Exception("User has insufficient points.");

            if (product.Stock < request.Quantity)
                throw new Exception("Not enough product stock.");

            user.Point -= totalCost;
            product.Stock -= request.Quantity;
            request.IsApproved = true;
            request.ApprovedAt = DateTime.UtcNow;

            await _userDal.Update(user);
            await _productDal.Update(product);
            await _purchaseDal.Update(request);
        }

        public async Task RejectPurchaseAsync(int requestId)
        {
            var request = await _purchaseDal.Get(r => r.Id == requestId);
            if (request == null) throw new Exception("Request not found.");

            if (request.IsApproved == true)
                throw new Exception("Cannot reject an already approved request.");

            await _purchaseDal.Delete(request);
        }
        public async Task<List<PurchaseRequest>> GetPendingRequestsAsync()
        {
            return await _purchaseDal.GetList(r => !r.IsApproved);
        }
        public async Task<List<PurchaseRequest>> GetAllRequestsAsync()
        {
            return await _purchaseDal.GetList();
        }

        public async Task RequestPurchaseAsync(int userId, int productId, int quantity)
        {
            var product = await _productDal.Get(p => p.Id == productId);
            if (product == null || product.Stock < quantity)
                throw new Exception("Product not available in desired quantity.");

            await _purchaseDal.Add(new PurchaseRequest
            {
                UserId = userId,
                ProductId = productId,
                Quantity = quantity,
                RequestedAt = DateTime.UtcNow,
                IsApproved = false
            });
        }
    }
}
