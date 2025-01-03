using Unity.Entities;

public struct FollowComponent : IComponentData
{
    public Entity Target;
    public float MinDistance;
    public float MaxDistance;
}
