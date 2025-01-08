using Unity.Entities;

public struct LoreEntityComponent : IBufferElementData
{
    public LoreType Type;
    public int Data;
    public int Data2;
}

public enum LoreType
{
    Story,
    MapPosition
}