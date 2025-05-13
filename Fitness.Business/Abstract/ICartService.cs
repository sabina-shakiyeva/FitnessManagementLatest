using Fitness.Entities.Concrete;
using Fitness.Entities.Models.CartItem;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fitness.Business.Abstract
{
    public interface ICartService
    {
        Task AddToCartAsync(int userId, int productId, int quantity);
        Task RemoveFromCartAsync(int userId, int productId);
        Task UpdateQuantityAsync(int userId, int productId, int newQuantity);

        Task<List<CartItemDto>> GetUserCartAsync(int userId);

        Task BuyAllFromCartAsync(int userId);
        Task<string> BuyNowAsync(string identityUserId, int productId, int quantity);
    }
}
