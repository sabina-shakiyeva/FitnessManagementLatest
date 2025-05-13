using Fitness.Business.Abstract;
using Fitness.Business.Concrete;
using Fitness.DataAccess.Abstract;
using Fitness.Entities.Models.PurchaseHistory;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace FitnessManagement.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    //Payment history
    public class PurchaseHistoryController : ControllerBase
    {
        private readonly IPurchaseHistoryService _purchaseHistoryService;
        private readonly IUserService _userService;
        private readonly IPaymentService _paymentService;
        private readonly IUserDal _userDal;
        public PurchaseHistoryController(IPurchaseHistoryService purchaseHistoryService, IUserService userService, IPaymentService paymentService,IUserDal userDal)
        {
            _purchaseHistoryService = purchaseHistoryService;
            _userService = userService;
            _paymentService = paymentService;
            _userDal = userDal;
        }
        //bu lazim deyil
        [HttpGet("my-history")]
        [Authorize]
      
        public async Task<IActionResult> GetMyPurchaseHistory()
        {
            var identityUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrEmpty(identityUserId))
                return Unauthorized();

            var user = await _userService.GetByIdentityUserIdAsync(identityUserId);

            if (user == null)
                return NotFound("User not found");

            var history = await _purchaseHistoryService.GetPurchaseHistoryByUserIdAsync(user.Id);

            return Ok(history);
        }
        //kimler ne zaman odenisi edib admin gorsun onlari Admin
        [HttpGet("all")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetAllPurchaseHistories()
        {
            var history = await _purchaseHistoryService.GetAllPurchaseHistoriesAsync();
            return Ok(history);
        }
        //buda lazim deyil
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> AddPurchaseHistory([FromBody] PurchaseHistoryAddDto dto)
        {
            await _purchaseHistoryService.AddPurchaseHistoryAsync(dto);
            return Ok("Purchase history added successfully.");
        }
        //bu ise my statdaki kimi ayligi cedvel kimi bolur odenmeyenleri
        [HttpGet("my-payment-status")]
        [Authorize]
        public async Task<IActionResult> GetMyPaymentStatus()
        {
            var identityUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(identityUserId))
                return Unauthorized();

            var user = await _userDal.Get(u => u.IdentityUserId == identityUserId);
            if (user == null)
                return NotFound("User not found.");

            var unpaidPayments = (await _purchaseHistoryService.GetPurchaseHistoryByUserIdAsync(user.Id))
                                    .Where(p => p.IsMonthlyPayment && !p.IsPaid)
                                    .OrderBy(p => p.PurchaseDate)
                                    .Select(p => new
                                    {
                                        PurchaseDate = p.PurchaseDate.ToString("yyyy-MM-dd"),
                                        TotalAmount = p.TotalAmount,
                                        PaidAmount = p.PaidAmount,
                                        //RemainingAmount = p.TotalAmount - p.PaidAmount
                                        RemainingAmount = Math.Round(p.TotalAmount - p.PaidAmount, 2)
                                    })
                                    .ToList();

            return Ok(unpaidPayments);
        }

        //[HttpGet("my-payment-status")]
        //[Authorize]
        //public async Task<IActionResult> GetMyPaymentStatus()
        //{
        //    var identityUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        //    if (string.IsNullOrEmpty(identityUserId))
        //        return Unauthorized();

        //    var user = await _userDal.Get(u => u.IdentityUserId == identityUserId);
        //    if (user == null)
        //        return NotFound("User not found.");

        //    var unpaidPayments = (await _purchaseHistoryService.GetPurchaseHistoryByUserIdAsync(user.Id))
        //                            .Where(p => p.IsMonthlyPayment && !p.IsPaid)
        //                            .OrderBy(p => p.PurchaseDate)
        //                            .Select(p => new
        //                            {
        //                                p.PurchaseDate,
        //                                p.Amount
        //                            })
        //                            .ToList();

        //    return Ok(unpaidPayments);
        //}


        //[HttpGet("my-payment-status")]
        //[Authorize]
        //public async Task<IActionResult> GetMyPaymentStatus()
        //{
        //    var identityUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        //    if (string.IsNullOrEmpty(identityUserId))
        //        return Unauthorized();

        //    var user = await _userDal.Get(u => u.IdentityUserId == identityUserId);
        //    if (user == null)
        //        return NotFound("User not found.");

        //    var purchaseHistory = await _purchaseHistoryService.GetPurchaseHistoryByUserIdAsync(user.Id);

        //    // Burada ödənişləri siyahıya əlavə etmək
        //    var remainingPayments = purchaseHistory.Where(p => p.IsPaid == false && p.IsMonthlyPayment == true).ToList();

        //    return Ok(remainingPayments);
        //}



    }
}
