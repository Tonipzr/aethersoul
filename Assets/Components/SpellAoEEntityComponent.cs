using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

public struct SpellAoEEntityComponent : IComponentData
{
    public float2 ToPosition;
    public float2 AreaOfEffect;
    public float2 Duration;
}
