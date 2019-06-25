using System;
using Framework.Profile;
using PuzzlesKingdom.Notifications;
using UniRx;
using Zenject;

namespace PuzzlesKingdom.Lobby
{
    public class BuildingNotification : IInitializable, IDisposable
    {
        [Inject] private IGameNotificationsSetting _gameNotificationsSetting;
        [Inject] private IGameNotificationsManager _gameNotificationsManager;
        [Inject] private IBuildingManager _buildingManager;
        [Inject] private IBuildingStateManager _buildingStateManager;
        [Inject] private ITimeManager _timeManager;
        [Inject] private IProfile<NotificationsProfile> _profile;

        private CompositeDisposable _disposable = new CompositeDisposable();
        public void Initialize()
        {
            // TODO 切换账号时需要删除
            _gameNotificationsSetting.BuildingQueueCompleted.Subscribe(OnSettingChange).AddTo(_disposable);
            
            _buildingStateManager.UpgradeSucAsObservable.Where(x => x.isSkipNormal).Subscribe(_ => OnBuildingQueueCompleteCancel(_.id)).AddTo(_disposable);
            _buildingStateManager.UpdateAsObservable.Subscribe(x => OnBuildingChange(x.id)).AddTo(_disposable);
        }

        #region OnSettingChange
        private void OnSettingChange(bool isSetting)
        {
            if (isSetting)
            {
                OnBuildingQueueCompleteOpenCheck();
            }
            else
            {
                OnBuildingQueueCompleteCancelAll(); 
            }

        }
        
        private void OnBuildingQueueCompleteOpenCheck()
        {
            // TODO 检查所有升级建筑
        }
        
        private void OnBuildingQueueCompleteCancelAll()
        {
            foreach (var building in _buildingManager.Buildings.Values)
            {
                OnBuildingQueueCompleteCancel(building.Id);
            }
        }
        #endregion
        
        private void OnBuildingQueueCompleteCancel(int buildingID)
        {
            if(!_profile.Instance.BuildingQueueCompleted) return;

            // 删除所有建筑相关推送
            var id = NotificationsID.GetBuildingQueueID(buildingID);
            _gameNotificationsManager.CancelNotification(id);
        }
        
        private void OnBuildingChange(int buildingId)
        {
            if(!_profile.Instance.BuildingQueueCompleted) return;
            
            var building = _buildingManager.Buildings[buildingId];
            if (building.UpgradeFinishTimeStamp == 0)
            {
                return;
            }

            var seconds = _timeManager.GetRemainTimeInSeconds(building.UpgradeFinishTimeStamp);
            if (seconds <= 0)
            {
                return;
            }
            
            var id = NotificationsID.GetBuildingQueueID(buildingId);
            _gameNotificationsManager.UpdateScheduledNotification(id, 
                new OptionalNotification { Title = "建筑队列升级完成", Body = "身体", DeliveryTime = System.DateTime.Now.AddSeconds(seconds)});
        }

        public void Dispose()
        {
            _disposable?.Dispose();
        }
    }
}
