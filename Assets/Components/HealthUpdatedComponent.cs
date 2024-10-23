using Unity.Entities;

public struct HealthUpdatedComponent : IComponentData
{
    public int CurrentHealth;
    public int MaxHealth;

    public int BeforeHealth;
    public int BeforeMaxHealth;
}
