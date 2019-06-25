namespace PuzzlesKingdom.Lobby
{
    public enum NotificationsType
    {
        Login = 1,

        PveBackFull = 1000,
        PvpBackFull = 2000,
        BuildingQueueCompleted = 3000,
        ArenasSegmentSettlement = 4000,
        GoldCollected = 5000,
        FoodCollected = 6000,
        GuildTitanAppears = 7000,
        GuildTitanRunAwayOrKilled = 8000,
    }

    public static class NotificationsID
    {
        public static int GetID(NotificationsType type)
        {
            if (type == NotificationsType.BuildingQueueCompleted)
            {
                throw new System.Exception(string.Format("please use GetBuildingQueueID"));
            }
            return (int)type;
        }
        
        public static int GetBuildingQueueID(int buildingID)
        {
            return (int)NotificationsType.BuildingQueueCompleted + buildingID;
        }
    }
}
