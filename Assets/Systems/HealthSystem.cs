using System;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;

partial struct HealthSystem : ISystem
{
    private EntityManager _entityManager;

    public void OnCreate(ref SystemState state)
    {

    }

    public void OnUpdate(ref SystemState state)
    {
        _entityManager = state.EntityManager;

        EntityCommandBuffer entityCommandBuffer = new EntityCommandBuffer(Allocator.Temp);

        foreach (var (health, entity) in SystemAPI.Query<RefRW<HealthComponent>>().WithEntityAccess())
        {
            if (health.ValueRO.CurrentHealth <= 0)
            {
                entityCommandBuffer.AddComponent(entity, new DeathComponent { });
                continue;
            }
        }

        entityCommandBuffer.Playback(_entityManager);
        entityCommandBuffer.Dispose();
    }

    public void OnDestroy(ref SystemState state)
    {

    }
}
