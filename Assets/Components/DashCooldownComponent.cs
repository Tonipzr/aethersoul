using Unity.Entities;

public struct DashCooldownComponent : IComponentData, IEnableableComponent
{
    public int Cooldown;

    public float CurrentTimeOnCooldown;
}
