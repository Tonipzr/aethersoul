using Unity.Entities;

public struct HealthComponent : IComponentData
{
    public int MaxHealth;
    public int CurrentHealth;
    public int BaseMaxHealth;
}
