#if UNITY_ANDROID
using System;
using Unity.Notifications.Android;
using UnityEngine;

namespace PuzzlesKingdom.Notifications.Android
{
    public class AndroidNotificationsPlatform : IGameNotificationsPlatform, IDisposable
    {
        public string DefaultChannelId { get; set; }

        public AndroidNotificationsPlatform()
        {
            AndroidNotificationCenter.OnNotificationReceived += OnLocalNotificationReceived;
        }
        
        public IGameNotification CreateNotification()
        {
            var notification = new AndroidGameNotification
            {
                DeliveredChannel = DefaultChannelId
            };
            return notification;
        }

        public void ScheduleNotification(IGameNotification gameNotification)
        {
            if (gameNotification == null)
            {
                throw new ArgumentNullException(nameof(gameNotification));
            }

            if (!(gameNotification is AndroidGameNotification androidNotification))
            {
                throw new InvalidOperationException(
                    "Notification provided to ScheduleNotification isn't an AndroidGameNotification.");
            }

            ScheduleNotification(androidNotification);
        }
                
        public void CancelNotification(int notificationId)
        {
            Debug.LogFormat("AndroidPlatformNotification CancelNotification:{0}", notificationId);
            AndroidNotificationCenter.CancelScheduledNotification(notificationId);
        }

        public void CancelAllScheduledNotifications()
        {
            AndroidNotificationCenter.CancelAllScheduledNotifications();
        }

        private void ScheduleNotification(AndroidGameNotification gameNotification)
        {
            if (gameNotification.Id.HasValue)
            {
                var identifier = gameNotification.Id.Value;
                var status = AndroidNotificationCenter.CheckScheduledNotificationStatus(identifier);
                Debug.LogFormat("AndroidGameNotification, status:{0}", status.ToString());
                switch (status)
                {
                    case NotificationStatus.Scheduled:
                        CancelNotification(identifier);
                        
                        // TODO checkIfPendingNotificationIsRegistered is error
                        // Replace the currently scheduled notification with a new notification.
//                        AndroidNotificationCenter.UpdateScheduledNotification(identifier, gameNotification.InternalNotification, gameNotification.DeliveredChannel);
//                        return;
                        break;
                    case NotificationStatus.Delivered:
                        //Remove the notification from the status bar
                        CancelNotification(identifier);
                        break;
                }

                AndroidNotificationCenter.SendNotificationWithExplicitID(gameNotification.InternalNotification,
                    gameNotification.DeliveredChannel,
                    gameNotification.Id.Value);
            }
            else
            {
                Debug.LogFormat("AndroidGameNotification ScheduleNotification");
                var notificationId = AndroidNotificationCenter.SendNotification(gameNotification.InternalNotification,
                    gameNotification.DeliveredChannel);
                gameNotification.Id = notificationId;
            }
        }

        /// <summary>
        /// Unregister delegates.
        /// </summary>
        public void Dispose()
        {
            AndroidNotificationCenter.OnNotificationReceived -= OnLocalNotificationReceived;
        }

        readonly AndroidNotificationCenter.NotificationReceivedCallback OnLocalNotificationReceived = 
            delegate(AndroidNotificationIntentData data)
            {
                var msg = "Notification received : " + data.Id + "\n";
                msg += "\n Notification received: ";
                msg += "\n .Title: " + data.Notification.Title;
                msg += "\n .Body: " + data.Notification.Text;
                msg += "\n .Channel: " + data.Channel;
                Debug.Log(msg);
            };
    }
}
#endif
