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

    public void OnCreate(ref SystemState state)
    {
        monsterArchetype = state.EntityManager.CreateArchetype(
                  typeof(LocalTransform),
                  typeof(PositionComponent),
                  typeof(VelocityComponent),
                  typeof(DirectionComponent),
                  typeof(MovementTypeComponent),
                  typeof(VisualsReferenceComponent),
                  typeof(HealthComponent),
                  typeof(LevelComponent),
                  typeof(ExperienceAfterDeathComponent),
                  typeof(MonsterComponent)
                );
    }

    public void OnUpdate(ref SystemState state)
    {
        _entityManager = state.EntityManager;

        EntityCommandBuffer entityCommandBuffer = new EntityCommandBuffer(Allocator.Temp);

        foreach (var (_, entity) in SystemAPI.Query<RefRO<DeathComponent>>().WithEntityAccess())
        {
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
                });

                _entityManager.SetComponentData(monsterEntity, new LevelComponent
                {
                    Level = spawnComponent.SpawnLevel
                });

                _entityManager.SetComponentData(monsterEntity, new ExperienceAfterDeathComponent
                {
                    ExperienceAfterDeath = MonsterExperiencePerLevel.CalculateExperience(spawnComponent.SpawnLevel, monsterType)
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
