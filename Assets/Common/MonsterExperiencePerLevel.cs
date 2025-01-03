using System.Collections.Generic;

public static class MonsterExperiencePerLevel
{
    private static readonly Dictionary<MonsterType, int> EXPERIENCE_PER_LEVEL = new Dictionary<MonsterType, int>
    {
        { MonsterType.Bat, 2 },
        { MonsterType.Crab, 4 },
        { MonsterType.Golem, 6 },
        { MonsterType.Rat, 8 },
        { MonsterType.Slime, 10 },
        { MonsterType.Boss, 100 },
    };

    public static int CalculateExperience(int level, MonsterType monsterType)
    {
        return level * EXPERIENCE_PER_LEVEL[monsterType];
    }
}