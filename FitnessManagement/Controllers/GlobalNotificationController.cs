using Fitness.Business.Abstract;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace FitnessManagement.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GlobalNotificationController : ControllerBase
    {
        private readonly IGlobalNotificationService _globalNotificationService;

        public GlobalNotificationController(IGlobalNotificationService globalNotificationService)
        {
            _globalNotificationService = globalNotificationService;
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] string message)
        {
            await _globalNotificationService.CreateGlobalNotificationAsync(message);
            return Ok(new { success = true });
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var result = await _globalNotificationService.GetAllGlobalNotificationsAsync();
            return Ok(result);
        }
    }
}
