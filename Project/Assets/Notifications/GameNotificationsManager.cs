using System;
using JetBrains.Annotations;
#if UNITY_ANDROID
using PuzzlesKingdom.Notifications.Android;
using Unity.Notifications.Android;
#elif UNITY_IOS
using PuzzlesKingdom.Notifications.iOS;
#endif
using UnityEngine;
using Zenject;

namespace PuzzlesKingdom.Notifications
{
    public class OptionalNotification
    {
        public string Title;
        public string Body;
        public DateTime DeliveryTime;

        public int? badgeNumber = null;
        public string channelId = null;
        public string smallIcon = null;
        public string largeIcon = null;
        public string group = null;

        public bool Repeat = false;
    }

    public class GameNotificationsManager : IInitializable, IGameNotificationsManager
    {
        private IGameNotificationsPlatform Platform { get; set; }

        public void Initialize()
        {
#if UNITY_ANDROID
            Platform = new AndroidNotificationsPlatform();
            var c = new AndroidNotificationChannel
            {
                Id = GameNotificationsChannel.ChannelID,
                Name =  GameNotificationsChannel.Name,
                Description =  GameNotificationsChannel.Description,
                Importance = Importance.High,
            };
            ((AndroidNotificationsPlatform)Platform).DefaultChannelId = c.Id;
            AndroidNotificationCenter.RegisterNotificationChannel(c);
#elif UNITY_IOS
            Platform = new iOSNotificationsPlatform();
#endif
            
//#if UNITY_EDITOR
//            Platform = null;
//#endif
            if (null == Platform)
            {
                return;
            }
        }

        public IGameNotification CreateNotification(OptionalNotification notification)
        {
            var no = GetGameNotification(notification);
            if (null == no) return null;
            
            var eventText = $"Queued event with ID \"{no.Id}\" at time {no.DeliveryTime:HH:mm}";
            Debug.Log($"Notification CreateNotification event with text \"{eventText}\"");
            ScheduleNotification(no);
            return no;
        }

        public void UpdateScheduledNotification(int notificationId, OptionalNotification notification)
        {
            var no = GetGameNotification(notification);
            if (null == no) return;

            no.Id = notificationId;
            var eventText = $"Queued event with ID \"{no.Id}\" at time {no.DeliveryTime:HH:mm}";
            Debug.Log($"Notification UpdateScheduledNotification event with text \"{eventText}\"");
            ScheduleNotification(no);
        }

        public void CancelNotification(int notificationId)
        {
            Debug.LogFormat("Notification CancelNotification:{0}", notificationId);
            Platform?.CancelNotification(notificationId);
        }

        public void CancelAllScheduledNotifications()
        {
            Platform?.CancelAllScheduledNotifications();
        }

        private IGameNotification GetGameNotification(OptionalNotification notification)
        {
            var no = Platform?.CreateNotification();
            if (null == no) return null;
            
            no.Title = notification.Title;
            no.Body = notification.Body;
            no.Group = !string.IsNullOrEmpty(notification.channelId) ? notification.channelId : GameNotificationsChannel.ChannelID;
            no.DeliveryTime = notification.DeliveryTime;
            no.SmallIcon = notification.smallIcon;
            no.LargeIcon = notification.largeIcon;
            no.Repeat = notification.Repeat;

            if (notification.group != null)
            {
                no.Group = notification.group ;
            }

            if (notification.badgeNumber != null)
            {
                no.BadgeNumber = notification.badgeNumber ;
            }
            
            return no;
        }

        private void ScheduleNotification(IGameNotification notification)
        {
            if (notification == null || Platform == null)
            {
                return;
            }

            Platform.ScheduleNotification(notification);
        }
    }
}
