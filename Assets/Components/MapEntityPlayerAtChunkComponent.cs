using Unity.Entities;
using UnityEngine;

public struct MapEntityPlayerAtChunkComponent : IComponentData
{
    public Vector2Int PlayerAtChunk;
}
