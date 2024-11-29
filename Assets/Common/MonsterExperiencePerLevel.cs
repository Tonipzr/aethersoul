using System.Collections.Generic;

public static class MonsterExperiencePerLevel
{
    private static readonly Dictionary<MonsterType, int> EXPERIENCE_PER_LEVEL = new Dictionary<MonsterType, int>
    {
        { MonsterType.Bat, 10 },
        { MonsterType.Crab, 20 },
        { MonsterType.Golem, 30 },
        { MonsterType.Rat, 40 },
        { MonsterType.Slime, 50 },
    };

    public static int CalculateExperience(int level, MonsterType monsterType)
    {
        return level * EXPERIENCE_PER_LEVEL[monsterType];
    }
}