using Unity.Entities;
using Unity.Mathematics;

public struct ExperienceShardEntityComponent : IComponentData
{
    public float AoEPickUpRange;
    public int ExperienceQuantity;
}
