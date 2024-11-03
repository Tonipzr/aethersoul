using Unity.Burst;
using Unity.Collections;
using Unity.Entities;

partial struct WeatherSystem : ISystem
{
    private EntityManager _entityManager;

    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        _entityManager = state.EntityManager;

        Entity entity = _entityManager.CreateEntity();
        _entityManager.AddComponent<WeatherEntityComponent>(entity);
        _entityManager.AddComponent<WeatherComponent>(entity);
        _entityManager.SetComponentData(entity, new WeatherComponent { Weather = WeatherType.Clear });
        _entityManager.AddComponent<TimeCounterComponent>(entity);
        _entityManager.SetComponentData(entity, new TimeCounterComponent { ElapsedTime = 0, EndTime = 120, isInfinite = false });
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        _entityManager = state.EntityManager;
        EntityCommandBuffer entityCommandBuffer = new EntityCommandBuffer(Allocator.Temp);

        Entity singletonEntity = SystemAPI.GetSingletonEntity<WeatherEntityComponent>();

        if (!SystemAPI.HasComponent<WeatherComponent>(singletonEntity))
        {
            return;
        }

        WeatherComponent weatherComponent = _entityManager.GetComponentData<WeatherComponent>(singletonEntity);

        foreach (var (_, _, entity) in SystemAPI.Query<RefRW<WeatherComponent>, RefRO<PlayerComponent>>().WithEntityAccess())
        {
            entityCommandBuffer.AddComponent(entity, new WeatherUpdated { NewWeather = weatherComponent.Weather });
        }

        entityCommandBuffer.Playback(_entityManager);
        entityCommandBuffer.Dispose();
    }

    [BurstCompile]
    public void OnDestroy(ref SystemState state)
    {

    }
}
