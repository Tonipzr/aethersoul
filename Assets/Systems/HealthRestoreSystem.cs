using System;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;

partial struct HealthRestoreSystem : ISystem
{
    private EntityManager _entityManager;
    private double elapsedTime;

    [BurstCompile]
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
                    CurrentHealth = Math.Min(health.ValueRO.MaxHealth, health.ValueRO.CurrentHealth + heal.ValueRO.HealAmount),
                    MaxHealth = health.ValueRO.MaxHealth,
                });

                health.ValueRW.CurrentHealth = Math.Min(health.ValueRO.MaxHealth, health.ValueRO.CurrentHealth + heal.ValueRO.HealAmount);
            }
        }

        float deltaTime = SystemAPI.Time.DeltaTime;
        elapsedTime += deltaTime;
        if (elapsedTime >= 5)
        {
            elapsedTime = 0;

            if (SystemAPI.TryGetSingletonEntity<PlayerComponent>(out Entity playerEntity))
            {
                int restoreQuantity = 5;

                if (_entityManager.HasComponent<ActiveUpgradesComponent>(playerEntity))
                {
                    var activeUpgrades = _entityManager.GetBuffer<ActiveUpgradesComponent>(playerEntity);

                    foreach (var upgrade in activeUpgrades)
                    {
                        if (upgrade.Type == UpgradeType.HealthRegen)
                        {
                            restoreQuantity += (int)upgrade.Value;
                        }
                    }
                }

                entityCommandBuffer.AddComponent(playerEntity, new HealthRestoreComponent
                {
                    HealAmount = restoreQuantity
                });
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
