using Unity.Entities;

public struct InvulnerableStateComponent : IComponentData, IEnableableComponent
{
    public float Duration;
    public float ElapsedTime;
}
