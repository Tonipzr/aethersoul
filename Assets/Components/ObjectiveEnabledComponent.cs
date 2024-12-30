using Unity.Entities;

public struct ObjectiveEnabledComponent : IComponentData, IEnableableComponent
{
    public bool Completed;
    public bool Processed;
}
