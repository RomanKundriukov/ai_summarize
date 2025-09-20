using Microsoft.Windows.AppNotifications;
using Microsoft.Windows.AppNotifications.Builder;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ai_summarize.Service
{
    internal static class AppNotificationService
    {
        internal static void GetNotification(string status, string message)
        {
            AppNotification notification = new AppNotificationBuilder()
                .AddText(status)
                .AddText(message)
                .BuildNotification();

            AppNotificationManager.Default.Show(notification);
        }
    }
}
