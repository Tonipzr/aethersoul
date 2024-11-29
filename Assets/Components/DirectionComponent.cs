using Unity.Entities;

public struct DirectionComponent : IComponentData
{
    public Direction Direction;
}

public enum Direction { Left, Right }
