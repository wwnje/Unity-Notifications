#if UNITY_IOS
using System;
using Unity.Notifications.iOS;

namespace PuzzlesKingdom.Notifications.iOS
{
    public class iOSNotificationsPlatform : IGameNotificationsPlatform, IDisposable
    {
        public IGameNotification CreateNotification()
        {
            return new iOSGameNotification();
        }

        public void ScheduleNotification(IGameNotification gameNotification)
        {
            if (gameNotification == null)
            {
                throw new ArgumentNullException(nameof(gameNotification));
            }

            if (!(gameNotification is iOSGameNotification iosGameNotification))
            {
                throw new InvalidOperationException(
                    "Notification provided to ScheduleNotification isn't an IosGameNotification.");
            }

            ScheduleNotification(iosGameNotification);
        }
        
        private void ScheduleNotification(iOSGameNotification notification)
        {
            if (notification == null)
            {
                throw new ArgumentNullException(nameof(notification));
            }

            iOSNotificationCenter.ScheduleNotification(notification.InternalNotification);
        }

        public void CancelNotification(int notificationId)
        {
            iOSNotificationCenter.RemoveScheduledNotification(notificationId.ToString());
        }

        public void CancelAllScheduledNotifications()
        {
            iOSNotificationCenter.RemoveAllScheduledNotifications();
        }

        public void Dispose()
        {
        }
    }
}
#endif
