using System;
using Framework.Profile;
using PuzzlesKingdom.Notifications;
using UniRx;
using Zenject;

namespace PuzzlesKingdom.Lobby
{
    public class ResourcesCollectNotification : IInitializable, IDisposable
    {
        [Inject] private IGameNotificationsSetting _gameNotificationsSetting;
        [Inject] private IGameNotificationsManager _gameNotificationsManager;
        [Inject] private ITimeManager _timeManager;
        [Inject] private IBuildingManager _buildingManager;
        [Inject] private IBuildingStateManager _buildingStateManager;
        [Inject] private IBuildingResourceCollect _buildingResourceCollect;
        [Inject] private IProfile<NotificationsProfile> _profile;
        
        private CompositeDisposable _disposable = new CompositeDisposable();
        private int _marketId;
        private int _millId;
        
        public void Initialize()
        {
            // TODO 切换账号时需要删除
            _gameNotificationsSetting.ResCollected.Subscribe(OnSettingChange).AddTo(_disposable);
            _buildingManager.CollectAsObservable.Subscribe(x => OnUpdate(x.buildingId)).AddTo(_disposable);
            
            // 升级重新检查
            _buildingStateManager.UpgradeSucAsObservable.Where(x => x.buildingType == (int)BuildingType.Mine || x.buildingType == (int)BuildingType.Farm)
                .Subscribe(x => CheckTopBuilding()).AddTo(_disposable);

            _marketId = -1;
            _millId = -1;
            CheckTopBuilding();
        }
        
        #region OnSettingChange
        private void OnSettingChange(bool isSetting)
        {
            if (isSetting)
            {
                CheckTopBuilding();
            }
            else
            {
                OnResCollectCancel(); 
            }
        }
        
        private void OnResCollectCancel()
        {
            var marketId = NotificationsID.GetID(NotificationsType.GoldCollected);
            _gameNotificationsManager.CancelNotification(marketId);
            
            var millId = NotificationsID.GetID(NotificationsType.FoodCollected);
            _gameNotificationsManager.CancelNotification(millId);
        }
        #endregion

        private void CheckTopBuilding()
        {
            if (!_profile.Instance.ResCollected) return;

            var marketLevel = int.MinValue;
            var farmLevel = int.MinValue;
            foreach (var building in _buildingManager.Buildings.Values)
            {
                if (building.BuildingType == BuildingType.Mine)
                {
                    if (building.Level <= marketLevel) continue;
                    _marketId = building.Id;
                    marketLevel = building.Level;
                }
                else if(building.BuildingType == BuildingType.Farm)
                {
                    if (building.Level <= farmLevel) continue;
                    _millId = building.Id;
                    farmLevel = building.Level;
                }
            }

            if (_marketId > 0)
            {
                OnChange(NotificationsType.GoldCollected, _marketId);
            }
            
            if (_millId > 0)
            {
                OnChange(NotificationsType.FoodCollected, _millId);
            }
        }

        private void OnUpdate(int buildingId)
        {
            if (!_profile.Instance.ResCollected) return;
            if (buildingId == _marketId)
            {
                OnChange(NotificationsType.GoldCollected, buildingId);
            }
            else if(buildingId == _millId)
            {
                OnChange(NotificationsType.FoodCollected, buildingId);
            }
        }

        private void OnChange(NotificationsType type, int buildingId)
        {
            var building = _buildingManager.Buildings[buildingId];
            var seconds = _buildingResourceCollect.GetFullRemainTime(building);
            if (seconds <= 0)
            {
                return;
            }
            
            var id = NotificationsID.GetID(type);
            _gameNotificationsManager.UpdateScheduledNotification(id, 
                new OptionalNotification { Title = "资源恢复完成", Body = type.ToString(), DeliveryTime = System.DateTime.Now.AddSeconds(seconds)});
        }

        public void Dispose()
        {
            _disposable?.Dispose();
        }
    }
}
