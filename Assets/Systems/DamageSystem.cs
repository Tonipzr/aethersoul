using System;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;

partial struct DamageSystem : ISystem
{
    private EntityManager _entityManager;

    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {

    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        _entityManager = state.EntityManager;

        EntityCommandBuffer entityCommandBuffer = new EntityCommandBuffer(Allocator.Temp);

        foreach (var (damage, health, entity) in SystemAPI.Query<RefRO<DamageComponent>, RefRW<HealthComponent>>().WithEntityAccess())
        {
            entityCommandBuffer.RemoveComponent<DamageComponent>(entity);
            entityCommandBuffer.AddComponent(entity, new HealthUpdatedComponent
            {
                CurrentHealth = Math.Max(0, health.ValueRO.CurrentHealth - damage.ValueRO.DamageAmount),
                MaxHealth = health.ValueRO.MaxHealth,
            });

            health.ValueRW.CurrentHealth = Math.Max(0, health.ValueRO.CurrentHealth - damage.ValueRO.DamageAmount);

            if (_entityManager.HasComponent<PlayerComponent>(entity))
            {
                var job = new UpdateMapStatsJob
                {
                    Type = MapStatsType.CurrentEnemiesKilledNoDamage,
                    Value = 0,
                    Incremental = false
                };
                job.Schedule();
            }
        }

        entityCommandBuffer.Playback(_entityManager);
        entityCommandBuffer.Dispose();
    }

    [BurstCompile]
    public void OnDestroy(ref SystemState state)
    {

    }
}
