namespace PuzzlesKingdom.Notifications
{
    public interface IGameNotificationsManager
    {
        IGameNotification CreateNotification(OptionalNotification notification);
        void UpdateScheduledNotification(int notificationId, OptionalNotification notification);
        void CancelNotification(int notificationId);
        void CancelAllScheduledNotifications();
    }
}
