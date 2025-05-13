using Fitness.Entities.Concrete;
using Fitness.Entities.Models.PurchaseHistory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fitness.Business.Abstract
{
    public interface IPurchaseHistoryService
    {
        Task AddPurchaseHistoryAsync(PurchaseHistoryAddDto dto);
        Task<List<PurchaseHistoryGetDto>> GetPurchaseHistoryByUserIdAsync(int userId);
        Task<List<PurchaseHistoryGetDto>> GetAllPurchaseHistoriesAsync();
        Task UpdatePurchaseHistoryAsync(PurchaseHistoryUpdateDto dto);
    }
}
