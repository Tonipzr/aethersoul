using Unity.Entities;
using Unity.Mathematics;

public struct SpellAoEEntityComponent : IComponentData
{
    public float2 ToPosition;
    public float2 AreaOfEffect;
    public float2 Duration;
}
