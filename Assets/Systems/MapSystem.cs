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
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {

    }

    [BurstCompile]
    public void OnDestroy(ref SystemState state)
    {

    }
}
