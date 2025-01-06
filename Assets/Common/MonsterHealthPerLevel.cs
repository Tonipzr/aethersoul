using System.Collections.Generic;

public static class MonsterHealthPerLevel
{
    private static readonly Dictionary<MonsterType, int> HEALTH_PER_LEVEL = new Dictionary<MonsterType, int>
    {
        { MonsterType.Bat, 5 },
        { MonsterType.Crab, 10 },
        { MonsterType.Golem, 15 },
        { MonsterType.Rat, 20 },
        { MonsterType.Slime, 25 },
        { MonsterType.Boss, 100 },
    };

    public static int CalculateHealth(int level, MonsterType monsterType)
    {
        return level * HEALTH_PER_LEVEL[monsterType];
    }
}