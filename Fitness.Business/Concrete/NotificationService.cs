using Fitness.Business.Abstract;
using Fitness.Business.Hubs;
using Fitness.DataAccess.Abstract;
using Fitness.DataAccess.Concrete.EfEntityFramework;
using Fitness.Entities.Concrete;
using Fitness.Entities.Models.Notification;
using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fitness.Business.Concrete
{
    public class NotificationService:INotificationService
    {
        private readonly INotificationDal _notificationDal;
        private readonly IHubContext<NotificationHub> _hubContext;
        private readonly IUserDal _userDal;
        private readonly ITrainerDal _trainerDal;

        public NotificationService(INotificationDal notificationDal, IHubContext<NotificationHub> hubContext, IUserDal userDal, ITrainerDal trainerDal)
        {
            _notificationDal = notificationDal;
            _hubContext = hubContext;
            _userDal = userDal;
            _trainerDal = trainerDal;
        }
        public async Task CreateNotificationAsync(int userId, string message)
        {
            var notification = new Notification
            {
                UserId = userId,
                Message = message,
                CreatedAt = DateTime.UtcNow,
                IsRead = false
            };

            await _notificationDal.Add(notification);

            await _hubContext.Clients.User(userId.ToString())
           .SendAsync("ReceiveNotification", message);
        }
        public async Task CreateNotificationForAllAsync(string message)
        {
            var users = await _userDal.GetList();
            var trainers = await _trainerDal.GetList();

            var allIds = users.Select(u => u.Id)
                              .Concat(trainers.Select(t => t.Id))
                              .ToList();

            foreach (var id in allIds)
            {
               
                await CreateNotificationAsync(id, message);
            }

            await _hubContext.Clients.All.SendAsync("ReceiveNotification", message);
         
        }

        public async Task<List<NotificationDto>> GetUserNotificationsAsync(int userId)
        {
            var notifications = await _notificationDal.GetList(n => n.UserId == userId);

            return notifications.Select(n => new NotificationDto
            {
                Id = n.Id,
                UserId = n.UserId,
                Message = n.Message,
                CreatedAt = n.CreatedAt,
                IsRead = n.IsRead
            }).ToList();
        }

      

        public async Task MarkAsReadAsync(int notificationId)
        {
            var notif = await _notificationDal.Get(n => n.Id == notificationId);
            if (notif != null)
            {
                notif.IsRead = true;
                await _notificationDal.Update(notif);
            }
        }

        public async Task DeleteNotificationAsync(int notificationId)
        {
            var notif = await _notificationDal.Get(n => n.Id == notificationId);
            if (notif != null)
                await _notificationDal.Delete(notif);
        }
    }
}
