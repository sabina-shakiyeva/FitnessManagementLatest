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
        private readonly ITrainerNotificationDal _trainerNotificationDal;
        private readonly IUserDal _userDal;
        private readonly ITrainerDal _trainerDal;

        public GlobalNotificationService(IGlobalNotificationDal globalNotificationDal, IHubContext<NotificationHub> hubContext, IUserNotificationDal userNotificationDal, IUserDal userDal, ITrainerNotificationDal trainerNotificationDal,ITrainerDal trainerDal)
        {
            _globalNotificationDal = globalNotificationDal;
            _hubContext = hubContext;
            _userNotificationDal = userNotificationDal;
            _userDal = userDal;
            _trainerNotificationDal = trainerNotificationDal;
            _trainerDal = trainerDal;
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

        //public async Task CreateGlobalNotificationAsync(string message)
        //{
        //    var notification = new GlobalNotification
        //    {
        //        Message = message,
        //        CreatedAt = DateTime.UtcNow
        //    };

        //    await _globalNotificationDal.Add(notification);

        //    var allUsers = await _userDal.GetList();

        //    foreach (var user in allUsers)
        //    {
        //        var userNotification = new UserNotification
        //        {
        //            UserId = user.Id,
        //            GlobalNotificationId = notification.Id,
        //            IsRead = false
        //        };

        //        await _userNotificationDal.Add(userNotification);
        //    }

        //    await _hubContext.Clients.All.SendAsync("ReceiveGlobalNotification", message);
        //}
        public async Task CreateGlobalNotificationAsync(string message)
        {
            var notification = new GlobalNotification
            {
                Message = message,
                CreatedAt = DateTime.UtcNow
            };

            await _globalNotificationDal.Add(notification);  // EF-in öz Add metodu SaveChangesAsync çağırmalıdır

            // bu addımdan sonra EF avtomatik olaraq notification.Id dəyərini doldurmalıdır
            if (notification.Id == 0)
                throw new Exception("notification.Id təyin olunmayıb! Add metodu SaveChangesAsync çağırırmı?");

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

            var allTrainers = await _trainerDal.GetList();
            foreach (var trainer in allTrainers)
            {
                var trainerNotification = new TrainerNotification
                {
                    TrainerId = trainer.Id,
                    GlobalNotificationId = notification.Id,
                    IsRead = false
                };
                await _trainerNotificationDal.Add(trainerNotification);
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
        public async Task<List<GlobalNotificationDto>> GetAllGlobalNotificationsForTrainerAsync(string identityUserId)
        {
            var trainer = await _trainerDal.Get(t => t.IdentityTrainerId == identityUserId);
            if (trainer == null)
                throw new Exception("Məşqçi tapılmadı.");

            var notifications = await _globalNotificationDal.GetList();
            var trainerNotifications = await _trainerNotificationDal.GetList(x => x.TrainerId == trainer.Id);

            var result = notifications
                .OrderByDescending(n => n.CreatedAt)
                .Select(n => new GlobalNotificationDto
                {
                    Id = n.Id,
                    Message = n.Message,
                    CreatedAt = n.CreatedAt,
                    IsRead = trainerNotifications.FirstOrDefault(tn => tn.GlobalNotificationId == n.Id)?.IsRead ?? false
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
            if (userNotification == null)
            {
                throw new Exception($"UserNotification tapılmadı. UserId: {user.Id}, NotificationId: {notificationId}");
            }

        }

        public async Task MarkAsReadTrainerAsync(int notificationId, string identityUserId)
        {
            var trainer = await _trainerDal.Get(t => t.IdentityTrainerId == identityUserId);
            if (trainer == null)
                throw new Exception("Məşqçi tapılmadı.");

            var trainerNotification = await _trainerNotificationDal.Get(
                x => x.TrainerId == trainer.Id && x.GlobalNotificationId == notificationId
            );

            if (trainerNotification != null && !trainerNotification.IsRead)
            {
                trainerNotification.IsRead = true;
                await _trainerNotificationDal.Update(trainerNotification);
            }

            if (trainerNotification == null)
            {
                throw new Exception($"TrainerNotification tapılmadı. TrainerId: {trainer.Id}, NotificationId: {notificationId}");
            }
        }



    }
}
