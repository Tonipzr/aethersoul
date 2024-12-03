using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Physics;
using UnityEngine;

partial struct CollisionSystem : ISystem
{
    private EntityManager _entityManager;

    [BurstCompile]
    public partial struct CountNumTriggerEvents : ITriggerEventsJob
    {
        public Entity EntityA;
        public Entity EntityB;
        public NativeList<Entity> entitiesColliding;

        [BurstCompile]
        public void Execute(TriggerEvent collisionEvent)
        {
            // UnityEngine.Debug.Log("Collision detected " + collisionEvent.EntityA.Index + " " + collisionEvent.EntityB.Index);
            Entity entityA = collisionEvent.EntityA;
            Entity entityB = collisionEvent.EntityB;

            entitiesColliding.Add(entityA);
            entitiesColliding.Add(entityB);
        }
    }

    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {

    }

    public void OnUpdate(ref SystemState state)
    {
        _entityManager = state.EntityManager;

        EntityCommandBuffer entityCommandBuffer = new EntityCommandBuffer(Allocator.Temp);

        var simuilationSingleton = SystemAPI.GetSingleton<SimulationSingleton>();
        NativeList<Entity> entitiesColliding = new NativeList<Entity>(Allocator.TempJob);
        JobHandle jobHandle = new CountNumTriggerEvents
        {
            entitiesColliding = entitiesColliding
        }.Schedule(simuilationSingleton, state.Dependency);
        jobHandle.Complete();


        for (int i = 0; i < entitiesColliding.Length; i += 2)
        {
            Entity entityA = entitiesColliding[i];
            Entity entityB = entitiesColliding[i + 1];

            if (entityA == Entity.Null || entityB == Entity.Null)
            {
                continue;
            }

            if (IsCollisionEnemyWithPlayer(entitiesColliding[0], entitiesColliding[1]))
            {
                Entity playerEntity = GetPlayerEntity(entitiesColliding[0], entitiesColliding[1]);
                Entity monsterEntity = GetMonsterEntity(entitiesColliding[0], entitiesColliding[1]);

                if (
                    _entityManager.HasComponent<DeathComponent>(monsterEntity) ||
                    _entityManager.IsComponentEnabled<InvulnerableStateComponent>(playerEntity)
                )
                {
                    return;
                }

                MonsterStatsComponent monsterStatsComponent = _entityManager.GetComponentData<MonsterStatsComponent>(monsterEntity);

                entityCommandBuffer.AddComponent(playerEntity, new DamageComponent
                {
                    DamageAmount = monsterStatsComponent.Damage
                });

                entityCommandBuffer.SetComponentEnabled<InvulnerableStateComponent>(playerEntity, true);
                entityCommandBuffer.AddComponent(playerEntity, new InvulnerableStateComponent
                {
                    Duration = 1,
                    ElapsedTime = 0
                });
            }

            if (IsCollisionExperienceWithPlayer(entitiesColliding[0], entitiesColliding[1]))
            {
                Entity playerEntity = GetPlayerEntity(entitiesColliding[0], entitiesColliding[1]);
                Entity experienceEntity = GetExperienceEntity(entitiesColliding[0], entitiesColliding[1]);

                ExperienceShardEntityComponent experienceShardEntityComponent = _entityManager.GetComponentData<ExperienceShardEntityComponent>(experienceEntity);

                entityCommandBuffer.AddComponent(playerEntity, new ExperienceGainComponent
                {
                    ExperienceGain = experienceShardEntityComponent.ExperienceQuantity
                });

                entityCommandBuffer.AddComponent(experienceEntity, new DestroyAfterDelayComponent
                {
                    ElapsedTime = 0,
                    EndTime = 0
                });
            }

            if (IsCollisionEnemyWithSpell(entitiesColliding[0], entitiesColliding[1]))
            {
                Entity spellEntity = GetSpellEntity(entitiesColliding[0], entitiesColliding[1]);
                Entity monsterEntity = GetMonsterEntity(entitiesColliding[0], entitiesColliding[1]);
                SpellDamageComponent spellDamage = _entityManager.GetComponentData<SpellDamageComponent>(spellEntity);
                SpellElementComponent spellElement = _entityManager.GetComponentData<SpellElementComponent>(spellEntity);
                MonsterComponent monsterComponent = _entityManager.GetComponentData<MonsterComponent>(monsterEntity);

                VisualsReferenceComponent visualsReferenceComponent = _entityManager.GetComponentData<VisualsReferenceComponent>(monsterEntity);

                int spellIncreasePercentage = spellElement.Element switch
                {
                    Element.Fire => DreamCityStatsGameObject.FireBuff,
                    Element.Water => DreamCityStatsGameObject.WaterBuff,
                    Element.Earth => DreamCityStatsGameObject.EarthBuff,
                    Element.Air => DreamCityStatsGameObject.AirBuff,
                    _ => 0
                };

                entityCommandBuffer.AddComponent(monsterEntity, new DamageComponent
                {
                    DamageAmount = spellDamage.Damage + Mathf.RoundToInt(spellDamage.Damage * spellIncreasePercentage / 100)
                });

                visualsReferenceComponent.gameObject.GetComponent<Animator>().SetTrigger("Hit");

                if (_entityManager.HasComponent<SpellSkillShotEntityComponent>(spellEntity))
                {
                    entityCommandBuffer.AddComponent<DestroySpellEntityComponent>(spellEntity);
                }
            }
        }

        entitiesColliding.Dispose();
        entityCommandBuffer.Playback(_entityManager);
        entityCommandBuffer.Dispose();
    }

