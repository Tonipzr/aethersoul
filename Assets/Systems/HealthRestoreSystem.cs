using System;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;

partial struct HealthRestoreSystem : ISystem
{
    private EntityManager _entityManager;

    public void OnCreate(ref SystemState state)
    {

    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        _entityManager = state.EntityManager;

        EntityCommandBuffer entityCommandBuffer = new EntityCommandBuffer(Allocator.Temp);

        foreach (var (heal, health, entity) in SystemAPI.Query<RefRW<HealthRestoreComponent>, RefRW<HealthComponent>>().WithEntityAccess())
        {
            if (heal.ValueRO.HealAmount > 0)
            {
                entityCommandBuffer.RemoveComponent<HealthRestoreComponent>(entity);
                entityCommandBuffer.AddComponent(entity, new HealthUpdatedComponent
                {
                    CurrentHealth = health.ValueRO.CurrentHealth,
                    MaxHealth = health.ValueRO.MaxHealth,
                    BeforeHealth = Math.Min(health.ValueRO.MaxHealth, health.ValueRO.CurrentHealth + heal.ValueRO.HealAmount),
                    BeforeMaxHealth = health.ValueRO.MaxHealth
                });

                health.ValueRW.CurrentHealth = Math.Min(health.ValueRO.MaxHealth, health.ValueRO.CurrentHealth + heal.ValueRO.HealAmount);
            }
        }

        entityCommandBuffer.Playback(_entityManager);
        entityCommandBuffer.Dispose();
    }

    public void OnDestroy(ref SystemState state)
    {

    }
}
