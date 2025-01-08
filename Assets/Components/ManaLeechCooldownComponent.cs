using Unity.Entities;

public struct ManaLeechCooldownComponent : IComponentData, IEnableableComponent
{
    public float Cooldown;
    public float CurrentTimeOnCooldown;
}
