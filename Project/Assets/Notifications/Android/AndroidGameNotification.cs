#if UNITY_ANDROID
using System;
using Unity.Notifications.Android;

namespace PuzzlesKingdom.Notifications.Android
{
    public class AndroidGameNotification : IGameNotification
    {
        private AndroidNotification internalNotification;
        public AndroidNotification InternalNotification => internalNotification;
        
        public int? Id { get; set; }
        public string Title { get => InternalNotification.Title; set => internalNotification.Title = value; }
        public string Body { get => InternalNotification.Text; set => internalNotification.Text = value; }
        
        /// <summary>
        /// Does nothing on Android.
        /// </summary>
        public string Subtitle { get => null; set { } }
        
        /// <inheritdoc />
        /// <remarks>
        /// On Android, this represents the notification's channel, and is required. Will be configured automatically by
        /// <see cref="AndroidNotificationsPlatform"/> if <see cref="AndroidNotificationsPlatform.DefaultChannelId"/> is set
        /// </remarks>
        /// <value>The value of <see cref="DeliveredChannel"/>.</value>
        public string Group { get => DeliveredChannel; set => DeliveredChannel = value; }
        
        public DateTime? DeliveryTime
        {
            get => InternalNotification.FireTime;
            set => internalNotification.FireTime = value ?? throw new ArgumentNullException(nameof(value));
        }
        
        /// <inheritdoc />
        public int? BadgeNumber
        {
            get => internalNotification.Number != -1 ? internalNotification.Number : (int?)null;
            set => internalNotification.Number = value ?? -1;
        }
        
        public bool Scheduled { get; }
		
        /// <inheritdoc />
        public string SmallIcon { get => InternalNotification.SmallIcon; set => internalNotification.SmallIcon = value; }

        /// <inheritdoc />
        public string LargeIcon { get => InternalNotification.LargeIcon; set => internalNotification.LargeIcon = value; }

        public bool Repeat { get; set; }

        /// <summary>
        /// Gets or sets the channel for this notification.
        /// </summary>
        public string DeliveredChannel { get; set; }
        
        /// <summary>
        /// Instantiate a new instance of <see cref="AndroidGameNotification"/>.
        /// </summary>
        public AndroidGameNotification()
        {
            internalNotification = new AndroidNotification();
        }
    }
}
#endif
