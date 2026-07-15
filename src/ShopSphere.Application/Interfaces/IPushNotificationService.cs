using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShopSphere.Application.Interfaces
{
    public interface IPushNotificationService
    {
        Task NotifyUserAsync(string userId, string title, string message, string type = "info");
        Task NotifyAdminsAsync(string title, string message, string type = "info");
        Task NotifyAllAsync(string title, string message, string type = "info");
    }
}
