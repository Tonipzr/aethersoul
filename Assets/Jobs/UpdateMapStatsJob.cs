using Unity.Burst;
using Unity.Entities;

[BurstCompile]
public partial struct UpdateMapStatsJob : IJobEntity
{
    public MapStatsType Type;
    public int Value;
    public float ValueFloat;
    public bool Incremental;

    public void Execute(ref MapEntityComponent mapEntity)
    {
        switch (Type)
        {
            case MapStatsType.CurrentSpellsUsed:
                mapEntity.CurrentSpellsUsed = Incremental ? mapEntity.CurrentSpellsUsed + Value : Value;
                mapEntity.TotalSpellsUsed = Incremental ? mapEntity.TotalSpellsUsed + Value : Value;
                break;
            case MapStatsType.CurrentGoldCollected:
                mapEntity.CurrentGoldCollected = Incremental ? mapEntity.CurrentGoldCollected + Value : Value;
                mapEntity.TotalGoldCollected = Incremental ? mapEntity.TotalGoldCollected + Value : Value;
                break;
            case MapStatsType.CurrentGoldUsed:
                mapEntity.CurrentGoldUsed = Incremental ? mapEntity.CurrentGoldUsed + Value : Value;
                mapEntity.TotalGoldUsed = Incremental ? mapEntity.TotalGoldUsed + Value : Value;
                break;
            case MapStatsType.CurrentEnemiesKilled:
                mapEntity.CurrentEnemiesKilled = Incremental ? mapEntity.CurrentEnemiesKilled + Value : Value;
                mapEntity.TotalEnemiesKilled = Incremental ? mapEntity.TotalEnemiesKilled + Value : Value;
                break;
            case MapStatsType.CurrentEnemiesKilledNoDamage:
                mapEntity.CurrentEnemiesKilledNoDamage = Incremental ? mapEntity.CurrentEnemiesKilledNoDamage + Value : Value;
                mapEntity.TotalEnemiesKilledNoDamage = Incremental ? mapEntity.TotalEnemiesKilledNoDamage + Value : Value;
                break;
            case MapStatsType.CurrentTraveledDistance:
                mapEntity.CurrentTraveledDistance = Incremental ? mapEntity.CurrentTraveledDistance + ValueFloat : Value;
                mapEntity.TotalTraveledDistance = Incremental ? mapEntity.TotalTraveledDistance + ValueFloat : Value;
                break;
            case MapStatsType.CurrentPOIsVisited:
                mapEntity.CurrentPOIsVisited = Incremental ? mapEntity.CurrentPOIsVisited + Value : Value;
                mapEntity.TotalPOIsVisited = Incremental ? mapEntity.TotalPOIsVisited + Value : Value;
                break;
            case MapStatsType.CurrentPOIsCleared:
                mapEntity.CurrentPOIsCleared = Incremental ? mapEntity.CurrentPOIsCleared + Value : Value;
                mapEntity.TotalPOIsCleared = Incremental ? mapEntity.TotalPOIsCleared + Value : Value;
                break;
            case MapStatsType.CurrentBuffsCollected:
                mapEntity.CurrentBuffsCollected = Incremental ? mapEntity.CurrentBuffsCollected + Value : Value;
                mapEntity.TotalBuffsCollected = Incremental ? mapEntity.TotalBuffsCollected + Value : Value;
                break;
            case MapStatsType.CurrentCheckpointsReached:
                mapEntity.CurrentCheckpointsReached = Incremental ? mapEntity.CurrentCheckpointsReached + Value : Value;
                mapEntity.TotalCheckpointsReached = Incremental ? mapEntity.TotalCheckpointsReached + Value : Value;
                break;
            case MapStatsType.CurrentLevelsUp:
                mapEntity.CurrentLevelsUp = Incremental ? mapEntity.CurrentLevelsUp + Value : Value;
                mapEntity.TotalLevelsUp = Incremental ? mapEntity.TotalLevelsUp + Value : Value;
                break;
            case MapStatsType.CurrentSpellsUnlocked:
                mapEntity.CurrentSpellsUnlocked = Incremental ? mapEntity.CurrentSpellsUnlocked + Value : Value;
                mapEntity.TotalSpellsUnlocked = Incremental ? mapEntity.TotalSpellsUnlocked + Value : Value;
                break;
            case MapStatsType.TotalSpellsUsed:
                mapEntity.TotalSpellsUsed = Incremental ? mapEntity.TotalSpellsUsed + Value : Value;
                break;
            case MapStatsType.TotalGoldCollected:
                mapEntity.TotalGoldCollected = Incremental ? mapEntity.TotalGoldCollected + Value : Value;
                break;
            case MapStatsType.TotalGoldUsed:
                mapEntity.TotalGoldUsed = Incremental ? mapEntity.TotalGoldUsed + Value : Value;
                break;
            case MapStatsType.TotalEnemiesKilled:
                mapEntity.TotalEnemiesKilled = Incremental ? mapEntity.TotalEnemiesKilled + Value : Value;
                break;
            case MapStatsType.TotalEnemiesKilledNoDamage:
                mapEntity.TotalEnemiesKilledNoDamage = Incremental ? mapEntity.TotalEnemiesKilledNoDamage + Value : Value;
                break;
            case MapStatsType.TotalTraveledDistance:
                mapEntity.TotalTraveledDistance = Incremental ? mapEntity.TotalTraveledDistance + ValueFloat : Value;
                break;
            case MapStatsType.TotalPOIsVisited:
                mapEntity.TotalPOIsVisited = Incremental ? mapEntity.TotalPOIsVisited + Value : Value;
                break;
            case MapStatsType.TotalPOIsCleared:
                mapEntity.TotalPOIsCleared = Incremental ? mapEntity.TotalPOIsCleared + Value : Value;
                break;
            case MapStatsType.TotalBuffsCollected:
                mapEntity.TotalBuffsCollected = Incremental ? mapEntity.TotalBuffsCollected + Value : Value;
                break;
            case MapStatsType.TotalCheckpointsReached:
                mapEntity.TotalCheckpointsReached = Incremental ? mapEntity.TotalCheckpointsReached + Value : Value;
                break;
            case MapStatsType.TotalLevelsUp:
                mapEntity.TotalLevelsUp = Incremental ? mapEntity.TotalLevelsUp + Value : Value;
                break;
            case MapStatsType.TotalSpellsUnlocked:
                mapEntity.TotalSpellsUnlocked = Incremental ? mapEntity.TotalSpellsUnlocked + Value : Value;
                break;
        }
    }
}

public enum MapStatsType
{
    CurrentSpellsUsed,
    CurrentGoldCollected,
    CurrentGoldUsed,
    CurrentEnemiesKilled,
    CurrentEnemiesKilledNoDamage,
    CurrentTraveledDistance,
    CurrentPOIsVisited,
    CurrentPOIsCleared,
    CurrentBuffsCollected,
    CurrentCheckpointsReached,
    CurrentLevelsUp,
    CurrentSpellsUnlocked,
    TotalSpellsUsed,
    TotalGoldCollected,
    TotalGoldUsed,
    TotalEnemiesKilled,
    TotalEnemiesKilledNoDamage,
    TotalTraveledDistance,
    TotalPOIsVisited,
    TotalPOIsCleared,
    TotalBuffsCollected,
    TotalCheckpointsReached,
    TotalLevelsUp,
    TotalSpellsUnlocked,
}
