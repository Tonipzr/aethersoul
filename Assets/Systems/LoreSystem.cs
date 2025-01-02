using Unity.Burst;
using Unity.Entities;
using UnityEngine;

partial struct LoreSystem : ISystem
{
    private EntityManager _entityManager;

    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        _entityManager = state.EntityManager;

        Entity entity = _entityManager.CreateEntity();
        _entityManager.AddBuffer<LoreEntityComponent>(entity);
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
