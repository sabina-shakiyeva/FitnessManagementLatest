using Fitness.Business.Abstract;
using Fitness.Entities.Models.Trainer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace FitnessManagement.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class TrainerScheduleController : ControllerBase
    {
        private readonly ITrainerScheduleService _trainerScheduleService;

        public TrainerScheduleController(ITrainerScheduleService trainerScheduleService)
        {
            _trainerScheduleService = trainerScheduleService;
        }
        //admin-de asagidakilar olacaq

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] TrainerScheduleCreateDto dto)
        {
            try
            {

                var result = await _trainerScheduleService.CreateScheduleAsync(dto);
                return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var schedules = await _trainerScheduleService.GetAllAsync();
            return Ok(schedules);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var schedule = await _trainerScheduleService.GetByIdAsync(id);
            if (schedule == null)
                return NotFound();
            return Ok(schedule);
        }

        [HttpGet("user/{userId}")]
        public async Task<IActionResult> GetByUserId(int userId)
        {
            var list = await _trainerScheduleService.GetByUserIdAsync(userId);
            return Ok(list);
        }

        [HttpGet("group/{groupId}")]
        public async Task<IActionResult> GetByGroupId(int groupId)
        {
            var list = await _trainerScheduleService.GetByGroupIdAsync(groupId);
            return Ok(list);
        }
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _trainerScheduleService.DeleteAsync(id);
            if (!result)
                return NotFound();
            return NoContent(); // 204
        }

        [HttpPut]
        public async Task<IActionResult> Update([FromBody] TrainerScheduleUpdateDto dto)
        {
            var updated = await _trainerScheduleService.UpdateAsync(dto);
            if (updated == null)
                return NotFound();
            return Ok(updated);
        }
        //Bu ise trainerin oz cedvelini gormesi ucundu
        [Authorize(Roles = "Trainer")]
        [HttpGet("myschedules")]
        public async Task<IActionResult> GetMySchedules()
        {
            var identityTrainerId = User.FindFirstValue(ClaimTypes.NameIdentifier); 

            var schedules = await _trainerScheduleService.GetSchedulesByTrainerIdentityIdAsync(identityTrainerId);
            return Ok(schedules);
        }

     


    }
}
