using System;

public static class ExperienceToNextLevel
{
    private static readonly int EXP_PER_LEVEL = 50;

    public static int CalculateExperienceToNextLevel(int level, int reduction)
    {
        return Math.Max((level * EXP_PER_LEVEL) - reduction, 1);
    }
}