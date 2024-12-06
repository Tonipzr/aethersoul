using Unity.Burst;
using Unity.Entities;
using UnityEngine;

partial struct MapSystem : ISystem
{
    private EntityManager _entityManager;

    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        _entityManager = state.EntityManager;

        Entity entity = _entityManager.CreateEntity();
        _entityManager.AddComponent<MapEntityComponent>(entity);
        _entityManager.AddComponent<MapEntityPlayerAtChunkComponent>(entity);
        _entityManager.AddComponentData(entity, new MapEntityPlayerAtChunkComponent { PlayerAtChunk = new Vector2Int(0, 0) });
        _entityManager.AddComponent<TimeCounterComponent>(entity);
        _entityManager.AddComponentData(entity, new TimeCounterComponent { ElapsedTime = 0, EndTime = 0, isInfinite = true });
        _entityManager.AddComponent<MapEntityGameStateComponent>(entity);
        _entityManager.AddComponentData(entity, new MapEntityGameStateComponent { IsPaused = false });
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
