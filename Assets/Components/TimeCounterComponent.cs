using Unity.Entities;

public struct TimeCounterComponent : IComponentData
{
    public float ElapsedTime;
    public float EndTime;
    public bool isInfinite;
}
