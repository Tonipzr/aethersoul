using Unity.Entities;

public struct ExperienceUpdatedComponent : IComponentData
{
    public int CurrentExperience;
    public int MaxExperience;
}
