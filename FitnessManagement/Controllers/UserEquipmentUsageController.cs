using Fitness.Business.Abstract;
using Fitness.Entities.Concrete;
using Fitness.Entities.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace FitnessManagement.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class UserEquipmentUsageController : ControllerBase
    {
        private readonly IUserEquipmentUsageService _usageService;

        public UserEquipmentUsageController(IUserEquipmentUsageService usageService)
        {
            _usageService = usageService;
        }
        [HttpPost]
        public async Task<IActionResult> AddUsage([FromBody] EquipmentUsageStatDto dto)
        {
            if (dto == null)
                return BadRequest("Usage info is required.");

            await _usageService.AddAsync(dto);

            return Ok("Usage data saved and points updated.");
        }

        [HttpGet("user/{userId}")]
        public async Task<IActionResult> GetUserUsageHistory(int userId)
        {
            var result = await _usageService.GetAllByUserIdAsync(userId);
            return Ok(result);
        }

        [HttpGet("stats/user/{userId}")]
        public async Task<IActionResult> GetUserUsageStats(int userId)
        {
            var stats = await _usageService.GetUsageStatsByUserIdAsync(userId);
            return Ok(stats);
        }
        [HttpGet("all-usages")]
        public async Task<IActionResult> GetAllUsages()
        {
            var result = await _usageService.GetAllEquipmentUsages();
            return Ok(result);
        }

    }
}
