using Fitness.Business.Abstract;
using Fitness.Business.Concrete;
using Fitness.Entities.Concrete;
using Fitness.Entities.Models;
using Fitness.Entities.Models.User;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace FitnessManagement.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    //[Authorize]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly UserManager<ApplicationUser> _userManager;

        public UserController(IUserService userService, UserManager<ApplicationUser> userManager)
        {
            _userService = userService;
            _userManager = userManager;
        }
        //dashboarda user kartlarini gormek ucun yazdim UserPackageInfoDto-dan istifade etdim
        [HttpGet("package-info")]
        public async Task<IActionResult> GetUserPackageInfo()
        {
            var identityUserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(identityUserId))
            {
                return Unauthorized("User identity not found");
            }

            var packageInfo = await _userService.GetUserPackageInfoAsync(identityUserId);

            if (packageInfo == null)
            {
                return NotFound("User package information not found");
            }

            return Ok(packageInfo);
        }
        //groupdaki userler coxdan aza dogru siralanacaq 
        [HttpGet("group-top-users")]
        public async Task<IActionResult> GetTopUsersByGroup()
        {
            var identityUserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(identityUserId))
                return Unauthorized();

            var topUsers = await _userService.GetTopUsersByGroupAsync(identityUserId);

            return Ok(topUsers);
        }


        [HttpPut("profile-update/{userId}")]
        public async Task<IActionResult> UpdateUserProfile(int userId, [FromForm] UserProfileUpdateDto dto)
        {
            try
            {
                await _userService.UpdateUserProfile(userId, dto);
                return Ok("User profile updated successfully.");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        //STATISTIKA 
        [HttpGet("summary-for-user")]
        public async Task<IActionResult> GetStatistics()
        {
            var result = await _userService.GetStatisticsForUser();
            return Ok(result);
        }

        [HttpGet("user-profile")]
		[Authorize(Roles = "User")]
		public async Task<IActionResult> GetMyUserProfile()
		{
			var identityUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
			var user = await _userService.GetUserProfile(identityUserId);
			return Ok(user);
		}

		[HttpGet("top-users")]
        public async Task<IActionResult> GetTopUsers()
        {
            var topUsers = await _userService.GetTop10UsersByPointsAsync();
            return Ok(topUsers);
        }
        //asagida yazdigim iki endpointde user-in cedvelinde package ve traineri gormesi ucun yazilib
        [HttpGet("details")]
        public async Task<IActionResult> GetAllUserPackageTrainer()
        {
            var result = await _userService.GetAllUserPackageTrainer();
            return Ok(result);
        }
        [HttpGet("{id}/details")]
        public async Task<IActionResult> GetUserPackageTrainer(int id)
        {
            try
            {
                var result = await _userService.GetUserPackageTrainer(id);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return NotFound(new { message = ex.Message });
            }
        }
        [HttpPut("{id}/details")]
        public async Task<IActionResult> UpdateUserPackageTrainer(int id, [FromForm] UserPackageTrainerUpdateDto dto)
        {
            try
            {
                await _userService.UpdateUserPackageTrainer(id, dto);
                return Ok(new { message = "User updated successfully" });
            }
            catch (Exception ex)
            {
                return NotFound(new { message = ex.Message });
            }
        }

        [HttpGet("balance")]
        public async Task<IActionResult> GetBalance()
        {
            var identityUserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(identityUserId))
                return Unauthorized();

            var balance = await _userService.GetBalanceAsync(identityUserId);
            return Ok(new { balance });
        }




    }
}
