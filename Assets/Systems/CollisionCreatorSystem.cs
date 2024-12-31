using System.Collections.Generic;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Physics;
using UnityEngine;

partial struct CollisionCreatorSystem : ISystem
{
    private EntityManager _entityManager;

    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {

    }

    public void OnUpdate(ref SystemState state)
    {
        _entityManager = state.EntityManager;
        var entityCommandBuffer = new EntityCommandBuffer(Allocator.TempJob);

        foreach (var (collision, entity) in SystemAPI.Query<RefRW<CollisionComponent>>().WithEntityAccess())
        {
            if (collision.ValueRW.IsCreated)
            {
                continue;
            }

            var collider = new PhysicsCollider
            {
                Value = Unity.Physics.BoxCollider.Create(new BoxGeometry
                {
                    Center = new float3(0.5f, 0.6f, 0),
                    Size = new float3(0.5f, 1f, 0.5f),
                    Orientation = quaternion.identity,
                }, new CollisionFilter
                {
                    BelongsTo = 64,
                    CollidesWith = 3,
                    GroupIndex = 0
                }),
            };

            collider.Value.Value.SetCollisionResponse(CollisionResponsePolicy.Collide);
            entityCommandBuffer.AddComponent(entity, collider);

            // ADd Physics Body motion type static
            entityCommandBuffer.AddComponent(entity, new PhysicsMass
            {
                Transform = RigidTransform.identity,
                InverseInertia = float3.zero,
                InverseMass = 0,
            });

            collision.ValueRW.IsCreated = true;
        }

        entityCommandBuffer.Playback(_entityManager);
        entityCommandBuffer.Dispose();
    }

    [BurstCompile]
    public void OnDestroy(ref SystemState state)
    {

    }
}
