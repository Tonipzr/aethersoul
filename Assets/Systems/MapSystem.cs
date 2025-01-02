using Unity.Burst;
using Unity.Entities;
using UnityEngine;

partial struct MapSystem : ISystem
{
    private EntityManager _entityManager;

    public void OnCreate(ref SystemState state)
    {
        _entityManager = state.EntityManager;

        SaveData gameSave = SaveGame.Load();

        Entity entity = _entityManager.CreateEntity();
        _entityManager.AddComponent<MapEntityComponent>(entity);

        bool hasStast = gameSave != null && gameSave.Stats != null;

        _entityManager.AddComponentData(entity, new MapEntityComponent
        {
            CurrentSpellsUsed = 0,
            CurrentGoldCollected = 0,
            CurrentGoldUsed = 0,
            CurrentEnemiesKilled = 0,
            CurrentEnemiesKilledNoDamage = 0,
            CurrentTraveledDistance = 0,
            CurrentPOIsVisited = 0,
            CurrentPOIsCleared = 0,
            CurrentBuffsCollected = 0,
            CurrentCheckpointsReached = 0,
            CurrentLevelsUp = 0,
            CurrentSpellsUnlocked = 0,
            TotalSpellsUsed = hasStast ? gameSave.Stats.TotalSpellsUsed : 0,
            TotalGoldCollected = hasStast ? gameSave.Stats.TotalGoldCollected : 0,
            TotalGoldUsed = hasStast ? gameSave.Stats.TotalGoldUsed : 0,
            TotalEnemiesKilled = hasStast ? gameSave.Stats.TotalEnemiesKilled : 0,
            TotalEnemiesKilledNoDamage = hasStast ? gameSave.Stats.TotalEnemiesKilledNoDamage : 0,
            TotalTraveledDistance = hasStast ? gameSave.Stats.TotalTraveledDistance : 0,
            TotalPOIsVisited = hasStast ? gameSave.Stats.TotalPOIsVisited : 0,
            TotalPOIsCleared = hasStast ? gameSave.Stats.TotalPOIsCleared : 0,
            TotalBuffsCollected = hasStast ? gameSave.Stats.TotalBuffsCollected : 0,
            TotalCheckpointsReached = hasStast ? gameSave.Stats.TotalCheckpointsReached : 0,
            TotalLevelsUp = hasStast ? gameSave.Stats.TotalLevelsUp : 0,
            TotalSpellsUnlocked = hasStast ? gameSave.Stats.TotalSpellsUnlocked : 0
        });
        _entityManager.AddComponent<MapEntityPlayerAtChunkComponent>(entity);
        _entityManager.AddComponentData(entity, new MapEntityPlayerAtChunkComponent { PlayerAtChunk = new Vector2Int(0, 0) });
        _entityManager.AddComponent<TimeCounterComponent>(entity);
        _entityManager.AddComponentData(entity, new TimeCounterComponent { ElapsedTime = 0, EndTime = 0, isInfinite = true });
        _entityManager.AddComponent<MapEntityGameStateComponent>(entity);
        _entityManager.AddComponentData(entity, new MapEntityGameStateComponent { IsPaused = false, PlayerCharacter = PlayerCharacter.Escarlina, IsNightmareActive = false });
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        foreach (var gameState in SystemAPI.Query<RefRO<MapEntityGameStateComponent>>())
        {
            if (gameState.ValueRO.IsPaused)
            {
                Time.timeScale = 0;
            }
            else
            {
                Time.timeScale = 1;
            }
        }
    }

    [BurstCompile]
    public void OnDestroy(ref SystemState state)
    {

    }
}
