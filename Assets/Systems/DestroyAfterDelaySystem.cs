using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using UnityEngine;

partial struct DestroyAfterDelaySystem : ISystem
{
    private EntityManager _entityManager;

    public void OnCreate(ref SystemState state)
    {
    }

    public void OnUpdate(ref SystemState state)
    {
        _entityManager = state.EntityManager;
        float deltaTime = SystemAPI.Time.DeltaTime;

        EntityCommandBuffer entityCommandBuffer = new EntityCommandBuffer(Allocator.Temp);

        foreach (var (DestroyAfterDelayComponent, entity) in SystemAPI.Query<RefRW<DestroyAfterDelayComponent>>().WithEntityAccess())
        {
            DestroyAfterDelayComponent.ValueRW.ElapsedTime += deltaTime;

            if (DestroyAfterDelayComponent.ValueRW.ElapsedTime >= DestroyAfterDelayComponent.ValueRW.EndTime)
            {
                entityCommandBuffer.RemoveComponent<DestroyAfterDelayComponent>(entity);

                if (_entityManager.HasComponent<VisualsReferenceComponent>(entity))
                {
                    entityCommandBuffer.DestroyEntity(entity);

                    Object.Destroy(_entityManager.GetComponentData<VisualsReferenceComponent>(entity).gameObject);
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
