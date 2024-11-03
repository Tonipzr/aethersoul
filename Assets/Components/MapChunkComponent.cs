using Unity.Entities;
using Unity.Mathematics;

public struct MapChunkComponent : IComponentData
{
    public int2 ChunkCoordinate;
    public int Size;
}
