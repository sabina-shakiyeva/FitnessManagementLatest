using Fitness.Business.Abstract;
using Fitness.DataAccess.Abstract;
using Fitness.Entities.Models;
using Fitness.Entities.Models.Payment;
using FitnessManagement.Dtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace FitnessManagement.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    //[Authorize]
    public class PaymentController : ControllerBase
    {
        private readonly IPaymentService _paymentService;
        private readonly IUserService _userService;

        public PaymentController(IPaymentService paymentService, IUserService userService)
        {
            _paymentService = paymentService;
            _userService = userService;


        }
        //ayliq odenisi etmekm isteyenler ucun form
        [HttpPost("monthly-payment")]
        [Authorize]
        public async Task<IActionResult> ProcessMonthlyPayment([FromBody] Payment2Dto paymentDto)
        {
            var identityUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrEmpty(identityUserId))
                return Unauthorized();

            try
            {
                var result = await _paymentService.ProcessMonthlyPaymentAsync(identityUserId, paymentDto);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        //ilk paket alinanda olur 
        [HttpPost("buy-package")]
        public async Task<IActionResult> BuyPackage([FromQuery] int packageId, [FromBody] PaymentDto paymentDto)
        {
            try
            {
                var userIdentityId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

                if (string.IsNullOrEmpty(userIdentityId))
                    return Unauthorized("User identity not found");

                var result = await _paymentService.PurchasePackageAsync(userIdentityId, packageId, paymentDto);
                return Ok(new { message = result });
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }
        //background service-e aid
        [HttpPost("check-delayed-payments")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> CheckDelayedPayments()
        {
            await _paymentService.CheckDelayedMonthlyPaymentsAsync();
            return Ok(new { message = "Delayed payments checked and updated." });
        }
        //admin terefde bloklanan userlerin getirilmesi
        [HttpGet("delayed-users")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetDelayedUsers()
        {
            var users = await _paymentService.GetDelayedBlockedUsersAsync();
            return Ok(users);
        }

        //bunu yazma
        [HttpGet("payments")]
        public async Task<IActionResult> GetAllUserPackageTrainer()
        {
            var result = await _userService.GetPayments();
            return Ok(result);
        }

        //bu userin odenis tarixcesidi yeni ne zaman hansi odenisleri edib onlari gorsun
        [HttpGet("user-payments")]
        public async Task<IActionResult> GetUserPayments()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var payments = await _paymentService.GetUserPaymentsAsync(userId);
            return Ok(payments);
        }

        //user-in balansinin artirilmasi
        [HttpPost("top-up")]
        public async Task<IActionResult> TopUpBalance([FromBody] BalanceDto balanceDto)
        {
            var identityUserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(identityUserId))
                return Unauthorized();

            var result = await _paymentService.TopUpBalanceAsync(identityUserId, balanceDto);
            return Ok(new { message = result });
        }
        //burada admin kim neyi odeyibse gore bilecek

        [HttpGet("admin/all")]
        public async Task<IActionResult> GetAllPaymentsForAdmin([FromQuery] string? search)
        {
            var result = await _paymentService.GetAllPaymentsForAdminAsync(search);
            return Ok(result);
        }
    }
}

