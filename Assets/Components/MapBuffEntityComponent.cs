using Unity.Entities;

public struct MapBuffEntityComponent : IComponentData
{
    public UnityEngine.Vector2Int Coordinates;
    public bool IsUsed;
}
