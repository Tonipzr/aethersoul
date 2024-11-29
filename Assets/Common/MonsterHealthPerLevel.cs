using System.Collections.Generic;

public static class MonsterHealthPerLevel
{
    private static readonly Dictionary<MonsterType, int> HEALTH_PER_LEVEL = new Dictionary<MonsterType, int>
    {
        { MonsterType.Bat, 10 },
        { MonsterType.Crab, 20 },
        { MonsterType.Golem, 30 },
        { MonsterType.Rat, 40 },
        { MonsterType.Slime, 50 },
    };

    public static int CalculateHealth(int level, MonsterType monsterType)
    {
        return level * HEALTH_PER_LEVEL[monsterType];
    }
}