using Unity.Entities;

public struct DestroyAfterDelayComponent : IComponentData
{
    public float ElapsedTime;
    public float EndTime;
}
