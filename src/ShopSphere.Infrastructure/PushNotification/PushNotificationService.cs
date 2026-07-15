using Microsoft.AspNetCore.SignalR;
using ShopSphere.Application.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShopSphere.Infrastructure.PushNotification
{
    public sealed class PushNotificationService : IPushNotificationService
    {
        private readonly IHubContext<PushNotificationHub> _hub;

        public PushNotificationService(IHubContext<PushNotificationHub> hub)
        {
            _hub = hub;
        }

        public async Task NotifyUserAsync(
            string userId,
            string title,
            string message,
            string type = "info")
        {
            await _hub.Clients.Group($"user-{userId}").SendAsync("ReceiveNotification", new
            {
                title,
                message,
                type,
                timestamp = DateTime.UtcNow,
            });
        }

        public async Task NotifyAdminsAsync(
            string title,
            string message,
            string type = "info")
        {
            await _hub.Clients.Group("admins").SendAsync("ReceiveNotification", new
            {
                title,
                message,
                type,
                timestamp = DateTime.UtcNow,
            });
        }

        public async Task NotifyAllAsync(
            string title,
            string message,
            string type = "info")
        {
            await _hub.Clients.All.SendAsync("ReceiveNotification", new
            {
                title,
                message,
                type,
                timestamp = DateTime.UtcNow,
            });
        }
    }
}