    [BurstCompile]
    public void OnDestroy(ref SystemState state)
    {

    }

    [BurstCompile]
    private bool IsCollisionEnemyWithPlayer(Entity entityA, Entity entityB)
    {
        return _entityManager.HasComponent<MonsterComponent>(entityA) && _entityManager.HasComponent<PlayerComponent>(entityB) ||
               _entityManager.HasComponent<MonsterComponent>(entityB) && _entityManager.HasComponent<PlayerComponent>(entityA);
    }

    private bool IsCollisionEnemyWithSpell(Entity entityA, Entity entityB)
    {
        return _entityManager.HasComponent<MonsterComponent>(entityA) && (_entityManager.HasComponent<SpellAoEEntityComponent>(entityB) || _entityManager.HasComponent<SpellSkillShotEntityComponent>(entityB)) ||
               _entityManager.HasComponent<MonsterComponent>(entityB) && (_entityManager.HasComponent<SpellAoEEntityComponent>(entityA) || _entityManager.HasComponent<SpellSkillShotEntityComponent>(entityA));
    }

    private bool IsCollisionExperienceWithPlayer(Entity entityA, Entity entityB)
    {
        return _entityManager.HasComponent<ExperienceShardEntityComponent>(entityA) && _entityManager.HasComponent<PlayerComponent>(entityB) ||
               _entityManager.HasComponent<ExperienceShardEntityComponent>(entityB) && _entityManager.HasComponent<PlayerComponent>(entityA);
    }

    private Entity GetSpellEntity(Entity entityA, Entity entityB)
    {
        return _entityManager.HasComponent<SpellAoEEntityComponent>(entityA) || _entityManager.HasComponent<SpellSkillShotEntityComponent>(entityA) ? entityA : entityB;
    }

    private Entity GetMonsterEntity(Entity entityA, Entity entityB)
    {
        return _entityManager.HasComponent<MonsterComponent>(entityA) ? entityA : entityB;
    }

    private Entity GetPlayerEntity(Entity entityA, Entity entityB)
    {
        return _entityManager.HasComponent<PlayerComponent>(entityA) ? entityA : entityB;
    }

    private Entity GetExperienceEntity(Entity entityA, Entity entityB)
    {
        return _entityManager.HasComponent<ExperienceShardEntityComponent>(entityA) ? entityA : entityB;
    }
}
