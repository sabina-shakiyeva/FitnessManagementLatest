using Fitness.Entities.Concrete;
using Fitness.Entities.Models.Puchase;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fitness.Business.Abstract
{
    public interface IPurchaseService
    {
        Task RequestPurchaseAsync(int userId, int productId, int quantity);
        Task<List<PurchaseRequest>> GetPendingRequestsAsync();
        Task ApprovePurchaseAsync(int requestId);
        Task<List<PurchaseRequest>> GetAllRequestsAsync();
        Task RejectPurchaseAsync(int requestId);
        Task<List<PurchaseProductDto>> GetAllPurchasesAsync();
        Task<bool> DeletePurchaseAsync(int id);
    }
}
