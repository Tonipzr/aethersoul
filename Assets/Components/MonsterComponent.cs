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
}

public enum MonsterDifficulty
{
    None,
    NightmareBoss,
    MiniBoss,
    Boss
}
