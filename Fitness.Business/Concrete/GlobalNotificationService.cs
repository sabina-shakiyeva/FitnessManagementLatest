using Fitness.Business.Abstract;
using Fitness.Business.Hubs;
using Fitness.DataAccess.Abstract;
using Fitness.Entities.Concrete;
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

        public GlobalNotificationService(IGlobalNotificationDal globalNotificationDal, IHubContext<NotificationHub> hubContext)
        {
            _globalNotificationDal = globalNotificationDal;
            _hubContext = hubContext;
        }

        public async Task CreateGlobalNotificationAsync(string message)
        {
            var notification = new GlobalNotification
            {
                Message = message,
                CreatedAt = DateTime.UtcNow
            };

            await _globalNotificationDal.Add(notification);

            await _hubContext.Clients.All.SendAsync("ReceiveGlobalNotification", message);
        }

        public async Task<List<GlobalNotification>> GetAllGlobalNotificationsAsync()
        {
            return await _globalNotificationDal.GetList();
        }
    }
}
