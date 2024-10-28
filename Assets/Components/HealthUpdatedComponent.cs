using Unity.Entities;

public struct HealthUpdatedComponent : IComponentData
{
    public int CurrentHealth;
    public int MaxHealth;
}
