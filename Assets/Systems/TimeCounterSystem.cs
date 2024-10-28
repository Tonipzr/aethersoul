using Unity.Burst;
using Unity.Collections;
using Unity.Entities;

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
