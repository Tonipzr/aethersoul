using Unity.Entities;

public struct SpellComponent : IComponentData
{
    public int SpellID;
    public SpellType SpellType;
}
