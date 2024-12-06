using Unity.Entities;

public struct ManaComponent : IComponentData
{
    public int MaxMana;
    public int CurrentMana;
    public int BaseMaxMana;
}
