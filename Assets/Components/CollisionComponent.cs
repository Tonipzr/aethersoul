using Unity.Entities;

public struct CollisionComponent : IComponentData
{
    public CollisionType Type;
    public bool IsCreated;
}

public enum CollisionType
{
    Tree,
    POI_Pillar,
    POI_Chest,
    POI_Area,
    Buff,
}