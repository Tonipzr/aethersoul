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

                if (SystemAPI.HasComponent<WeatherComponent>(entity) && SystemAPI.HasComponent<WeatherEntityComponent>(entity))
                {
                    int random = UnityEngine.Random.Range(1, 3);

                    if (random == 1)
                    {
                        entityCommandBuffer.SetComponent(entity, new WeatherComponent { Weather = WeatherType.Clear });
                    }
                    else
                    {
                        entityCommandBuffer.SetComponent(entity, new WeatherComponent { Weather = WeatherType.Rain });
                    }

                    entityCommandBuffer.RemoveComponent<TimeCounterComponent>(entity);
                    entityCommandBuffer.AddComponent<TimeCounterComponent>(entity, new TimeCounterComponent { ElapsedTime = 0, EndTime = 120, isInfinite = false });

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
