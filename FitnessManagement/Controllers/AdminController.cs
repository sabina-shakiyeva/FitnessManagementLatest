using Fitness.Business.Abstract;
using Fitness.Business.Concrete;
using Fitness.DataAccess.Abstract;
using Fitness.Entities.Concrete;
using Fitness.Entities.Models;
using FitnessManagement.Dtos;
using FitnessManagement.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Text;

namespace FitnessManagement.Controllers
{

    [Route("api/[controller]")]
    [ApiController]
    //[Authorize(Roles = "Admin")]
    public class AdminController : ControllerBase
    {
      
        private readonly UserManager<ApplicationUser> _userManager;

        private readonly IUserService _userService;
        private readonly ITrainerService _trainerService;
        private readonly IUserDal _userDal;
        private readonly IAdminService _adminService;



        public AdminController(UserManager<ApplicationUser> userManager, IUserService userService, IUserDal userDal, ITrainerService trainerService, IAdminService adminService)
        {
            _userManager = userManager;
            _userService = userService;
            _userDal = userDal;
            _trainerService = trainerService;
            _adminService = adminService;
        }
        [HttpGet("admin-profile")]
       
		public async Task<IActionResult> GetMyProfile()
		{
			var identityId = User.FindFirstValue(ClaimTypes.NameIdentifier);
			var admin = await _adminService.GetAdminByIdAsync(identityId);
			return Ok(admin);
		}


		[HttpPost("add-user")]
        public async Task<IActionResult> AddUser([FromForm] UserDto userDto)
        {
            if(userDto==null)
            {
                return BadRequest(new { Status = "Error", Message = "User data is required!" });
            }
            await _userService.AddUser(userDto);
            return Ok("User added successfully!");

        }
        [HttpPut("update-user/{userId}")]
        public async Task<IActionResult> UpdateUser([FromForm] UserUpdateDto userUpdateDto,int userId)
        {
            try
            {
                await _userService.UpdateUser(userId,userUpdateDto);
                return Ok(new { message = "User updated successfully." });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
        [HttpPut("update-trainer/{trainerId}")]
        public async Task<IActionResult> UpdateTrainer([FromForm] TrainerUpdateDto trainerUpdateDto, int trainerId)
        {
            try
            {
                await _trainerService.UpdateTrainer(trainerId,trainerUpdateDto);
                return Ok(new { message = "Trainer updated successfully." });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
        [HttpPut("update-admin/{adminId}")]
        public async Task<IActionResult> UpdateAdmin(int adminId, [FromForm] AdminUpdateDto adminUpdateDto)
        {
            try
            {
                await _adminService.UpdateAdminAsync(adminId, adminUpdateDto);
                return Ok(new { message = "Admin updated successfully" });
            }
            catch (Exception ex)
            {
              
                return BadRequest(new { message = ex.Message });
            }
        }


        [HttpDelete("delete-user/{id}")]
        public async Task<IActionResult> DeleteUser(int id)
        {
            try
            {
                await _userService.DeleteUser(id);
                return Ok(new { message = "User deleted successfully." });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpDelete("delete-trainer/{id}")]
        public async Task<IActionResult> DeleteTrainer(int id)
        {
            try
            {
                await _trainerService.DeleteTrainer(id);
                return Ok(new { message = "Trainer deleted successfully." });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
        [HttpGet("users")]
        public async Task<IActionResult> GetAllUsers()
        {
            var users = await _userService.GetAllUsers();
            return Ok(users);
        }
        [HttpGet("trainers")]
        public async Task<IActionResult> GetAllTrainers()
        {
            var trainers = await _trainerService.GetAllTrainers();
            return Ok(trainers);
        }
        [HttpGet("user/{id}")]
        public async Task<IActionResult> GetUserById(int id)
        {
            var user = await _userService.GetUserById(id);
            if (user == null)
            {
                return NotFound("Not Found");
            }
            return Ok(user);
        }
        [HttpGet("trainer/{id}")]
        public async Task<IActionResult> GetTrainerById(int id)
        {
            var trainer = await _trainerService.GetTrainerById(id);
            if (trainer == null)
            {
                return NotFound("Not Found");
            }
            return Ok(trainer);
        }
        [HttpGet("pending-users")]
        public async Task<IActionResult> GetPendingUsers()
        {
            var pendingUsers = await _userService.GetPendingUsers();

            //if (!pendingUsers.Any())
            //{
            //    return NotFound(new { Status = "Error", Message = "No pending users found!" });
            //}

            return Ok(pendingUsers.Select(u => new
            {
                u.Id,
                u.FullName,
                u.Email,
                u.UserName
            }));
           
        }

        [HttpGet("pending-trainers")]
        public async Task<IActionResult> GetPendingTrainers()
        {
            var pendingTrainers = await _trainerService.GetPendingTrainers();

            //if (!pendingTrainers.Any())
            //{
            //    return NotFound(new { Status = "Error", Message = "No pending trainers found!" });
            //}

            return Ok(pendingTrainers.Select(t => new
            {
                t.Id,
                t.FullName,
                t.Email,
                t.UserName
            }));
        }


        [HttpPost("approve-user/{userId}")]
        public async Task<IActionResult> ApproveUser(string userId)
        {
            try
            {
                await _userService.ApproveUser(userId);
                return Ok(new { Status = "Success", Message = "User approved successfully!" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { Status = "Error", Message = ex.Message });
            }
        }
        [HttpPost("decline-user/{userId}")]
        public async Task<IActionResult> DeclineUser(string userId)
        {
            try
            {
              
                await _userService.DeclineUser(userId);
                return Ok(new { Status = "Success", Message = "User declined and removed from pending users!" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { Status = "Error", Message = ex.Message });
            }
        }

        [HttpPost("decline-trainer/{trainerId}")]
        public async Task<IActionResult> DeclineTrainer(string trainerId)
        {
            try
            {

                await _trainerService.DeclineTrainer(trainerId);
                return Ok(new { Status = "Success", Message = "Trainer declined and removed from pending trainers!" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { Status = "Error", Message = ex.Message });
            }
        }

        

        [HttpPost("approve-trainer/{trainerId}")]
        public async Task<IActionResult> ApproveTrainer(string trainerId)
        {
            try
            {
                await _trainerService.ApproveTrainer(trainerId);
                return Ok(new { Status = "Success", Message = "Trainer approved successfully!" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { Status = "Error", Message = ex.Message });
            }
        }

      
        [HttpPost("add-trainer")]
        public async Task<IActionResult> AddTrainer([FromForm] TrainerDto trainerDto)
        {
            if (trainerDto == null)
            {
                return BadRequest(new { Status = "Error", Message = "Trainer data is required!" });
            }
            await _trainerService.AddTrainer(trainerDto);
            return Ok("Trainer added successfully!");

        }

        //STATISTIKA
        [HttpGet("summary")]
        public async Task<IActionResult> GetStatistics()
        {
            var result = await _adminService.GetStatisticsAsync();
            return Ok(result);
        }

    }
}
