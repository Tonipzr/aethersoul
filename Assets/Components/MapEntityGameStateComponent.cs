using Unity.Entities;

public struct MapEntityGameStateComponent : IComponentData
{
    public bool IsPaused;
    public PlayerCharacter PlayerCharacter;
}

public enum PlayerCharacter
{
    None,
    Escarlina,
}