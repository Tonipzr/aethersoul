using Unity.Entities;

public struct MapEntityGameStateComponent : IComponentData
{
    public bool IsPaused;
    public PlayerCharacter PlayerCharacter;
    public bool IsNightmareActive;
}

public enum PlayerCharacter
{
    None,
    Escarlina,
}