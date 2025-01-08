using System.Collections.Generic;

public static class MonsterExperiencePerLevel
{
    private static readonly Dictionary<MonsterType, int> EXPERIENCE_PER_LEVEL = new Dictionary<MonsterType, int>
    {
        { MonsterType.Bat, 4 },
        { MonsterType.Crab, 6 },
        { MonsterType.Golem, 8 },
        { MonsterType.Rat, 10 },
        { MonsterType.Slime, 12 },
        { MonsterType.Boss, 100 },
    };

    public static int CalculateExperience(int level, MonsterType monsterType)
    {
        return level * EXPERIENCE_PER_LEVEL[monsterType];
    }
}