using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Transforms;
using UnityEngine;

partial struct DeathSystem : ISystem
{
    private EntityManager _entityManager;
    private EntityArchetype monsterArchetype;
    private EntityArchetype experienceShardArchetype;
    private EntityArchetype experienceShardAoEArchetype;

    public void OnCreate(ref SystemState state)
    {
        monsterArchetype = state.EntityManager.CreateArchetype(
                  typeof(LocalTransform),
                  typeof(PhysicsCollider),
                  typeof(PhysicsDamping),
                  typeof(PhysicsGravityFactor),
                  typeof(PhysicsMass),
                  typeof(PhysicsVelocity),
                  typeof(PhysicsWorldIndex),
                  typeof(PositionComponent),
                  typeof(VelocityComponent),
                  typeof(DirectionComponent),
                  typeof(MovementTypeComponent),
                  typeof(VisualsReferenceComponent),
                  typeof(HealthComponent),
                  typeof(LevelComponent),
                  typeof(ExperienceAfterDeathComponent),
                  typeof(MonsterStatsComponent),
                  typeof(MonsterComponent),
                  typeof(MonsterNightmareFragmentComponent)
                );

        experienceShardArchetype = state.EntityManager.CreateArchetype(
            typeof(LocalTransform),
            typeof(PhysicsCollider),
            typeof(PhysicsDamping),
            typeof(PhysicsGravityFactor),
            typeof(PhysicsMass),
            typeof(PhysicsVelocity),
            typeof(PhysicsWorldIndex),
            typeof(PositionComponent),
            typeof(VelocityComponent),
            typeof(VisualsReferenceComponent),
            typeof(ExperienceShardEntityComponent)
        );

        experienceShardAoEArchetype = state.EntityManager.CreateArchetype(
            typeof(LocalTransform),
            typeof(PhysicsCollider),
            typeof(PhysicsDamping),
            typeof(PhysicsGravityFactor),
            typeof(PhysicsMass),
            typeof(PhysicsVelocity),
            typeof(PhysicsWorldIndex),
            typeof(PositionComponent),
            typeof(VelocityComponent),
            typeof(FollowComponent),
            typeof(ExperienceShardPickUpAreaComponent)
        );
    }

