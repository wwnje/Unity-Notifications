namespace PuzzlesKingdom.Notifications
{
    public interface IGameNotificationsPlatform
    {
        IGameNotification CreateNotification();
        void ScheduleNotification(IGameNotification gameNotification);
        void CancelNotification(int notificationId);
        void CancelAllScheduledNotifications();
    }
}
