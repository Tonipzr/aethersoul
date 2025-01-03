using Unity.Entities;

public struct MonsterComponent : IComponentData
{
    public MonsterType MonsterType;
    public MonsterDifficulty MonsterDifficulty;
}

public enum MonsterType
{
    Bat,
    Crab,
    Golem,
    Rat,
    Slime,
    Boss,
}

public enum MonsterDifficulty
{
    None,
    MiniBoss,
    Boss
}
