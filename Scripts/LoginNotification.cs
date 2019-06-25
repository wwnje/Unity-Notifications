using System;
using Framework.Profile;
using PuzzlesKingdom.Notifications;
using UniRx;
using UnityEngine;
using Zenject;

namespace PuzzlesKingdom.Lobby
{
    public class LoginNotification : IInitializable, IDisposable
    {
        [Inject] private IGameNotificationsManager _gameNotificationsManager;
        [Inject] private ITimeManager _timeManager;

        private CompositeDisposable _disposable = new CompositeDisposable();

        public void Initialize()
        {
            var id = NotificationsID.GetID(NotificationsType.Login);
            
            var no = new OptionalNotification {Title = "提醒上线", Body = "身体", DeliveryTime = System.DateTime.Today.AddDays(1).AddHours(16)};
            _gameNotificationsManager.UpdateScheduledNotification(id, no);
            Debug.LogFormat("LoginNotification:{0}", no.DeliveryTime);
        }

        public void Dispose()
        {
        }
    }
}
