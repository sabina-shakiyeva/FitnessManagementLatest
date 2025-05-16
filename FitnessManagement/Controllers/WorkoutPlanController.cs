using Fitness.Business.Abstract;
using Fitness.Entities.Models.Workout;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace FitnessManagement.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class WorkoutPlanController : ControllerBase
    {
        private readonly IWorkoutPlanService _workoutPlanService;

        public WorkoutPlanController(IWorkoutPlanService workoutPlanService)
        {
            _workoutPlanService = workoutPlanService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] string? search)
        {
            var result = await _workoutPlanService.GetAllPlansAsync(search);
            return Ok(result);
        }
        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            var result = await _workoutPlanService.GetPlanByIdAsync(id);
            return Ok(result);
        }
        [HttpPost]
        public async Task<IActionResult> Add([FromBody] WorkoutPlanDto dto)
        {
            await _workoutPlanService.AddPlanAsync(dto);
            return Ok("Workout plan created successfully.");
        }
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] WorkoutPlanDto dto)
        {
            await _workoutPlanService.UpdatePlanAsync(id, dto);
            return Ok("Workout plan updated successfully.");
        }
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            await _workoutPlanService.DeletePlanAsync(id);
            return Ok("Workout plan deleted successfully.");
        }
        //USER terefde olacaq
        [HttpGet("by-package/{packageId}")]
        public async Task<IActionResult> GetPlanByPackageId(int packageId)
        {
            var plan = await _workoutPlanService.GetPlanByPackageIdAsync(packageId);
            return Ok(plan);
        }

    }
}
