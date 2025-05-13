using Fitness.Business.Abstract;
using Fitness.Business.Concrete;
using Fitness.Entities.Models.Feedback;
using Microsoft.AspNet.SignalR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace FitnessManagement.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Trainer")] 
    public class TrainerFeedbackController : ControllerBase
    {
        private readonly ITrainerFeedbackService _feedbackService;

        public TrainerFeedbackController(ITrainerFeedbackService feedbackService)
        {
            _feedbackService = feedbackService;
        }
        //trainerin usere feedback vermesi
        [HttpPost]
        public async Task<IActionResult> AddFeedback([FromBody] TrainerToUserFeedbackCreateDto dto)
        {
            await _feedbackService.AddFeedbackAsync(dto); 
            return Ok();
        }
    }
}
