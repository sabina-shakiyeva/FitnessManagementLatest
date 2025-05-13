using Fitness.Business.Abstract;
using Fitness.Business.Hubs;
using Fitness.DataAccess.Abstract;
using Fitness.DataAccess.Concrete.EfEntityFramework;
using Fitness.Entities.Concrete;
using Fitness.Entities.Models.Feedback;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Fitness.Business.Concrete
{
    public class TrainerFeedbackService:ITrainerFeedbackService
    {
        private readonly IFeedbackDal _feedbackDal;
        private readonly IUserDal _userDal;
        private readonly ITrainerDal _trainerDal;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IHubContext<NotificationHub> _hubContext;
        private readonly INotificationService _notificationService;

        public TrainerFeedbackService(IFeedbackDal feedbackDal, IUserDal userDal, ITrainerDal trainerDal, IHttpContextAccessor httpContextAccessor, INotificationService notificationService, IHubContext<NotificationHub> hubContext)
        {
            _feedbackDal = feedbackDal;
            _userDal = userDal;
            _trainerDal = trainerDal;
            _httpContextAccessor = httpContextAccessor;
            _notificationService = notificationService;
            _hubContext = hubContext;
        }
        private async Task<int> GetTrainerIdFromToken()
        {
            var identityId = _httpContextAccessor.HttpContext?.User?.FindFirstValue(ClaimTypes.NameIdentifier);
            var trainer =await  _trainerDal.Get(t => t.IdentityTrainerId == identityId);
            return trainer?.Id ?? throw new Exception("Trainer not found");
        }
        public async Task AddFeedbackAsync(TrainerToUserFeedbackCreateDto dto)
        {
            var trainerId = await GetTrainerIdFromToken();
            var user = await _userDal.Get(u => u.Id == dto.UserId);

            if (user == null)
            {
                Console.WriteLine("User tapılmadı.");
                return;
            }
            var feedback = new Feedback
            {
                TrainerId = trainerId,
                UserId = dto.UserId,
                Comment = dto.Comment,
                IsPositive = dto.IsPositive,
                GivenAt = DateTime.UtcNow
            };

            await _feedbackDal.Add(feedback);

            var message = $"Trainer sizə yeni feedback verdi: {dto.Comment}";
            await _notificationService.CreateNotificationAsync(dto.UserId, message);

            //await _hubContext.Clients.User(dto.UserId.ToString()).SendAsync("ReceiveNotification", message);
           await _hubContext.Clients.User(user.IdentityUserId).SendAsync("ReceiveNotification", message);
            Console.WriteLine("Bildiriş göndərildi: " + message);
        }
    }
}
