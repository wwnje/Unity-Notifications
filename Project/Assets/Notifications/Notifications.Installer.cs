
namespace PuzzlesKingdom.Notifications
{
    public class NotificationsInstaller : Zenject.Installer<NotificationsInstaller>
    {
        public override void InstallBindings()
        {
            Container.BindInterfacesTo<GameNotificationsManager>().AsSingle();
        }
    }
}
