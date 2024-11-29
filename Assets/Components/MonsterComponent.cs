using Unity.Entities;

public struct MonsterComponent : IComponentData
{
    public MonsterType MonsterType;
}

public enum MonsterType
{
    Bat,
    Crab,
    Golem,
    Rat,
    Slime,
}
