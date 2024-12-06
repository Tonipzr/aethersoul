using Unity.Entities;

public struct MapCheckpointEntityComponent : IComponentData
{
    public UnityEngine.Vector2Int Coordinates;
    public bool IsColliding;
}
