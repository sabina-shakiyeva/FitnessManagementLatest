using Fitness.Business.Abstract;
using Fitness.DataAccess.Abstract;
using Fitness.Entities.Models.Feedback;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Fitness.Business.Concrete
{
    public class UserFeedbackService:IUserFeedbackService
    {
        private readonly IFeedbackDal _feedbackDal;
        private readonly IUserDal _userDal;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public UserFeedbackService(IFeedbackDal feedbackDal, IUserDal userDal, IHttpContextAccessor httpContextAccessor)
        {
            _feedbackDal = feedbackDal;
            _userDal = userDal;
            _httpContextAccessor = httpContextAccessor;
        }

        private async Task<int>  GetUserIdFromToken()
        {
            var identityId = _httpContextAccessor.HttpContext?.User?.FindFirstValue(ClaimTypes.NameIdentifier);
            var user = await _userDal.Get(u => u.IdentityUserId == identityId);
            return user?.Id ?? throw new Exception("User not found");
        }

        // User-in aldığı feedback-ləri göstərmək
        public async Task<List<UserReceivedFeedbackDto>> GetMyFeedbacksAsync()
        {
            var userId = await GetUserIdFromToken();

            var feedbacks = await _feedbackDal.GetAllWithIncludeAsync(
                f => f.UserId == userId,
                include: f => f.Include(x => x.Trainer)
            );

            return feedbacks.Select(f => new UserReceivedFeedbackDto
            {
                Id = f.Id,
                Comment = f.Comment,
                IsPositive = f.IsPositive,
                GivenAt = f.GivenAt,
                TrainerId = f.TrainerId,
                TrainerFullName = f.Trainer.Name
            }).ToList();
        }
    }
}
