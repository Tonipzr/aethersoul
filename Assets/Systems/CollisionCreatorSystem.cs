using System.Collections.Generic;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Entities.UniversalDelegates;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Physics;
using Unity.VisualScripting;
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
            if (collision.ValueRO.IsCreated)
            {
                continue;
            }

            if (collision.ValueRO.Type == CollisionType.Tree)
            {
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

                entityCommandBuffer.AddComponent(entity, new PhysicsMass
                {
                    Transform = RigidTransform.identity,
                    InverseInertia = float3.zero,
                    InverseMass = 0,
                });
            }

            if (collision.ValueRO.Type == CollisionType.POI_Chest)
            {
                var colliderPhysic = Unity.Physics.BoxCollider.Create(new BoxGeometry
                {
                    Center = new float3(0.5f, 0.5f, 0),
                    Size = new float3(1f, 1f, 0.5f),
                    Orientation = quaternion.identity,
                }, new CollisionFilter
                {
                    BelongsTo = 96,
                    CollidesWith = 3,
                    GroupIndex = 0
                });
                var colliderInteract = Unity.Physics.BoxCollider.Create(new BoxGeometry
                {
                    Center = new float3(0.5f, 0.5f, 0),
                    Size = new float3(3f, 3f, 0.5f),
                    Orientation = quaternion.identity,
                }, new CollisionFilter
                {
                    BelongsTo = 32,
                    CollidesWith = 1,
                    GroupIndex = 0
                });

                colliderPhysic.Value.SetCollisionResponse(CollisionResponsePolicy.Collide);
                colliderInteract.Value.SetCollisionResponse(CollisionResponsePolicy.RaiseTriggerEvents);

                entityCommandBuffer.AddComponent(entity, new PhysicsCollider
                {
                    Value = CompoundCollider.Create(new NativeArray<CompoundCollider.ColliderBlobInstance>(2, Allocator.Temp)
                    {
                        [0] = new CompoundCollider.ColliderBlobInstance
                        {
                            CompoundFromChild = new RigidTransform
                            {
                                pos = new float3(0, 0, 0),
                                rot = quaternion.identity
                            },
                            Collider = colliderPhysic
                        },
                        [1] = new CompoundCollider.ColliderBlobInstance
                        {
                            CompoundFromChild = new RigidTransform
                            {
                                pos = new float3(0, 0, 0),
                                rot = quaternion.identity
                            },
                            Collider = colliderInteract
                        }
                    })
                });

                entityCommandBuffer.AddComponent(entity, new PhysicsMass
                {
                    Transform = RigidTransform.identity,
                    InverseInertia = float3.zero,
                    InverseMass = 0,
                });
            }

            if (collision.ValueRO.Type == CollisionType.POI_Pillar)
            {
                var collider = new PhysicsCollider
                {
                    Value = Unity.Physics.BoxCollider.Create(new BoxGeometry
                    {
                        Center = new float3(0.5f, 1f, 0),
                        Size = new float3(1f, 2f, 0.5f),
                        Orientation = quaternion.identity,
                    }, new CollisionFilter
                    {
                        BelongsTo = 96,
                        CollidesWith = 3,
                        GroupIndex = 0
                    }),
                };

                collider.Value.Value.SetCollisionResponse(CollisionResponsePolicy.Collide);
                entityCommandBuffer.AddComponent(entity, collider);

                entityCommandBuffer.AddComponent(entity, new PhysicsMass
                {
                    Transform = RigidTransform.identity,
                    InverseInertia = float3.zero,
                    InverseMass = 0,
                });
            }

            if (collision.ValueRO.Type == CollisionType.POI_Area)
            {
                var collider = new PhysicsCollider
                {
                    Value = Unity.Physics.BoxCollider.Create(new BoxGeometry
                    {
                        Center = new float3(0.5f, 0.5f, 0),
                        Size = new float3(17f, 18f, 0.5f),
                        Orientation = quaternion.identity,
                    }, new CollisionFilter
                    {
                        BelongsTo = 32,
                        CollidesWith = 1,
                        GroupIndex = 0
                    }),
                };

                collider.Value.Value.SetCollisionResponse(CollisionResponsePolicy.RaiseTriggerEvents);
                entityCommandBuffer.AddComponent(entity, collider);
            }

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