    public void OnUpdate(ref SystemState state)
    {
        _entityManager = state.EntityManager;

        EntityCommandBuffer entityCommandBuffer = new EntityCommandBuffer(Allocator.Temp);

        foreach (var (_, entity) in SystemAPI.Query<RefRO<DeathComponent>>().WithEntityAccess())
        {
            if (_entityManager.HasComponent<MonsterComponent>(entity) && !_entityManager.HasComponent<DestroyAfterDelayComponent>(entity))
            {
                entityCommandBuffer.AddComponent(entity, new DestroyAfterDelayComponent
                {
                    ElapsedTime = 0,
                    EndTime = 5
                });

                entityCommandBuffer.SetComponent(entity, new PhysicsVelocity
                {
                    Linear = float3.zero,
                    Angular = float3.zero
                });

                entityCommandBuffer.RemoveComponent<PhysicsCollider>(entity);

                entityCommandBuffer.RemoveComponent<DeathComponent>(entity);

                var jobAnyKilled = new UpdateMapStatsJob
                {
                    Type = MapStatsType.CurrentEnemiesKilled,
                    Value = 1,
                    Incremental = true
                };
                jobAnyKilled.Schedule();

                var jobNoDamageKilled = new UpdateMapStatsJob
                {
                    Type = MapStatsType.CurrentEnemiesKilledNoDamage,
                    Value = 1,
                    Incremental = true
                };
                jobNoDamageKilled.Schedule();

                if (_entityManager.HasComponent<VisualsReferenceComponent>(entity))
                {
                    VisualsReferenceComponent visualsReferenceComponent = _entityManager.GetComponentData<VisualsReferenceComponent>(entity);

                    visualsReferenceComponent.gameObject.GetComponent<Animator>().SetBool("Death", true);
                }

                Entity audioEntity = entityCommandBuffer.CreateEntity();
                entityCommandBuffer.AddComponent(audioEntity, new AudioComponent
                {
                    Volume = 1,
                    Audio = AudioType.Death
                });

                if (_entityManager.HasComponent<ExperienceAfterDeathComponent>(entity))
                {
                    if (!SystemAPI.ManagedAPI.TryGetSingleton(out AnimationVisualsPrefabs animationVisualsPrefabs))
                    {
                        continue;
                    }

                    ExperienceAfterDeathComponent experienceAfterDeathComponent = _entityManager.GetComponentData<ExperienceAfterDeathComponent>(entity);
                    PositionComponent positionComponent = _entityManager.GetComponentData<PositionComponent>(entity);

                    Entity shardEntity = _entityManager.CreateEntity(experienceShardArchetype);
                    Entity shardAoEEntity = _entityManager.CreateEntity(experienceShardAoEArchetype);

                    _entityManager.SetComponentData(shardEntity, new LocalTransform
                    {
                        Position = new float3(positionComponent.Position.x, positionComponent.Position.y, 0),
                        Rotation = quaternion.identity,
                        Scale = 1
                    });
                    _entityManager.SetComponentData(shardAoEEntity, new LocalTransform
                    {
                        Position = new float3(positionComponent.Position.x, positionComponent.Position.y, 0),
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

                    var colliderAoE = new PhysicsCollider
                    {
                        Value = Unity.Physics.SphereCollider.Create(new SphereGeometry
                        {
                            Center = new float3(0, 0, 0),
                            Radius = 1.5f,
                        }, new CollisionFilter
                        {
                            BelongsTo = 8,
                            CollidesWith = 1,
                            GroupIndex = 0
                        }),
                    };

                    colliderAoE.Value.Value.SetCollisionResponse(CollisionResponsePolicy.RaiseTriggerEvents);
                    _entityManager.SetComponentData(shardAoEEntity, colliderAoE);

                    _entityManager.SetComponentData(shardEntity, new PhysicsDamping
                    {
                        Linear = 0.01f,
                        Angular = 0.05f
                    });
                    _entityManager.SetComponentData(shardAoEEntity, new PhysicsDamping
                    {
                        Linear = 0.01f,
                        Angular = 0.05f
                    });

                    _entityManager.SetComponentData(shardEntity, new PhysicsGravityFactor
                    {
                        Value = 0
                    });
                    _entityManager.SetComponentData(shardAoEEntity, new PhysicsGravityFactor
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
                    _entityManager.SetComponentData(shardAoEEntity, new PhysicsMass
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
                    _entityManager.SetComponentData(shardAoEEntity, new PhysicsVelocity
                    {
                        Linear = new float3(0, 0, 0),
                        Angular = new float3(0, 0, 0)
                    });

                    _entityManager.SetComponentData(shardEntity, new PositionComponent
                    {
                        Position = new float2(positionComponent.Position.x, positionComponent.Position.y)
                    });
                    _entityManager.SetComponentData(shardEntity, new VelocityComponent
                    {
                        Velocity = 7
                    });
                    _entityManager.SetComponentData(shardAoEEntity, new PositionComponent
                    {
                        Position = new float2(positionComponent.Position.x, positionComponent.Position.y)
                    });
                    _entityManager.SetComponentData(shardAoEEntity, new VelocityComponent
                    {
                        Velocity = 7
                    });

                    GameObject shardVisuals = experienceAfterDeathComponent.ExperienceAfterDeath switch
                    {
                        < 30 => Object.Instantiate(animationVisualsPrefabs.GreenShardExperience, new Vector3(positionComponent.Position.x, positionComponent.Position.y, 0), Quaternion.identity),
                        < 60 => Object.Instantiate(animationVisualsPrefabs.BlueShardExperience, new Vector3(positionComponent.Position.x, positionComponent.Position.y, 0), Quaternion.identity),
                        < 90 => Object.Instantiate(animationVisualsPrefabs.PinkShardExperience, new Vector3(positionComponent.Position.x, positionComponent.Position.y, 0), Quaternion.identity),
                        < 120 => Object.Instantiate(animationVisualsPrefabs.PurpleShardExperience, new Vector3(positionComponent.Position.x, positionComponent.Position.y, 0), Quaternion.identity),
                        _ => Object.Instantiate(animationVisualsPrefabs.RedShardExperience, new Vector3(positionComponent.Position.x, positionComponent.Position.y, 0), Quaternion.identity),
                    };

                    _entityManager.SetComponentData(shardEntity, new VisualsReferenceComponent
                    {
                        gameObject = shardVisuals,
                    });

                    _entityManager.SetComponentData(shardEntity, new ExperienceShardEntityComponent
                    {
                        ExperienceQuantity = experienceAfterDeathComponent.ExperienceAfterDeath,
                        AoEPickUpRange = 0.5f
                    });
                    _entityManager.SetComponentData(shardAoEEntity, new ExperienceShardPickUpAreaComponent
                    {
                        Parent = shardEntity,
                    });

                    _entityManager.SetComponentData(shardAoEEntity, new FollowComponent
                    {
                        Target = shardEntity,
                        MinDistance = 0,
                        MaxDistance = 0
                    });
                }
            }

            if (SystemAPI.HasComponent<SpawnPointComponent>(entity))
            {
                entityCommandBuffer.DestroyEntity(entity);

                if (!SystemAPI.ManagedAPI.TryGetSingleton(out AnimationVisualsPrefabs animationVisualsPrefabs))
                {
                    continue;
                }

                PositionComponent spawnPosition = _entityManager.GetComponentData<PositionComponent>(entity);

                Entity monsterEntity = _entityManager.CreateEntity(monsterArchetype);
                SpawnPointComponent spawnComponent = _entityManager.GetComponentData<SpawnPointComponent>(entity);

                int scale = spawnComponent.Difficulty switch
                {
                    MonsterDifficulty.None => 2,
                    MonsterDifficulty.MiniBoss => 4,
                    MonsterDifficulty.Boss => 6,
                    _ => 2,
                };
                _entityManager.SetComponentData(monsterEntity, new LocalTransform
                {
                    Position = new float3(spawnPosition.Position.x, spawnPosition.Position.y, 0),
                    Rotation = quaternion.identity,
                    Scale = scale
                });

                var collider = Unity.Physics.BoxCollider.Create(new BoxGeometry
                {
                    Center = new float3(0, 0, 0),
                    Size = new float3(0.4f, 0.3f, 1),
                    Orientation = quaternion.identity,
                    BevelRadius = 0,
                }, new CollisionFilter
                {
                    BelongsTo = 2,
                    CollidesWith = 5,
                    GroupIndex = 0
                });
                var colliderPhysic = Unity.Physics.BoxCollider.Create(new BoxGeometry
                {
                    Center = new float3(0, 0, 0),
                    Size = new float3(0.4f, 0.3f, 1),
                    Orientation = quaternion.identity,
                    BevelRadius = 0,
                }, new CollisionFilter
                {
                    BelongsTo = 2,
                    CollidesWith = 66,
                    GroupIndex = 0
                });

                collider.Value.SetCollisionResponse(CollisionResponsePolicy.RaiseTriggerEvents);
                colliderPhysic.Value.SetCollisionResponse(CollisionResponsePolicy.Collide);

                _entityManager.SetComponentData(monsterEntity, new PhysicsCollider
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
                            Collider = collider
                        },
                        [1] = new CompoundCollider.ColliderBlobInstance
                        {
                            CompoundFromChild = new RigidTransform
                            {
                                pos = new float3(0, 0, 0),
                                rot = quaternion.identity
                            },
                            Collider = colliderPhysic
                        }
                    })
                });

                _entityManager.SetComponentData(monsterEntity, new PhysicsDamping
                {
                    Linear = 0.01f,
                    Angular = 0.05f
                });

                _entityManager.SetComponentData(monsterEntity, new PhysicsGravityFactor
                {
                    Value = 0
                });

                _entityManager.SetComponentData(monsterEntity, new PhysicsMass
                {
                    InverseInertia = new float3(0, 0, 0),
                    InverseMass = 1,
                    AngularExpansionFactor = 0,
                    InertiaOrientation = quaternion.identity
                });

                _entityManager.SetComponentData(monsterEntity, new PhysicsVelocity
                {
                    Linear = new float3(0, 0, 0),
                    Angular = new float3(0, 0, 0)
                });

                _entityManager.SetComponentData(monsterEntity, new PositionComponent
                {
                    Position = new float2(spawnPosition.Position.x, spawnPosition.Position.y)
                });

                GameObject monsterVisuals = spawnComponent.MonsterType switch
                {
                    MonsterType.Bat => Object.Instantiate(animationVisualsPrefabs.Bat, new Vector3(spawnPosition.Position.x, spawnPosition.Position.y, 0), Quaternion.identity),
                    MonsterType.Crab => Object.Instantiate(animationVisualsPrefabs.Crab, new Vector3(spawnPosition.Position.x, spawnPosition.Position.y, 0), Quaternion.identity),
                    MonsterType.Golem => Object.Instantiate(animationVisualsPrefabs.Golem, new Vector3(spawnPosition.Position.x, spawnPosition.Position.y, 0), Quaternion.identity),
                    MonsterType.Rat => Object.Instantiate(animationVisualsPrefabs.Rat, new Vector3(spawnPosition.Position.x, spawnPosition.Position.y, 0), Quaternion.identity),
                    MonsterType.Slime => Object.Instantiate(animationVisualsPrefabs.Slime, new Vector3(spawnPosition.Position.x, spawnPosition.Position.y, 0), Quaternion.identity),
                    _ => Object.Instantiate(animationVisualsPrefabs.Bat, new Vector3(spawnPosition.Position.x, spawnPosition.Position.y, 0), Quaternion.identity),
                };

                _entityManager.SetComponentData(monsterEntity, new VisualsReferenceComponent
                {
                    gameObject = monsterVisuals,
                });

                _entityManager.SetComponentData(monsterEntity, new VelocityComponent
                {
                    Velocity = 3 * (PlayerPrefsManager.Instance.GetMonsterSpeed() / 100f)
                });

                _entityManager.SetComponentData(monsterEntity, new DirectionComponent
                {
                    Direction = Direction.Right
                });

                _entityManager.SetComponentData(monsterEntity, new MovementTypeComponent
                {
                    MovementType = MovementType.AIControlled
                });

                _entityManager.SetComponentData(monsterEntity, new HealthComponent
                {
                    MaxHealth = MonsterHealthPerLevel.CalculateHealth(spawnComponent.SpawnLevel, spawnComponent.MonsterType),
                    CurrentHealth = MonsterHealthPerLevel.CalculateHealth(spawnComponent.SpawnLevel, spawnComponent.MonsterType),
                    BaseMaxHealth = MonsterHealthPerLevel.CalculateHealth(spawnComponent.SpawnLevel, spawnComponent.MonsterType)
                });

                _entityManager.SetComponentData(monsterEntity, new LevelComponent
                {
                    Level = spawnComponent.SpawnLevel
                });

                _entityManager.SetComponentData(monsterEntity, new ExperienceAfterDeathComponent
                {
                    ExperienceAfterDeath = MonsterExperiencePerLevel.CalculateExperience(spawnComponent.SpawnLevel, spawnComponent.MonsterType)
                });

                _entityManager.SetComponentData(monsterEntity, new MonsterStatsComponent
                {
                    Damage = 10,
                    Element = Element.Fire,
                });

                _entityManager.SetComponentData(monsterEntity, new MonsterComponent
                {
                    MonsterType = spawnComponent.MonsterType,
                    MonsterDifficulty = spawnComponent.Difficulty,
                });

                _entityManager.SetComponentData(monsterEntity, new MonsterNightmareFragmentComponent
                {
                    Parent = spawnComponent.Parent
                });
                _entityManager.SetComponentEnabled<MonsterNightmareFragmentComponent>(monsterEntity, spawnComponent.Type == SpawnType.NightmareFragment);
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
