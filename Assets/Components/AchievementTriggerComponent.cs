using Unity.Entities;

public struct AchievementTriggerComponent : IComponentData
{
    public Entity AchievementEntity;
    public bool ShouldActivate;
    public bool TriggerProcessed;
}
