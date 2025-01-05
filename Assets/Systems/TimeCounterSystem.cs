using System;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Entities.UniversalDelegates;

partial struct TimeCounterSystem : ISystem
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
        float deltaTime = SystemAPI.Time.DeltaTime;
        EntityCommandBuffer entityCommandBuffer = new EntityCommandBuffer(Allocator.Temp);

        foreach (var (timeCounter, entity) in SystemAPI.Query<RefRW<TimeCounterComponent>>().WithEntityAccess())
        {
            timeCounter.ValueRW.ElapsedTime += deltaTime;

            if (!timeCounter.ValueRW.isInfinite && timeCounter.ValueRW.ElapsedTime >= timeCounter.ValueRW.EndTime)
            {
                if (SystemAPI.HasComponent<SpellComponent>(entity))
                {
                    SystemAPI.SetComponentEnabled<SpellOnCooldownComponent>(entity, false);
                    entityCommandBuffer.SetComponentEnabled<SpellOnCooldownComponent>(entity, false);
                    entityCommandBuffer.RemoveComponent<TimeCounterComponent>(entity);
                }

                if (
                    SystemAPI.HasComponent<SpellAoEEntityComponent>(entity)
                )
                {
                    entityCommandBuffer.AddComponent<DestroySpellEntityComponent>(entity);
                }

                if (SystemAPI.HasComponent<SpawnPointComponent>(entity))
                {
                    entityCommandBuffer.RemoveComponent<TimeCounterComponent>(entity);
                    entityCommandBuffer.AddComponent<DeathComponent>(entity);
                }

                if (SystemAPI.HasComponent<LoreDelayEntityComponent>(entity))
                {
                    entityCommandBuffer.RemoveComponent<TimeCounterComponent>(entity);
                }
            }

            if (
                timeCounter.ValueRW.isInfinite &&
                SystemAPI.HasComponent<MapEntityComponent>(entity))
            {
                if (timeCounter.ValueRW.ElapsedTime >= 300 && timeCounter.ValueRO.ElapsedTime < 600)
                {
                    MapEntityGameStateComponent gameState = _entityManager.GetComponentData<MapEntityGameStateComponent>(entity);

                    if (gameState.GamePhase != GamePhase.Phase2)
                    {
                        gameState.GamePhase = GamePhase.Phase2;
                        entityCommandBuffer.SetComponent(entity, gameState);
                    }
                }

                if (timeCounter.ValueRW.ElapsedTime >= 450 && timeCounter.ValueRO.ElapsedTime < 600)
                {
                    if (SystemAPI.TryGetSingletonEntity<LoreEntityComponent>(out Entity loreEntity))
                    {
                        DynamicBuffer<LoreEntityComponent> loreEntityComponent = _entityManager.GetBuffer<LoreEntityComponent>(loreEntity);

                        loreEntityComponent.Add(new LoreEntityComponent
                        {
                            Type = LoreType.Story,
                            Data = 0,
                            Data2 = 14
                        });
                    }
                }

                if (timeCounter.ValueRO.ElapsedTime >= 600)
                {
                    MapEntityGameStateComponent gameState = _entityManager.GetComponentData<MapEntityGameStateComponent>(entity);

                    if (gameState.GamePhase != GamePhase.PhaseBoss && gameState.GamePhase != GamePhase.AfterBoss)
                    {
                        gameState.GamePhase = GamePhase.PhaseBoss;
                        entityCommandBuffer.SetComponent(entity, gameState);
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
