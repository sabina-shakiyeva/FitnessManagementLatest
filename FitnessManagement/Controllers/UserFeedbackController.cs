using Fitness.Business.Abstract;
using Fitness.Business.Concrete;
using Microsoft.AspNet.SignalR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace FitnessManagement.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "User")] 
    public class UserFeedbackController : ControllerBase
    {
        private readonly IUserFeedbackService _userFeedbackService;

        public UserFeedbackController(IUserFeedbackService userFeedbackService)
        {
            _userFeedbackService = userFeedbackService;
        }

        // User-e verilmis feedback-leri göstermek
        [HttpGet]
        public async Task<IActionResult> GetMyFeedbacks()
        {
            var feedbacks = await _userFeedbackService.GetMyFeedbacksAsync(); 
            return Ok(feedbacks); 
        }
    }
}
