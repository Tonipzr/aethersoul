using Unity.Entities;

public struct AudioComponent : IComponentData
{
    public float Volume;
    public bool IsProcessed;
    public AudioType Audio;
}