using Fitness.Entities.Concrete;
using Fitness.Entities.Models.Notification;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fitness.Business.Abstract
{
    public interface IGlobalNotificationService
    {
        Task CreateGlobalNotificationAsync(string message);
        //Task<List<GlobalNotification>> GetAllGlobalNotificationsAsync();
        Task<List<GlobalNotificationDto>> GetAllGlobalNotificationsAsync(string identityUserId);
        //Task MarkAsReadAsync(int notificationId);
        Task MarkAsReadAsync(int notificationId, string identityUserId);
    }
}
