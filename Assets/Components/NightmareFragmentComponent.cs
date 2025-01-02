using Unity.Collections;
using Unity.Entities;

public struct NightmareFragmentComponent : IComponentData
{
    public NightmareFragmentType Type;
    public bool IsActive;
    public bool IsCompleted;
    public bool IsCompletedProcessed;
    public bool IsVisited;
    public int RemainingEnemies;

    public float StartedAtTime;

    public NativeArray<Entity> PillarEntities;
    public Entity AreaEntity;
}

public enum NightmareFragmentType
{
    Endless_Siege,
    Fury
}
