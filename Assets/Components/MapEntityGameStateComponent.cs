using Unity.Entities;

public struct MapEntityGameStateComponent : IComponentData
{
    public bool GameStarted;
    public bool IsPaused;
    public PlayerCharacter PlayerCharacter;
    public GamePhase GamePhase;
    public bool IsNightmareActive;
}

public enum PlayerCharacter
{
    None,
    Escarlina,
}

public enum GamePhase
{
    None,
    Phase1,
    Phase2,
    PhaseBoss,
}