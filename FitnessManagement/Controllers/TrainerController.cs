using Fitness.Business.Abstract;
using Fitness.Business.Concrete;
using Fitness.Entities.Models;
using Fitness.Entities.Models.Trainer;
using Fitness.Entities.Models.User;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace FitnessManagement.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    //[Authorize]
    public class TrainerController : ControllerBase
    {
        private readonly ITrainerService _trainerService;
        private readonly ITrainerScheduleService _trainerScheduleService;

        public TrainerController(ITrainerService trainerService, ITrainerScheduleService trainerScheduleService)
        {
            _trainerService = trainerService;
            _trainerScheduleService = trainerScheduleService;
        }

		[HttpGet("trainer-profile")]
		[Authorize(Roles = "Trainer")]
		public async Task<IActionResult> GetMyTrainerProfile()
		{
			var identityId = User.FindFirstValue(ClaimTypes.NameIdentifier);
			var trainer = await _trainerService.GetTrainerProfile(identityId);
			return Ok(trainer);
		}
        [HttpPut("profile-update/{trainerId}")]
        public async Task<IActionResult> UpdateUserProfile(int trainerId, [FromForm] TrainerUpdateProfileDto dto)
        {
            try
            {
                await _trainerService.UpdateTrainerProfile(trainerId, dto);
                return Ok("Trainer profile updated successfully.");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }


        //QEYD:swaggerde test etme umumen trainer meselelerinde authorize mutleq qalmalidi cunki giris eden trainere gore userler get ve post oluna bilir postmandan test et

        [HttpGet("statistics")]
        [Authorize(Roles = "Trainer")]
        public async Task<IActionResult> GetStatistics()
        {
            // IdentityTrainerId-ni JWT token-dən çıxarırıq
            var trainerIdentityId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrEmpty(trainerIdentityId))
                return Unauthorized("Trainer identity not found");

            var stats = await _trainerService.GetTrainerStatisticsAsync(trainerIdentityId);
            return Ok(stats);
        }
        //attendanceleri get methodunu burada yazdim
        [HttpGet("attendance")]
        [Authorize(Roles = "Trainer")]
        public async Task<IActionResult> GetMyUserAttendance()
        {
            var trainerIdentityId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrEmpty(trainerIdentityId))
                return Unauthorized("Trainer identity not found");

            var attendanceList = await _trainerService.GetTrainerAttendanceListAsync(trainerIdentityId);
            return Ok(attendanceList);
        }
        //buda ele evvel yazdigimiz kimidi take attendance 
        [HttpPost("take-attendance")]
        [Authorize(Roles = "Trainer")]
        public async Task<IActionResult> TakeAttendance([FromBody] TakeAttendanceDto dto)
        {
            var trainerIdentityId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrEmpty(trainerIdentityId))
                return Unauthorized("Trainer identity not found");

            await _trainerService.TakeAttendanceByTrainerAsync(trainerIdentityId, dto);

            return Ok("Attendance recorded successfully.");
        }

        [Authorize(Roles = "Trainer")]
        //trainerin sirf oz ders kecdiyi grouplari gostermsi ucundu
        [HttpGet("groups-by-trainer")]
        public async Task<IActionResult> GetGroupsByTrainer()
        {
            var identityTrainerId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var result = await _trainerScheduleService.GetGroupsByTrainerIdentityIdAsync(identityTrainerId);
            return Ok(result);
        }
        [Authorize(Roles = "Trainer")]
        //trainerin ders kecdiyi groupdaki userleri gosterir
        [HttpGet("users-in-group")]
        public async Task<IActionResult> GetUsersInGroup([FromQuery] int groupId)
        {
            try
            {
                var identityTrainerId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                var result = await _trainerScheduleService.GetUsersInTrainerGroupAsync(identityTrainerId, groupId);
                return Ok(result);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }



    }
}
