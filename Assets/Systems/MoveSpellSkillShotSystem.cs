using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using UnityEngine;

partial struct MoveSpellSkillShotSystem : ISystem
{
    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {

    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        EntityCommandBuffer entityCommandBuffer = new EntityCommandBuffer(Allocator.Temp);

        foreach (var (skillShot, velocity, entity) in SystemAPI.Query<RefRW<SpellSkillShotEntityComponent>, RefRO<VelocityComponent>>().WithEntityAccess())
        {
            SpellEntityGameObjectReferenceComponent gameObjectReference = state.EntityManager.GetComponentData<SpellEntityGameObjectReferenceComponent>(entity);
            Vector3 currentPosition = gameObjectReference.GameObject.transform.position;

            if (new Vector3(skillShot.ValueRW.ToPosition.x, skillShot.ValueRW.ToPosition.y, currentPosition.z) == currentPosition)
            {
                entityCommandBuffer.AddComponent<DestroySpellEntityComponent>(entity);
                continue;
            }

            Vector3 targetPosition = new Vector3(skillShot.ValueRW.ToPosition.x, skillShot.ValueRW.ToPosition.y, 0);

            gameObjectReference.GameObject.transform.position = Vector3.MoveTowards(currentPosition, targetPosition, velocity.ValueRO.Velocity * Time.deltaTime);
        }

        entityCommandBuffer.Playback(state.EntityManager);
        entityCommandBuffer.Dispose();
    }

    [BurstCompile]
    public void OnDestroy(ref SystemState state)
    {

    }
}
