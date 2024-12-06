using System;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;

partial struct ManaRestoreSystem : ISystem
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

        foreach (var (restore, mana, entity) in SystemAPI.Query<RefRW<ManaRestoreComponent>, RefRW<ManaComponent>>().WithEntityAccess())
        {
            if (restore.ValueRO.RestoreAmount > 0)
            {
                entityCommandBuffer.RemoveComponent<ManaRestoreComponent>(entity);
                entityCommandBuffer.AddComponent(entity, new ManaUpdatedComponent
                {
                    CurrentMana = Math.Min(mana.ValueRO.MaxMana, mana.ValueRO.CurrentMana + restore.ValueRO.RestoreAmount),
                    MaxMana = mana.ValueRO.MaxMana,
                });

                mana.ValueRW.CurrentMana = Math.Min(mana.ValueRO.MaxMana, mana.ValueRO.CurrentMana + restore.ValueRO.RestoreAmount);
            }

            if (restore.ValueRO.RestoreAmount < 0)
            {
                entityCommandBuffer.RemoveComponent<ManaRestoreComponent>(entity);
                entityCommandBuffer.AddComponent(entity, new ManaUpdatedComponent
                {
                    CurrentMana = Math.Max(0, mana.ValueRO.CurrentMana + restore.ValueRO.RestoreAmount),
                    MaxMana = mana.ValueRO.MaxMana,
                });

                mana.ValueRW.CurrentMana = Math.Max(0, mana.ValueRO.CurrentMana + restore.ValueRO.RestoreAmount);
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
                        if (upgrade.Type == UpgradeType.ManaRegen)
                        {
                            restoreQuantity = (int)(restoreQuantity * (1 + (upgrade.Value / 100)));
                        }
                    }
                }

                entityCommandBuffer.AddComponent(playerEntity, new ManaRestoreComponent
                {
                    RestoreAmount = restoreQuantity
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
