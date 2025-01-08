using Unity.Entities;

public struct AchievementComponent : IComponentData
{
    public int AchievementID;
    public bool IsCompleted;
    public bool IsProcessed;
}
