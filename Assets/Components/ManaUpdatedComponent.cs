using Unity.Entities;

public struct ManaUpdatedComponent : IComponentData
{
    public int CurrentMana;
    public int MaxMana;
}
