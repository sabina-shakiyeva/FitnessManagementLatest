using Fitness.Business.Abstract;
using Fitness.Entities.Models.Notification;
using Microsoft.AspNet.SignalR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

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
        public async Task<IActionResult> Create([FromBody] GlobalNotificationDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.Message))
                return BadRequest("Mesaj boş ola bilməz.");

            await _globalNotificationService.CreateGlobalNotificationAsync(dto.Message);
            return Ok(new { success = true });
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var identityUserId = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;

            if (identityUserId == null)
                return Unauthorized("İstifadəçi identifikasiyası tapılmadı.");

            var result = await _globalNotificationService.GetAllGlobalNotificationsAsync(identityUserId);
            return Ok(result);
        }

        [Authorize]
        [HttpPut("mark-as-read/{id}")]
        public async Task<IActionResult> MarkAsRead(int id)
        {
            var identityUserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (identityUserId == null)
                return Unauthorized();

            await _globalNotificationService.MarkAsReadAsync(id, identityUserId);
            return NoContent();
        }


        //[HttpPut("mark-as-read/{id}")]
        //public async Task<IActionResult> MarkAsRead(int id)
        //{
        //    await _globalNotificationService.MarkAsReadAsync(id);
        //    return NoContent();
        //}
    }
}
