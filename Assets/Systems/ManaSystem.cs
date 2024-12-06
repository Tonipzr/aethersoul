using System;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using UnityEngine;

partial struct ManaSystem : ISystem
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

        foreach (var (mana, entity) in SystemAPI.Query<RefRW<ManaComponent>>().WithEntityAccess())
        {
            if (_entityManager.HasComponent<ActiveUpgradesComponent>(entity))
            {
                var activeUpgrades = _entityManager.GetBuffer<ActiveUpgradesComponent>(entity);

                foreach (var upgrade in activeUpgrades)
                {
                    if (upgrade.Type == UpgradeType.MaxMana)
                    {
                        mana.ValueRW.MaxMana = mana.ValueRO.BaseMaxMana + Mathf.RoundToInt(upgrade.Value);
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
