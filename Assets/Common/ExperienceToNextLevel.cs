public static class ExperienceToNextLevel
{
    private static readonly int EXP_PER_LEVEL = 145;

    public static int CalculateExperienceToNextLevel(int level)
    {
        return level * EXP_PER_LEVEL;
    }
}