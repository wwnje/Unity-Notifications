using System;
using Framework.Profile;
using PuzzlesKingdom.Notifications;
using UniRx;
using Zenject;

namespace PuzzlesKingdom.Lobby
{
    public class UserNotification : IInitializable, IDisposable
    {
        [Inject] private IGameNotificationsSetting _gameNotificationsSetting;
        [Inject] private IGameNotificationsManager _gameNotificationsManager;
        [Inject] private ITimeManager _timeManager;
        [Inject] private IPlayerData _playerData;
        [Inject] private IUserInfo _userInfo;
        [Inject] private IProfile<NotificationsProfile> _profile;

        private CompositeDisposable _disposable = new CompositeDisposable();
        public void Initialize()
        {
            // TODO 切换账号时需要删除
            _gameNotificationsSetting.PvBackFull.Subscribe(OnSettingChange).AddTo(_disposable);
            _playerData.PveEnergyCD.Where(_ => _profile.Instance.PvBackFull).Subscribe(x => OnChange(NotificationsType.PveBackFull, _userInfo.GetPveRemainTime)).AddTo(_disposable);
            _playerData.RaidEnergyCD.Where(_ => _profile.Instance.PvBackFull).Subscribe(x => OnChange(NotificationsType.PvpBackFull, _userInfo.GetPvpRemainTime)).AddTo(_disposable);
        }
        
        #region OnSettingChange
        private void OnSettingChange(bool isSetting)
        {
            if (isSetting)
            {
                OnPvBackFullOpenCheck();
            }
            else
            {
                OnPvBackFullCancel(); 
            }
        }
        
        private void OnPvBackFullOpenCheck()
        {
            OnChange(NotificationsType.PveBackFull, _userInfo.GetPveRemainTime);
            OnChange(NotificationsType.PvpBackFull, _userInfo.GetPvpRemainTime);
        }
        
        private void OnPvBackFullCancel()
        {
            var pvpId = NotificationsID.GetID(NotificationsType.PvpBackFull);
            _gameNotificationsManager.CancelNotification(pvpId);
            
            var pveId = NotificationsID.GetID(NotificationsType.PveBackFull);
            _gameNotificationsManager.CancelNotification(pveId);
        }
        #endregion
        
        private void OnChange(NotificationsType type, long seconds)
        {
            if (seconds <= 0)
            {
                return;
            }
            
            var id = NotificationsID.GetID(type);
            _gameNotificationsManager.UpdateScheduledNotification(id, 
                new OptionalNotification { Title = "体力恢复完成", Body = type.ToString(), DeliveryTime = System.DateTime.Now.AddSeconds(seconds)});
        }

        public void Dispose()
        {
            _disposable?.Dispose();
        }
    }
}
