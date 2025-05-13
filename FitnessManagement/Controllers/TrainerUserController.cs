using Fitness.Business.Abstract;
using Fitness.DataAccess.Abstract;
using Fitness.Entities.Concrete;
using Fitness.Entities.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace FitnessManagement.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Trainer")]
    public class TrainerUserController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly ITrainerService _trainerService;
        private readonly IUserDal _userDal;
        private readonly UserManager<ApplicationUser> _userManager;

        public TrainerUserController(IUserService userService, ITrainerService trainerService, IUserDal userDal, UserManager<ApplicationUser> userManager)
        {
            _userService = userService;
            _trainerService = trainerService;
            _userDal = userDal;
            _userManager = userManager;
        }

        [HttpGet("my-users")]
        public async Task<IActionResult> GetMyUsers()
        {
            string identityTrainerId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var trainer = await _userManager.FindByIdAsync(identityTrainerId);
          
            if (trainer == null)
                return Unauthorized();

            var users = await _trainerService.GetUsersByTrainerId(trainer.Id);
            return Ok(users);
        }

        [HttpGet("user/{id}")]
        public async Task<IActionResult> GetUserById(int id)
        {
            string identityTrainerId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            try
            {
                var user = await _trainerService.GetUserByIdForTrainer(id, identityTrainerId);
                return Ok(user);
            }
            catch (Exception ex)
            {
                return NotFound(ex.Message);
            }
        }


        [HttpPut("{id}")]
       
        public async Task<IActionResult> UpdateUser(int id, UserUpdateDto dto)
        {
            string identityTrainerId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var trainer = await _trainerService.GetTrainerByIdentityId(identityTrainerId);
            if (trainer == null)
                return Unauthorized();

            var user = await _trainerService.GetUserByIdForTrainer(id, identityTrainerId);
            if (user == null || user.TrainerId != trainer.Id)
                return Forbid();

            await _trainerService.UpdateUserByTrainer(id, trainer.Id, dto);
            return NoContent();
        }



        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUser(int id)
        {
            string identityTrainerId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var trainer = await _trainerService.GetTrainerByIdentityId(identityTrainerId);

            var user = await _trainerService.GetUserByIdForTrainer(id, identityTrainerId);

            if (user == null || user.TrainerId != trainer.Id)
                return Forbid();

            await _trainerService.DeleteUserByTrainer(id,trainer.Id);
            return NoContent();
        }


    }
}
