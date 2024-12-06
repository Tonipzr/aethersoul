using System.Collections;
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
                  typeof(MonsterComponent)
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
            typeof(VisualsReferenceComponent),
            typeof(ExperienceShardEntityComponent)
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

                entityCommandBuffer.RemoveComponent<DeathComponent>(entity);

                if (_entityManager.HasComponent<VisualsReferenceComponent>(entity))
                {
                    VisualsReferenceComponent visualsReferenceComponent = _entityManager.GetComponentData<VisualsReferenceComponent>(entity);

                    visualsReferenceComponent.gameObject.GetComponent<Animator>().SetBool("Death", true);
                }

                if (_entityManager.HasComponent<ExperienceAfterDeathComponent>(entity))
                {
                    if (!SystemAPI.ManagedAPI.TryGetSingleton(out AnimationVisualsPrefabs animationVisualsPrefabs))
                    {
                        continue;
                    }

                    ExperienceAfterDeathComponent experienceAfterDeathComponent = _entityManager.GetComponentData<ExperienceAfterDeathComponent>(entity);
                    PositionComponent positionComponent = _entityManager.GetComponentData<PositionComponent>(entity);

                    Entity shardEntity = _entityManager.CreateEntity(experienceShardArchetype);

                    _entityManager.SetComponentData(shardEntity, new LocalTransform
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
                        Position = new float2(positionComponent.Position.x, positionComponent.Position.y)
                    });

                    _entityManager.SetComponentData(shardEntity, new VisualsReferenceComponent
                    {
                        gameObject = Object.Instantiate(animationVisualsPrefabs.GreenShardExperience, new Vector3(positionComponent.Position.x, positionComponent.Position.y, 0), Quaternion.identity),
                    });

                    _entityManager.SetComponentData(shardEntity, new ExperienceShardEntityComponent
                    {
                        ExperienceQuantity = experienceAfterDeathComponent.ExperienceAfterDeath,
                        AoEPickUpRange = 0.5f
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
                MonsterType monsterType = getMonsterTypeByLevel(spawnComponent.SpawnLevel);

                _entityManager.SetComponentData(monsterEntity, new LocalTransform
                {
                    Position = new float3(spawnPosition.Position.x, spawnPosition.Position.y, 0),
                    Rotation = quaternion.identity,
                    Scale = 2
                });

                var collider = new PhysicsCollider
                {
                    Value = Unity.Physics.BoxCollider.Create(new BoxGeometry
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
                    }),
                };
                collider.Value.Value.SetCollisionResponse(CollisionResponsePolicy.RaiseTriggerEvents);
                _entityManager.SetComponentData(monsterEntity, collider);

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
                    InverseInertia = 6,
                    InverseMass = 1,
                    AngularExpansionFactor = 0,
                    InertiaOrientation = quaternion.identity,
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

                GameObject monsterVisuals = monsterType switch
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
                    Velocity = 3
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
                    MaxHealth = MonsterHealthPerLevel.CalculateHealth(spawnComponent.SpawnLevel, monsterType),
                    CurrentHealth = MonsterHealthPerLevel.CalculateHealth(spawnComponent.SpawnLevel, monsterType),
                    BaseMaxHealth = MonsterHealthPerLevel.CalculateHealth(spawnComponent.SpawnLevel, monsterType)
                });

                _entityManager.SetComponentData(monsterEntity, new LevelComponent
                {
                    Level = spawnComponent.SpawnLevel
                });

                _entityManager.SetComponentData(monsterEntity, new ExperienceAfterDeathComponent
                {
                    ExperienceAfterDeath = MonsterExperiencePerLevel.CalculateExperience(spawnComponent.SpawnLevel, monsterType)
                });

                _entityManager.SetComponentData(monsterEntity, new MonsterStatsComponent
                {
                    Damage = 10,
                    Element = Element.Fire,
                });

                _entityManager.SetComponentData(monsterEntity, new MonsterComponent
                {
                    MonsterType = monsterType
                });
            }
        }

        entityCommandBuffer.Playback(_entityManager);
        entityCommandBuffer.Dispose();
    }

    [BurstCompile]
    public void OnDestroy(ref SystemState state)
    {

    }

    [BurstCompile]
    private MonsterType getMonsterTypeByLevel(int level)
    {
        if (level <= 5) return MonsterType.Bat;
        if (level <= 10) return MonsterType.Crab;
        if (level <= 15) return MonsterType.Rat;
        if (level <= 20) return MonsterType.Slime;
        return MonsterType.Golem;
    }
}
