using Fitness.Business.Abstract;
using Fitness.Entities.Models.Puchase;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace FitnessManagement.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    //PRODUCT ALIS TARIXCESI
    public class PurchaseController : ControllerBase
    {
        private readonly IPurchaseService _purchaseService;

        public PurchaseController(IPurchaseService purchaseService)
        {
            _purchaseService = purchaseService;
        }
        //alinan produktlkarin siyahisini yeni kim alib ne vaxt alib bunlari admin gorur
        [HttpGet]
        public async Task<ActionResult<List<PurchaseProductDto>>> GetAll()
        {
            var purchases = await _purchaseService.GetAllPurchasesAsync();
            return Ok(purchases);
        }

        //buida alinan produktlarin silinmesiid
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _purchaseService.DeletePurchaseAsync(id);
            if (!result)
                return NotFound("Purchase not found.");

            return Ok("Purchase deleted successfully.");
        }
    }
}
