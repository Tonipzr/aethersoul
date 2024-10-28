using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using UnityEngine;

partial struct DestroySpellEntitySystem : ISystem
{
    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {

    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        EntityCommandBuffer entityCommandBuffer = new EntityCommandBuffer(Allocator.Temp);

        foreach (var (_, entity) in SystemAPI.Query<RefRW<DestroySpellEntityComponent>>().WithEntityAccess())
        {
            SpellEntityGameObjectReferenceComponent gameObjectReference = state.EntityManager.GetComponentData<SpellEntityGameObjectReferenceComponent>(entity);

            entityCommandBuffer.DestroyEntity(entity);
            Object.Destroy(gameObjectReference.GameObject);
        }

        entityCommandBuffer.Playback(state.EntityManager);
        entityCommandBuffer.Dispose();
    }

    [BurstCompile]
    public void OnDestroy(ref SystemState state)
    {

    }
}
