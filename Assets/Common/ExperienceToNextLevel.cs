public static class ExperienceToNextLevel
{
    private static readonly int EXP_PER_LEVEL = 75;

    public static int CalculateExperienceToNextLevel(int level, int reduction)
    {
        return (level * EXP_PER_LEVEL) - reduction;
    }
}