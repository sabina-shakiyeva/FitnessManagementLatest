using Fitness.Entities.Concrete;
using Fitness.Entities.Models.Notification;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fitness.Business.Abstract
{
    public interface INotificationService
    {
        Task CreateNotificationAsync(int userId, string message);
        Task<List<NotificationDto>> GetUserNotificationsAsync(int userId);
        Task MarkAsReadAsync(int notificationId);
        Task DeleteNotificationAsync(int notificationId);
        Task CreateNotificationForAllAsync(string message);
    }
}
