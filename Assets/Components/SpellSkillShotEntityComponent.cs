using Unity.Entities;
using Unity.Mathematics;

public struct SpellSkillShotEntityComponent : IComponentData
{
    public float2 ToPosition;
    public float2 FromPosition;
}
