using Fitness.Business.Abstract;
using Fitness.DataAccess.Abstract;
using Fitness.Entities.Models.CartItem;
using Microsoft.AspNet.SignalR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace FitnessManagement.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    //User ucun
    public class CartItemController : ControllerBase
    {
        private readonly ICartService _cartService;
        private readonly IUserDal _userDal;

        public CartItemController(ICartService cartService, IUserDal userDal)
        {
            _cartService = cartService;
            _userDal = userDal;

        }
        [HttpPost("add-to-cart")]
        public async Task<IActionResult> AddToCart([FromBody] CartItemAddDto dto)
        {
            var userIdentityId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(userIdentityId))
                return Unauthorized("User identity not found");

            var user = await _userDal.Get(u => u.IdentityUserId == userIdentityId);

            if (user == null)
                return Unauthorized("User not found");

            await _cartService.AddToCartAsync(user.Id, dto.ProductId, dto.Quantity);
            return Ok(new { message = "Product added to cart!" });
        }

        [HttpDelete("remove-from-cart/{productId}")]
        public async Task<IActionResult> RemoveFromCart(int productId)
        {
            var userIdentityId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(userIdentityId))
            {
                return Unauthorized("User identity not found");
            }

            var user = await _userDal.Get(u => u.IdentityUserId == userIdentityId);

            if (user == null)
            {
                return Unauthorized("User not found");
            }

            await _cartService.RemoveFromCartAsync(user.Id, productId);
            return Ok(new { message = "Product removed from cart!" });
        }
        //+ VE - UCUN 
        [HttpPost("update-quantity")]
        public async Task<IActionResult> UpdateQuantity([FromBody] CartItemUpdateDto dto)
        {
            var userIdentityId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(userIdentityId))
                return Unauthorized("User identity not found");

            var user = await _userDal.Get(u => u.IdentityUserId == userIdentityId);

            if (user == null)
                return Unauthorized("User not found");

            if (dto.NewQuantity <= 0)
            {
                await _cartService.RemoveFromCartAsync(user.Id, dto.ProductId);
                return Ok(new { message = "Product removed from cart because quantity was 0." });
            }

            await _cartService.UpdateQuantityAsync(user.Id, dto.ProductId, dto.NewQuantity);
            return Ok(new { message = "Product quantity updated!" });
        }
        //SEBETI GORMEK UCUN
        [HttpGet("get-cart")]
        public async Task<IActionResult> GetCart()
        {
            var userIdentityId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;  

            if (string.IsNullOrEmpty(userIdentityId))
            {
                return Unauthorized("User identity not found");
            }


            var user = await _userDal.Get(u => u.IdentityUserId == userIdentityId);

            if (user == null)
            {
                return Unauthorized("User not found");
            }
            var cartItems = await _cartService.GetUserCartAsync(user.Id);
            return Ok(cartItems);
        }
        //bu ise pramoy alis ucundur
        [HttpPost("buy-now")]
        public async Task<IActionResult> BuyNow([FromBody] CartItemAddDto dto)
        {
            var userIdentityId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(userIdentityId))
                return Unauthorized("User identity not found");

            var message = await _cartService.BuyNowAsync(userIdentityId, dto.ProductId, dto.Quantity);

            return Ok(new { message });
        }
        //bu ise kartda olan umumi mehsullarin alisidir
        [HttpPost("buy-all")]
        public async Task<IActionResult> BuyAll()
        {
         
            var userIdentityId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;  // Bu halda, userIdentityId string olacaq

            if (string.IsNullOrEmpty(userIdentityId))
            {
                return Unauthorized("User identity not found");
            }

          
            var user = await _userDal.Get(u => u.IdentityUserId == userIdentityId);

            if (user == null)
            {
                return Unauthorized("User not found");
            }

          
            await _cartService.BuyAllFromCartAsync(user.Id);

            return Ok("Purchase requests created and cart cleared.");
        }


    }
}
