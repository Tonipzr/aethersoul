using Unity.Entities;

public struct SpawnPointComponent : IComponentData
{
    public int SpawnLevel;
    public SpawnType Type;
    public MonsterDifficulty Difficulty;
    public MonsterType MonsterType;
    public Entity Parent;
}

public enum SpawnType
{
    None,
    NightmareFragment
}
