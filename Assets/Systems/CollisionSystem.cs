using System.Collections.Generic;
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

        List<Entity> collidingCheckpoints = new List<Entity>();
        for (int i = 0; i < entitiesColliding.Length; i += 2)
        {
            Entity entityA = entitiesColliding[i];
            Entity entityB = entitiesColliding[i + 1];

            if (entityA == Entity.Null || entityB == Entity.Null)
            {
                continue;
            }

            if (IsCollisionEnemyWithPlayer(entityA, entityB))
            {
                Entity playerEntity = GetPlayerEntity(entityA, entityB);
                Entity monsterEntity = GetMonsterEntity(entityA, entityB);

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
                    ElapsedTime = 0,
                    isCheckpoint = false
                });

                if (_entityManager.HasComponent<VisualsReferenceComponent>(playerEntity))
                {
                    VisualsReferenceComponent visualsReferenceComponent = _entityManager.GetComponentData<VisualsReferenceComponent>(playerEntity);
                    visualsReferenceComponent.gameObject.GetComponent<Animator>().SetTrigger("Hit");
                }

                if (_entityManager.HasComponent<VisualsReferenceComponent>(monsterEntity))
                {
                    VisualsReferenceComponent visualsReferenceComponent = _entityManager.GetComponentData<VisualsReferenceComponent>(monsterEntity);
                    visualsReferenceComponent.gameObject.GetComponent<Animator>().SetTrigger("Attack");

                    CreateAudioEntity(entityCommandBuffer);
                }
            }

            if (IsCollisionExperienceWithPlayer(entityA, entityB))
            {
                Entity playerEntity = GetPlayerEntity(entityA, entityB);
                Entity experienceEntity = GetExperienceEntity(entityA, entityB);

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

            if (IsCollisionEnemyWithSpell(entityA, entityB))
            {
                Entity spellEntity = GetSpellEntity(entityA, entityB);
                Entity monsterEntity = GetMonsterEntity(entityA, entityB);
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

                int spellIncreaseByCurrentGameBuffsPercentage = 0;
                int lifeLeech = 0;
                int manaLeech = 0;
                if (SystemAPI.TryGetSingletonEntity<PlayerComponent>(out Entity playerEntity))
                {
                    if (_entityManager.HasComponent<ActiveUpgradesComponent>(playerEntity))
                    {
                        var activeUpgrades = _entityManager.GetBuffer<ActiveUpgradesComponent>(playerEntity);

                        foreach (var upgrade in activeUpgrades)
                        {
                            if (upgrade.Type == UpgradeType.FireDamage && spellElement.Element == Element.Fire)
                            {
                                spellIncreaseByCurrentGameBuffsPercentage += (int)upgrade.Value;
                            }

                            if (upgrade.Type == UpgradeType.WaterDamage && spellElement.Element == Element.Water)
                            {
                                spellIncreaseByCurrentGameBuffsPercentage += (int)upgrade.Value;
                            }

                            if (upgrade.Type == UpgradeType.EarthDamage && spellElement.Element == Element.Earth)
                            {
                                spellIncreaseByCurrentGameBuffsPercentage += (int)upgrade.Value;
                            }

                            if (upgrade.Type == UpgradeType.AirDamage && spellElement.Element == Element.Air)
                            {
                                spellIncreaseByCurrentGameBuffsPercentage += (int)upgrade.Value;
                            }

                            if (upgrade.Type == UpgradeType.Lifeleech)
                            {
                                lifeLeech = (int)upgrade.Value;
                            }

                            if (upgrade.Type == UpgradeType.Manaleech)
                            {
                                manaLeech = (int)upgrade.Value;
                            }
                        }
                    }
                }

                int damage = Mathf.RoundToInt(spellDamage.Damage * (1 + ((float)spellIncreasePercentage / 100)) * (1 + (spellIncreaseByCurrentGameBuffsPercentage / 100)));
                entityCommandBuffer.AddComponent(monsterEntity, new DamageComponent
                {
                    DamageAmount = damage
                });

                if (lifeLeech > 0)
                {
                    entityCommandBuffer.AddComponent(playerEntity, new HealthRestoreComponent
                    {
                        HealAmount = Mathf.RoundToInt(damage * (1 + ((float)lifeLeech / 100)))
                    });
                }

                if (manaLeech > 0)
                {
                    entityCommandBuffer.AddComponent(playerEntity, new ManaRestoreComponent
                    {
                        RestoreAmount = Mathf.RoundToInt(damage * (1 + ((float)manaLeech / 100)))
                    });
                }

                visualsReferenceComponent.gameObject.GetComponent<Animator>().SetTrigger("Hit");

                if (_entityManager.HasComponent<SpellSkillShotEntityComponent>(spellEntity))
                {
                    entityCommandBuffer.AddComponent<DestroySpellEntityComponent>(spellEntity);
                }
            }

            if (IsCollisionPlayerWithCheckpoint(entityA, entityB))
            {
                Entity playerEntity = GetPlayerEntity(entityA, entityB);
                Entity checkpointEntity = GetCheckpointEntity(entityA, entityB);

                MapCheckpointEntityComponent mapCheckpointEntityComponent = _entityManager.GetComponentData<MapCheckpointEntityComponent>(checkpointEntity);

                collidingCheckpoints.Add(checkpointEntity);

                if (!mapCheckpointEntityComponent.IsColliding)
                {
                    mapCheckpointEntityComponent.IsColliding = true;
                    entityCommandBuffer.SetComponent(checkpointEntity, mapCheckpointEntityComponent);

                    InvulnerableStateComponent playerInvulnerable = _entityManager.GetComponentData<InvulnerableStateComponent>(playerEntity);
                    playerInvulnerable.ElapsedTime = 0;
                    playerInvulnerable.Duration = 9999;
                    playerInvulnerable.isCheckpoint = true;
                    entityCommandBuffer.SetComponentEnabled<InvulnerableStateComponent>(playerEntity, true);
                    entityCommandBuffer.SetComponent(playerEntity, playerInvulnerable);
                }
            }
        }

        for (int i = 0; i < collidingCheckpoints.Count; i++)
        {
            Entity checkpointEntity = collidingCheckpoints[i];

            foreach (var (checkpoint, entity) in SystemAPI.Query<RefRW<MapCheckpointEntityComponent>>().WithEntityAccess())
            {
                if (entity == checkpointEntity)
                {
                    continue;
                }

                if (checkpoint.ValueRW.IsColliding)
                {
                    checkpoint.ValueRW.IsColliding = false;
                }
            }
        }

        if (collidingCheckpoints.Count == 0)
        {
            if (SystemAPI.TryGetSingletonEntity<PlayerComponent>(out Entity playerEntity))
            {
                if (_entityManager.IsComponentEnabled<InvulnerableStateComponent>(playerEntity))
                {
                    InvulnerableStateComponent playerInvulnerable = _entityManager.GetComponentData<InvulnerableStateComponent>(playerEntity);

                    if (playerInvulnerable.isCheckpoint)
                    {
                        playerInvulnerable.ElapsedTime = 0;
                        playerInvulnerable.Duration = 1;
                        playerInvulnerable.isCheckpoint = false;
                        entityCommandBuffer.SetComponentEnabled<InvulnerableStateComponent>(playerEntity, true);
                        entityCommandBuffer.SetComponent(playerEntity, playerInvulnerable);
                    }
                }
            }

            foreach (var checkpoint in SystemAPI.Query<RefRW<MapCheckpointEntityComponent>>())
            {
                if (checkpoint.ValueRW.IsColliding)
                {
                    checkpoint.ValueRW.IsColliding = false;
                }
            }
        }

        collidingCheckpoints.Clear();
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

    private bool IsCollisionPlayerWithCheckpoint(Entity entityA, Entity entityB)
    {
        return _entityManager.HasComponent<PlayerComponent>(entityA) && _entityManager.HasComponent<MapCheckpointEntityComponent>(entityB) ||
               _entityManager.HasComponent<PlayerComponent>(entityB) && _entityManager.HasComponent<MapCheckpointEntityComponent>(entityA);
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

    private Entity GetCheckpointEntity(Entity entityA, Entity entityB)
    {
        return _entityManager.HasComponent<MapCheckpointEntityComponent>(entityA) ? entityA : entityB;
    }

    private void CreateAudioEntity(EntityCommandBuffer entityCommandBuffer)
    {
        Entity audioEntity = entityCommandBuffer.CreateEntity();
        entityCommandBuffer.AddComponent(audioEntity, new AudioComponent
        {
            Volume = 1,
            Audio = AudioType.Claw
        });
    }
}
