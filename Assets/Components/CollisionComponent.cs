using Unity.Entities;

public struct CollisionComponent : IComponentData
{
    public CollisionType Type;
    public bool IsCreated;
}

public enum CollisionType
{
    Tree,
    POI_PILLAR_TOP_LEFT,
    POI_PILLAR_TOP_RIGHT,
    POI_PILLAR_BOTTOM_LEFT,
    POI_PILLAR_BOTTOM_RIGHT,
    POI_CHEST
}