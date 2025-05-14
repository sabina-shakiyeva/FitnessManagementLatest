using Fitness.Business.Abstract;
using Fitness.DataAccess.Abstract;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace FitnessManagement.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class NotificationController : ControllerBase
    {
        private readonly INotificationService _notificationService;
        private readonly IUserDal _userDal;

        public NotificationController(INotificationService notificationService, IUserDal userDal)
        {
            _notificationService = notificationService;
            _userDal = userDal;
        }
        //bu hissede user oz notification larini gorecek notification hissesinde
        [HttpGet("my")]
        public async Task<IActionResult> GetMyNotifications()
        {
            var identityId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var user = await _userDal.Get(u => u.IdentityUserId == identityId);
            if (user == null) return Unauthorized();

            var notifications = await _notificationService.GetUserNotificationsAsync(user.Id);
            return Ok(notifications);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            await _notificationService.DeleteNotificationAsync(id);
            return NoContent();
        }

        [HttpPut("mark-as-read/{id}")]
        public async Task<IActionResult> MarkAsRead(int id)
        {
            await _notificationService.MarkAsReadAsync(id);
            return NoContent();
        }
    }
}
