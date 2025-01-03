using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using UnityEngine;

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
    }

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

        Entity mapEntity = SystemAPI.GetSingletonEntity<MapEntityComponent>();
        MapEntityGameStateComponent mapEntityGameStateComponent = _entityManager.GetComponentData<MapEntityGameStateComponent>(mapEntity);

        if (
            (mapEntityGameStateComponent.GamePhase == GamePhase.PhaseBoss || mapEntityGameStateComponent.GamePhase == GamePhase.Phase2) &&
            weatherComponent.Weather != WeatherType.Rain
        )
        {
            weatherComponent.Weather = WeatherType.Rain;
            entityCommandBuffer.SetComponent(singletonEntity, weatherComponent);

            if (SystemAPI.TryGetSingletonEntity<LoreEntityComponent>(out Entity loreEntity))
            {
                DynamicBuffer<LoreEntityComponent> loreEntityComponent = _entityManager.GetBuffer<LoreEntityComponent>(loreEntity);

                loreEntityComponent.Add(new LoreEntityComponent
                {
                    Type = LoreType.Story,
                    Data = 1,
                    Data2 = 9
                });
            }
        }

        if (!SystemAPI.HasSingleton<PlayerComponent>())
        {
            return;
        }

        Entity playerEntity = SystemAPI.GetSingletonEntity<PlayerComponent>();
        VisualsReferenceComponent visualsReferenceComponent = _entityManager.GetComponentData<VisualsReferenceComponent>(playerEntity);
        Transform weatherTransform = visualsReferenceComponent.gameObject.transform.Find("Weather");

        if (weatherComponent.Weather == WeatherType.Rain)
        {
            Transform rain = weatherTransform.Find("Rain");
            rain.gameObject.SetActive(true);
        }

        if (weatherComponent.Weather == WeatherType.Clear)
        {
            Transform rain = weatherTransform.Find("Rain");
            rain.gameObject.SetActive(false);
        }

        entityCommandBuffer.Playback(_entityManager);
        entityCommandBuffer.Dispose();
    }

    [BurstCompile]
    public void OnDestroy(ref SystemState state)
    {

    }
}
