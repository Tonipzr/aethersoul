using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using UnityEngine;

partial struct WeatherUpdateSystem : ISystem
{
    private EntityManager _entityManager;

    public void OnCreate(ref SystemState state)
    {

    }

    public void OnUpdate(ref SystemState state)
    {
        _entityManager = state.EntityManager;
        EntityCommandBuffer entityCommandBuffer = new EntityCommandBuffer(Allocator.Temp);

        foreach (var (weather, wUpdated, _, entity) in SystemAPI.Query<RefRW<WeatherComponent>, RefRO<WeatherUpdated>, RefRO<PlayerComponent>>().WithEntityAccess())
        {
            entityCommandBuffer.RemoveComponent<WeatherUpdated>(entity);
            weather.ValueRW.Weather = wUpdated.ValueRO.NewWeather;

            if (!_entityManager.HasComponent<VisualsReferenceComponent>(entity))
            {
                continue;
            }

            VisualsReferenceComponent visualsReferenceComponent = _entityManager.GetComponentData<VisualsReferenceComponent>(entity);

            Transform weatherTransform = visualsReferenceComponent.gameObject.transform.Find("Weather");
            if (weather.ValueRO.Weather == WeatherType.Rain)
            {
                Transform rain = weatherTransform.Find("Rain");
                rain.gameObject.SetActive(true);
            }
            else
            {
                Transform rain = weatherTransform.Find("Rain");
                rain.gameObject.SetActive(false);
            }
        }

        entityCommandBuffer.Playback(_entityManager);
        entityCommandBuffer.Dispose();
    }

    public void OnDestroy(ref SystemState state)
    {

    }
}
