using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Physics.Extensions;
using Unity.Transforms;
using UnityEngine;

partial struct MoveSpellSkillShotSystem : ISystem
{
    public void OnCreate(ref SystemState state)
    {

    }

    public void OnUpdate(ref SystemState state)
    {
        EntityCommandBuffer entityCommandBuffer = new EntityCommandBuffer(Allocator.Temp);

        foreach (var (skillShot, velocity, transform, position, entity) in SystemAPI.Query<RefRW<SpellSkillShotEntityComponent>, RefRO<VelocityComponent>, RefRW<LocalTransform>, RefRW<PositionComponent>>().WithEntityAccess())
        {
            SpellEntityGameObjectReferenceComponent gameObjectReference = state.EntityManager.GetComponentData<SpellEntityGameObjectReferenceComponent>(entity);
            Vector3 currentPosition = new Vector3(position.ValueRO.Position.x, position.ValueRO.Position.y, 0);

            if (state.EntityManager.HasComponent(entity, typeof(DestroySpellEntityComponent)))
            {
                continue;
            }

            if (
                math.dot(new float2(skillShot.ValueRO.ToPosition.x, skillShot.ValueRO.ToPosition.y) - position.ValueRW.Position, new float2(skillShot.ValueRO.ToPosition.x, skillShot.ValueRO.ToPosition.y) - new float2(skillShot.ValueRO.FromPosition.x, skillShot.ValueRO.FromPosition.y)) < 0
            )
            {
                entityCommandBuffer.AddComponent<DestroySpellEntityComponent>(entity);
                continue;
            }

            Vector3 targetPosition = new Vector3(skillShot.ValueRO.ToPosition.x, skillShot.ValueRO.ToPosition.y, 0);

            PhysicsVelocity physicsVelocity = state.EntityManager.GetComponentData<PhysicsVelocity>(entity);
            PhysicsMass physicsMass = state.EntityManager.GetComponentData<PhysicsMass>(entity);

            Vector3 direction = (targetPosition - currentPosition).normalized;
            Vector3 forceVector = direction * velocity.ValueRO.Velocity * Time.deltaTime;

            physicsVelocity.ApplyLinearImpulse(physicsMass, forceVector);
            entityCommandBuffer.SetComponent(entity, physicsVelocity);
            position.ValueRW.Position = new float2(transform.ValueRO.Position.x, transform.ValueRO.Position.y);
            gameObjectReference.GameObject.transform.position = transform.ValueRO.Position;
        }

        entityCommandBuffer.Playback(state.EntityManager);
        entityCommandBuffer.Dispose();
    }

    public void OnDestroy(ref SystemState state)
    {

    }
}
