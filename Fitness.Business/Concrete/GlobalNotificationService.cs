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
    public class GlobalNotificationService:IGlobalNotificationService
    {
        private readonly IGlobalNotificationDal _globalNotificationDal;
        private readonly IHubContext<NotificationHub> _hubContext;
        private readonly IUserNotificationDal _userNotificationDal;
        private readonly IUserDal _userDal;

        public GlobalNotificationService(IGlobalNotificationDal globalNotificationDal, IHubContext<NotificationHub> hubContext, IUserNotificationDal userNotificationDal, IUserDal userDal)
        {
            _globalNotificationDal = globalNotificationDal;
            _hubContext = hubContext;
            _userNotificationDal = userNotificationDal;
            _userDal = userDal;
        }

        //public async Task CreateGlobalNotificationAsync(string message)
        //{
        //    var notification = new GlobalNotification
        //    {
        //        Message = message,
        //        CreatedAt = DateTime.UtcNow
        //    };

        //    await _globalNotificationDal.Add(notification);

        //    await _hubContext.Clients.All.SendAsync("ReceiveGlobalNotification", message);
        //}

        public async Task CreateGlobalNotificationAsync(string message)
        {
            var notification = new GlobalNotification
            {
                Message = message,
                CreatedAt = DateTime.UtcNow
            };

            await _globalNotificationDal.Add(notification);

            var allUsers = await _userDal.GetList();

            foreach (var user in allUsers)
            {
                var userNotification = new UserNotification
                {
                    UserId = user.Id,
                    GlobalNotificationId = notification.Id,
                    IsRead = false
                };

                await _userNotificationDal.Add(userNotification);
            }

            await _hubContext.Clients.All.SendAsync("ReceiveGlobalNotification", message);
        }
        public async Task<List<GlobalNotificationDto>> GetAllGlobalNotificationsAsync(string identityUserId)
        {
            var user = await _userDal.Get(u => u.IdentityUserId == identityUserId);
            if (user == null)
                throw new Exception("İstifadəçi tapılmadı.");

            var notifications = await _globalNotificationDal.GetList();
            var userNotifications = await _userNotificationDal.GetList(x => x.UserId == user.Id);

            var result = notifications
                .OrderByDescending(n => n.CreatedAt)
                .Select(n => new GlobalNotificationDto
                {
                    Id = n.Id,
                    Message = n.Message,
                    CreatedAt = n.CreatedAt,
                    IsRead = userNotifications.FirstOrDefault(un => un.GlobalNotificationId == n.Id)?.IsRead ?? false
                }).ToList();

            return result;
        }


        //public async Task<List<GlobalNotification>> GetAllGlobalNotificationsAsync()
        //{
        //    return await _globalNotificationDal.GetList();
        //}


        //public async Task MarkAsReadAsync(int notificationId)
        //{
        //    var notif = await _globalNotificationDal.Get(n => n.Id == notificationId);
        //    if (notif != null)
        //    {
        //        notif.IsRead = true;
        //        await _globalNotificationDal.Update(notif);
        //    }
        //}

        public async Task MarkAsReadAsync(int notificationId, string identityUserId)
        {
            var user = await _userDal.Get(u => u.IdentityUserId == identityUserId);
            if (user == null)
                throw new Exception("İstifadəçi tapılmadı.");

            var userNotification = await _userNotificationDal.Get(
                x => x.UserId == user.Id && x.GlobalNotificationId == notificationId
            );

            if (userNotification != null && !userNotification.IsRead)
            {
                userNotification.IsRead = true;
                await _userNotificationDal.Update(userNotification);
            }
        }


    }
}
