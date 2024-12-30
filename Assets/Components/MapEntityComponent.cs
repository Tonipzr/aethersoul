using Unity.Entities;

public struct MapEntityComponent : IComponentData
{
    public int CurrentSpellsUsed;
    public int CurrentGoldCollected;
    public int CurrentGoldUsed;
    public int CurrentEnemiesKilled;
    public int CurrentEnemiesKilledNoDamage;
    public float CurrentTraveledDistance;
    public int CurrentPOIsVisited;
    public int CurrentPOIsCleared;
    public int CurrentBuffsCollected;
    public int CurrentCheckpointsReached;
    public int CurrentLevelsUp;
    public int CurrentSpellsUnlocked;

    public int TotalSpellsUsed;
    public int TotalGoldCollected;
    public int TotalGoldUsed;
    public int TotalEnemiesKilled;
    public int TotalEnemiesKilledNoDamage;
    public float TotalTraveledDistance;
    public int TotalPOIsVisited;
    public int TotalPOIsCleared;
    public int TotalBuffsCollected;
    public int TotalCheckpointsReached;
    public int TotalLevelsUp;
    public int TotalSpellsUnlocked;
}
