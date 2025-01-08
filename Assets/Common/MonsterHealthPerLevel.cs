using System.Collections.Generic;

public static class MonsterHealthPerLevel
{
    private static readonly Dictionary<MonsterType, int> HEALTH_PER_LEVEL = new Dictionary<MonsterType, int>
    {
        { MonsterType.Bat, 3 },
        { MonsterType.Crab, 4 },
        { MonsterType.Golem, 5 },
        { MonsterType.Rat, 6 },
        { MonsterType.Slime, 7 },
        { MonsterType.Boss, 10 },
    };

    public static int CalculateHealth(int level, MonsterType monsterType)
    {
        return level * HEALTH_PER_LEVEL[monsterType];
    }
}