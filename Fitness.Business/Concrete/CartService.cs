using Fitness.Business.Abstract;
using Fitness.DataAccess.Abstract;
using Fitness.Entities.Concrete;
using Fitness.Entities.Models.CartItem;
using FitnessManagement.Entities;
using FitnessManagement.Services;
using Microsoft.EntityFrameworkCore;

public class CartService : ICartService
{
    private readonly ICartItemDal _cartItemDal;
    private readonly IProductDal _productDal;
    private readonly IUserDal _userDal;
    private readonly IPurchaseRequestDal _purchaseRequestDal;
    private readonly IFileService _fileService;

    public CartService(ICartItemDal cartItemDal, IProductDal productDal, IUserDal userDal, IPurchaseRequestDal purchaseRequestDal, IFileService fileService)
    {
        _cartItemDal = cartItemDal;
        _productDal = productDal;
        _userDal = userDal;
        _purchaseRequestDal = purchaseRequestDal;
        _fileService = fileService;

    }

    public async Task BuyAllFromCartAsync(int userId)
    {
      
        var cartItems = await _cartItemDal.GetList(
     ci => ci.UserId == userId,
     query => query.Include(ci => ci.Product)
                   .Include(ci => ci.User)
 );

        var user = cartItems.FirstOrDefault()?.User;
        if (user == null)
            throw new Exception("User not found");

        int totalPointCost = 0;

        foreach (var item in cartItems)
        {
            var product = item.Product;
            if (product == null) continue;

            if (product.Stock < item.Quantity)
                throw new Exception($"'{product.Name}' məhsulundan stokda kifayət qədər yoxdur.");
            totalPointCost += item.Quantity * product.PointCost;

            //await _purchaseRequestDal.Add(new PurchaseRequest
            //{
            //    UserId = userId,
            //    ProductId = item.ProductId,
            //    Quantity = item.Quantity,
            //    IsApproved = false,
            //    RequestedAt = DateTime.Now
            //});
        }

        if (totalPointCost > user.Point)
            throw new Exception("Not enough points for all items in cart");
        user.Point -= totalPointCost;
        await _userDal.Update(user);



        foreach (var item in cartItems)
        {
            var product = item.Product!;
            product.Stock -= item.Quantity;
            await _productDal.Update(product);

            await _purchaseRequestDal.Add(new PurchaseRequest
            {
                UserId = userId,
                ProductId = item.ProductId,
                Quantity = item.Quantity,
                IsApproved = true,
                RequestedAt = DateTime.Now
            });

            await _cartItemDal.Delete(item);
        }
    }

    public async Task UpdateQuantityAsync(int userId, int productId, int newQuantity)
    {
        var item = await _cartItemDal.Get(
            ci => ci.UserId == userId && ci.ProductId == productId,
            query => query.Include(ci => ci.Product).Include(ci => ci.User) 
        );

        if (item == null)
            throw new Exception("Cart item not found");

        if (item.Product == null)
            throw new Exception("Product not found");

        if (item.User == null)
            throw new Exception("User not found");

        int totalPointCost = newQuantity * item.Product.PointCost;

        if (totalPointCost > item.User.Point)
            throw new Exception("Not enough points");

        item.Quantity = newQuantity;
        await _cartItemDal.Update(item);
    }


    public async Task AddToCartAsync(int userId, int productId, int quantity)
    {
        var product = await _productDal.Get(p => p.Id == productId);
        if (product == null)
        {
            throw new Exception("Product not found");
        }
        var item = await _cartItemDal.Get(ci => ci.UserId == userId && ci.ProductId == productId);
        if (item != null)
        {
            item.Quantity += quantity;
            await _cartItemDal.Update(item);
        }
        else
        {
            await _cartItemDal.Add(new CartItem
            {
                UserId = userId,
                ProductId = productId,
                Quantity = quantity
            });
        }
    }

    public async Task<List<CartItemDto>> GetUserCartAsync(int userId)
    {
        var cartItems = await _cartItemDal.GetList(
            ci => ci.UserId == userId,
            query => query.Include(ci => ci.Product)
        );

        return cartItems
      .Where(ci => ci.Product != null) 
      .Select(ci => new CartItemDto
      {
          ProductId = ci.ProductId,
          ProductName = ci.Product.Name,
          Quantity = ci.Quantity,
          PointCost = ci.Product.PointCost,
          ImageUrl = !string.IsNullOrEmpty(ci.Product.ImageUrl)
              ? _fileService.GetFileUrl(ci.Product.ImageUrl)
              : null,
      })
      .ToList();

    }

    public async Task<string> BuyNowAsync(string identityUserId,int productId,int quantity)
    {
        var user=await _userDal.Get(u => u.IdentityUserId == identityUserId);
        var product = await _productDal.Get(p => p.Id == productId);
        if (product == null || user==null)
        {
            throw new Exception("Product or User not found");

        }
        int totalPointCost = quantity * product.PointCost;
        if (user.Point < totalPointCost)
            return "Coininiz yetmir.";

        if (product.Stock < quantity)
            return "Stokda kifayət qədər məhsul yoxdur.";
        user.Point -= totalPointCost;
        await _userDal.Update(user);

        product.Stock -= quantity;
        await _productDal.Update(product);
        await _purchaseRequestDal.Add(new PurchaseRequest
        {
            UserId = user.Id,
            ProductId = productId,
            Quantity = quantity,
            IsApproved = true,
            RequestedAt = DateTime.Now
        });
        return "Productunuz hazırdır, gəlib götürə bilərsiniz.";
    }

    public async Task RemoveFromCartAsync(int userId, int productId)
    {
        var item = await _cartItemDal.Get(ci => ci.UserId == userId && ci.ProductId == productId);
        if (item != null)
        {
            await _cartItemDal.Delete(item);
        }
    }




}
