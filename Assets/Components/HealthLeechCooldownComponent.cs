using Unity.Entities;

public struct HealthLeechCooldownComponent : IComponentData, IEnableableComponent
{
    public float Cooldown;
    public float CurrentTimeOnCooldown;
}
