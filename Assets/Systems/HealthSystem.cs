using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using UnityEngine;

partial struct HealthSystem : ISystem
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

        foreach (var (health, entity) in SystemAPI.Query<RefRW<HealthComponent>>().WithEntityAccess())
        {
            if (health.ValueRO.CurrentHealth <= 0)
            {
                entityCommandBuffer.AddComponent(entity, new DeathComponent { });
                continue;
            }

            if (_entityManager.HasComponent<ActiveUpgradesComponent>(entity))
            {
                var activeUpgrades = _entityManager.GetBuffer<ActiveUpgradesComponent>(entity);

                foreach (var upgrade in activeUpgrades)
                {
                    if (upgrade.Type == UpgradeType.MaxHealth)
                    {
                        health.ValueRW.MaxHealth = health.ValueRO.BaseMaxHealth + Mathf.RoundToInt(upgrade.Value);
                    }
                }
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
