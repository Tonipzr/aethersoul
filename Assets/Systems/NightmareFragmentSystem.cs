using System.Collections.Generic;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Transforms;
using UnityEngine;

partial struct NightmareFragmentSystem : ISystem
{
    private EntityManager _entityManager;

    private EntityArchetype experienceShardArchetype;

    public void OnCreate(ref SystemState state)
    {
        experienceShardArchetype = state.EntityManager.CreateArchetype(
            typeof(LocalTransform),
            typeof(PhysicsCollider),
            typeof(PhysicsDamping),
            typeof(PhysicsGravityFactor),
            typeof(PhysicsMass),
            typeof(PhysicsVelocity),
            typeof(PhysicsWorldIndex),
            typeof(PositionComponent),
            typeof(VisualsReferenceComponent),
            typeof(ExperienceShardEntityComponent)
        );
    }

    public void OnUpdate(ref SystemState state)
    {
        _entityManager = state.EntityManager;

        EntityCommandBuffer entityCommandBuffer = new EntityCommandBuffer(Allocator.Temp);

        foreach (var (nightmareFragment, entity) in SystemAPI.Query<RefRW<NightmareFragmentComponent>>().WithEntityAccess())
        {
            if (nightmareFragment.ValueRO.IsActive)
            {
                if (!SystemAPI.TryGetSingletonEntity<MapEntityComponent>(out Entity mapEntity)) return;
                MapEntityGameStateComponent gameState = _entityManager.GetComponentData<MapEntityGameStateComponent>(mapEntity);

                gameState.IsNightmareActive = true;
                _entityManager.SetComponentData(mapEntity, gameState);

                if (nightmareFragment.ValueRO.Type == NightmareFragmentType.Endless_Siege)
                {
                    bool alreadyExists = false;
                    foreach (var endlessSiegeComponent in SystemAPI.Query<RefRO<NightmareFragmentEndlessSiegeComponent>>())
                    {
                        if (endlessSiegeComponent.ValueRO.Parent == entity)
                        {
                            alreadyExists = true;
                            break;
                        }
                    }

                    if (alreadyExists) continue;

                    Entity endlessSiegeEntity = entityCommandBuffer.CreateEntity();
                    entityCommandBuffer.AddComponent(endlessSiegeEntity, new NightmareFragmentEndlessSiegeComponent
                    {
                        Round = 1,
                        Parent = entity
                    });
                }
            }
            else
            {
                if (!nightmareFragment.ValueRO.IsCompleted) nightmareFragment.ValueRW.RemainingEnemies = 999999;

                if (nightmareFragment.ValueRO.IsCompleted && !nightmareFragment.ValueRO.IsCompletedProcessed)
                {
                    nightmareFragment.ValueRW.IsCompletedProcessed = true;

                    if (!SystemAPI.ManagedAPI.TryGetSingleton(out AnimationVisualsPrefabs animationVisualsPrefabs))
                    {
                        continue;
                    }

                    if (!SystemAPI.TryGetSingletonEntity<PlayerComponent>(out Entity playerEntity)) continue;

                    PositionComponent positionComponent = _entityManager.GetComponentData<PositionComponent>(playerEntity);

                    NativeArray<float2> spawnPoints = new NativeArray<float2>(50, Allocator.Persistent);
                    for (int i = 0; i < 50; i++)
                    {
                        spawnPoints[i] = new float2(positionComponent.Position.x + UnityEngine.Random.Range(-5, 5), positionComponent.Position.y + UnityEngine.Random.Range(-5, 5));
                    }

                    for (int i = 0; i < spawnPoints.Length; i++)
                    {
                        Entity shardEntity = _entityManager.CreateEntity(experienceShardArchetype);

                        _entityManager.SetComponentData(shardEntity, new LocalTransform
                        {
                            Position = new float3(spawnPoints[i].x, spawnPoints[i].y, 0),
                            Rotation = quaternion.identity,
                            Scale = 1
                        });

                        var collider = new PhysicsCollider
                        {
                            Value = Unity.Physics.SphereCollider.Create(new SphereGeometry
                            {
                                Center = new float3(0, 0, 0),
                                Radius = 0.5f,
                            }, new CollisionFilter
                            {
                                BelongsTo = 8,
                                CollidesWith = 1,
                                GroupIndex = 0
                            }),
                        };

                        collider.Value.Value.SetCollisionResponse(CollisionResponsePolicy.RaiseTriggerEvents);
                        _entityManager.SetComponentData(shardEntity, collider);

                        _entityManager.SetComponentData(shardEntity, new PhysicsDamping
                        {
                            Linear = 0.01f,
                            Angular = 0.05f
                        });

                        _entityManager.SetComponentData(shardEntity, new PhysicsGravityFactor
                        {
                            Value = 0
                        });

                        _entityManager.SetComponentData(shardEntity, new PhysicsMass
                        {
                            InverseInertia = 6,
                            InverseMass = 1,
                            AngularExpansionFactor = 0,
                            InertiaOrientation = quaternion.identity,
                        });

                        _entityManager.SetComponentData(shardEntity, new PhysicsVelocity
                        {
                            Linear = new float3(0, 0, 0),
                            Angular = new float3(0, 0, 0)
                        });

                        _entityManager.SetComponentData(shardEntity, new PositionComponent
                        {
                            Position = new float2(spawnPoints[i].x, spawnPoints[i].y)
                        });

                        int randomExperience = UnityEngine.Random.Range(1, 7) * 10;

                        GameObject shardVisuals = randomExperience switch
                        {
                            < 30 => Object.Instantiate(animationVisualsPrefabs.GreenShardExperience, new Vector3(spawnPoints[i].x, spawnPoints[i].y, 0), Quaternion.identity),
                            < 60 => Object.Instantiate(animationVisualsPrefabs.BlueShardExperience, new Vector3(spawnPoints[i].x, spawnPoints[i].y, 0), Quaternion.identity),
                            < 90 => Object.Instantiate(animationVisualsPrefabs.PinkShardExperience, new Vector3(spawnPoints[i].x, spawnPoints[i].y, 0), Quaternion.identity),
                            < 120 => Object.Instantiate(animationVisualsPrefabs.PurpleShardExperience, new Vector3(spawnPoints[i].x, spawnPoints[i].y, 0), Quaternion.identity),
                            _ => Object.Instantiate(animationVisualsPrefabs.RedShardExperience, new Vector3(spawnPoints[i].x, spawnPoints[i].y, 0), Quaternion.identity),
                        };

                        _entityManager.SetComponentData(shardEntity, new VisualsReferenceComponent
                        {
                            gameObject = shardVisuals,
                        });

                        _entityManager.SetComponentData(shardEntity, new ExperienceShardEntityComponent
                        {
                            ExperienceQuantity = randomExperience,
                            AoEPickUpRange = 0.5f
                        });
                    }

                    spawnPoints.Dispose();
                }
            }
        }

        Dictionary<int, int> enemiesPerNightmare = new Dictionary<int, int>();
        foreach (var monsterNightmareFragment in SystemAPI.Query<RefRO<MonsterNightmareFragmentComponent>>())
        {
            NightmareFragmentComponent nightmareParent = _entityManager.GetComponentData<NightmareFragmentComponent>(monsterNightmareFragment.ValueRO.Parent);

            if (nightmareParent.IsActive)
            {
                if (!enemiesPerNightmare.ContainsKey(monsterNightmareFragment.ValueRO.Parent.Index))
                {
                    enemiesPerNightmare.Add(monsterNightmareFragment.ValueRO.Parent.Index, 1);
                }
                else
                {
                    enemiesPerNightmare[monsterNightmareFragment.ValueRO.Parent.Index]++;
                }
            }
        }

        foreach (var (nightmareFragment, entity) in SystemAPI.Query<RefRO<NightmareFragmentComponent>>().WithEntityAccess())
        {
            if (nightmareFragment.ValueRO.IsActive && nightmareFragment.ValueRO.StartedAtTime + 10 < Time.time)
            {
                NightmareFragmentComponent nightmareFragmentComponent = _entityManager.GetComponentData<NightmareFragmentComponent>(entity);

                if (enemiesPerNightmare.ContainsKey(entity.Index))
                {
                    nightmareFragmentComponent.IsCompleted = false;
                    nightmareFragmentComponent.RemainingEnemies = enemiesPerNightmare[entity.Index];
                    _entityManager.SetComponentData(entity, nightmareFragmentComponent);
                }
                else
                {
                    nightmareFragmentComponent.RemainingEnemies = 0;
                    nightmareFragmentComponent.IsCompleted = true;
                    nightmareFragmentComponent.IsActive = false;
                    _entityManager.SetComponentData(entity, nightmareFragmentComponent);

                    var jobPOIsCleared = new UpdateMapStatsJob
                    {
                        Type = MapStatsType.CurrentPOIsCleared,
                        Value = 1,
                        Incremental = true
                    };
                    jobPOIsCleared.Schedule();

                    if (!SystemAPI.TryGetSingletonEntity<MapEntityComponent>(out Entity mapEntity)) continue;
                    MapEntityGameStateComponent gameState = _entityManager.GetComponentData<MapEntityGameStateComponent>(mapEntity);

                    gameState.IsNightmareActive = false;
                    _entityManager.SetComponentData(mapEntity, gameState);
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
