using System;
using System.Collections.Generic;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Entities.UniversalDelegates;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Transforms;
using UnityEngine;

public struct EntityPair : IEquatable<EntityPair>
{
    public Entity EntityA;
    public Entity EntityB;

    public EntityPair(Entity a, Entity b)
    {
        if (a.Index < b.Index)
        {
            EntityA = a;
            EntityB = b;
        }
        else
        {
            EntityA = b;
            EntityB = a;
        }
    }

    public bool Equals(EntityPair other)
    {
        return EntityA == other.EntityA && EntityB == other.EntityB;
    }

    public override int GetHashCode()
    {
        return EntityA.GetHashCode() ^ EntityB.GetHashCode();
    }
}

partial struct CollisionSystem : ISystem
{
    private EntityManager _entityManager;

    [BurstCompile]
    public partial struct CountNumTriggerEvents : ITriggerEventsJob
    {
        public Entity EntityA;
        public Entity EntityB;
        public NativeList<EntityPair>.ParallelWriter Collisions;

        [BurstCompile]
        public void Execute(TriggerEvent collisionEvent)
        {
            // UnityEngine.Debug.Log("Collision detected " + collisionEvent.EntityA.Index + " " + collisionEvent.EntityB.Index);
            var pair = new EntityPair(collisionEvent.EntityA, collisionEvent.EntityB);
            Collisions.AddNoResize(pair);
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
        NativeList<EntityPair> collisions = new NativeList<EntityPair>(1024, Allocator.TempJob);
        JobHandle jobHandle = new CountNumTriggerEvents
        {
            Collisions = collisions.AsParallelWriter()
        }.Schedule(simuilationSingleton, state.Dependency);
        jobHandle.Complete();

        HashSet<EntityPair> uniqueCollisions = new HashSet<EntityPair>();
        for (int i = 0; i < collisions.Length; i++)
        {
            uniqueCollisions.Add(collisions[i]);
        }

        List<Entity> collidingCheckpoints = new List<Entity>();
        foreach (var pair in uniqueCollisions)
        {
            Entity entityA = pair.EntityA;
            Entity entityB = pair.EntityB;

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
                    continue;
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

            if (IsCollisionExperienceAoEWithPlayer(entityA, entityB))
            {
                Entity playerEntity = GetPlayerEntity(entityA, entityB);
                Entity experienceEntity = GetExperienceAoEEntity(entityA, entityB);

                ExperienceShardPickUpAreaComponent experienceShardEntityComponent = _entityManager.GetComponentData<ExperienceShardPickUpAreaComponent>(experienceEntity);

                if (!_entityManager.Exists(experienceShardEntityComponent.Parent))
                {
                    entityCommandBuffer.AddComponent(experienceEntity, new DestroyAfterDelayComponent
                    {
                        ElapsedTime = 0,
                        EndTime = 0
                    });

                    continue;
                }

                entityCommandBuffer.AddComponent(experienceShardEntityComponent.Parent, new FollowComponent
                {
                    Target = playerEntity,
                    MinDistance = 0
                });

            }

            if (IsCollisionExperienceWithPlayer(entityA, entityB))
            {
                Entity playerEntity = GetPlayerEntity(entityA, entityB);
                Entity experienceEntity = GetExperienceEntity(entityA, entityB);

                ExperienceShardEntityComponent experienceShardEntityComponent = _entityManager.GetComponentData<ExperienceShardEntityComponent>(experienceEntity);

                if (experienceShardEntityComponent.IsProcessed)
                {
                    continue;
                }

                DynamicBuffer<ExperienceGainComponent> experienceGainComponent = _entityManager.GetBuffer<ExperienceGainComponent>(playerEntity);

                experienceGainComponent.Add(new ExperienceGainComponent
                {
                    ExperienceGain = experienceShardEntityComponent.ExperienceQuantity
                });

                entityCommandBuffer.AddComponent(experienceEntity, new DestroyAfterDelayComponent
                {
                    ElapsedTime = 0,
                    EndTime = 0
                });

                experienceShardEntityComponent.IsProcessed = true;
                entityCommandBuffer.SetComponent(experienceEntity, experienceShardEntityComponent);
            }

            if (IsCollisionEntityWithSpell(entityA, entityB))
            {
                Entity spellEntity = GetSpellEntity(entityA, entityB);

                Entity targetEntity = TargetIsMonsterEntity(entityA, entityB) ? GetMonsterEntity(entityA, entityB) : GetPlayerEntity(entityA, entityB);
                SpellDamageComponent spellDamage = _entityManager.GetComponentData<SpellDamageComponent>(spellEntity);
                SpellElementComponent spellElement = _entityManager.GetComponentData<SpellElementComponent>(spellEntity);

                if (_entityManager.IsComponentEnabled<InvulnerableStateComponent>(targetEntity))
                {
                    continue;
                }

                int damage = spellDamage.Damage;
                if (TargetIsMonsterEntity(entityA, entityB))
                {
                    MonsterComponent monsterComponent = _entityManager.GetComponentData<MonsterComponent>(targetEntity);

                    VisualsReferenceComponent visualsReferenceComponent = _entityManager.GetComponentData<VisualsReferenceComponent>(targetEntity);

                    if (SystemAPI.TryGetSingletonEntity<PlayerComponent>(out Entity playerEntity))
                    {
                        if (SystemAPI.IsComponentEnabled<InvulnerableStateComponent>(playerEntity))
                        {
                            InvulnerableStateComponent invulnerableStateComponent = _entityManager.GetComponentData<InvulnerableStateComponent>(playerEntity);

                            if (invulnerableStateComponent.isCheckpoint)
                            {
                                continue;
                            }
                        }

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

                                if (upgrade.Type == UpgradeType.KillAnyDamage)
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

                        damage = Mathf.RoundToInt(spellDamage.Damage * (1 + ((float)spellIncreasePercentage / 100)) * (1 + (spellIncreaseByCurrentGameBuffsPercentage / 100)));

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
                    }

                    visualsReferenceComponent.gameObject.GetComponent<Animator>().SetTrigger("Hit");

                    entityCommandBuffer.SetComponentEnabled<InvulnerableStateComponent>(targetEntity, true);
                    entityCommandBuffer.AddComponent(targetEntity, new InvulnerableStateComponent
                    {
                        Duration = 0.5f,
                        ElapsedTime = 0,
                        isCheckpoint = false
                    });
                }
                else
                {
                    entityCommandBuffer.SetComponentEnabled<InvulnerableStateComponent>(targetEntity, true);
                    entityCommandBuffer.AddComponent(targetEntity, new InvulnerableStateComponent
                    {
                        Duration = 1,
                        ElapsedTime = 0,
                        isCheckpoint = false
                    });

                    if (_entityManager.HasComponent<VisualsReferenceComponent>(targetEntity))
                    {
                        VisualsReferenceComponent visualsReferenceComponent = _entityManager.GetComponentData<VisualsReferenceComponent>(targetEntity);
                        visualsReferenceComponent.gameObject.GetComponent<Animator>().SetTrigger("Hit");
                    }
                }

                entityCommandBuffer.AddComponent(targetEntity, new DamageComponent
                {
                    DamageAmount = damage
                });

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

                if (!mapCheckpointEntityComponent.IsVisited)
                {
                    mapCheckpointEntityComponent.IsVisited = true;
                    entityCommandBuffer.SetComponent(checkpointEntity, mapCheckpointEntityComponent);

                    var jobCheckpoint = new UpdateMapStatsJob
                    {
                        Type = MapStatsType.CurrentCheckpointsReached,
                        Value = 1,
                        Incremental = true
                    };
                    jobCheckpoint.Schedule();
                }
            }

            if (IsCollisionPlayerWithPOIArea(entityA, entityB))
            {
                Entity poiEntity = GetPOIAreaEntity(entityA, entityB);

                if (SystemAPI.TryGetSingletonEntity<LoreEntityComponent>(out Entity loreEntity))
                {
                    DynamicBuffer<LoreEntityComponent> loreEntityComponent = _entityManager.GetBuffer<LoreEntityComponent>(loreEntity);
                    NightmareFragmentAreaComponent nightmareFragmentAreaComponent = _entityManager.GetComponentData<NightmareFragmentAreaComponent>(poiEntity);

                    NightmareFragmentComponent nightmareFragmentComponent = _entityManager.GetComponentData<NightmareFragmentComponent>(nightmareFragmentAreaComponent.Parent);

                    if (!nightmareFragmentComponent.IsVisited)
                    {
                        nightmareFragmentComponent.IsVisited = true;
                        entityCommandBuffer.SetComponent(nightmareFragmentAreaComponent.Parent, nightmareFragmentComponent);

                        var jobPOIsVisited = new UpdateMapStatsJob
                        {
                            Type = MapStatsType.CurrentPOIsVisited,
                            Value = 1,
                            Incremental = true
                        };
                        jobPOIsVisited.Schedule();
                    }

                    loreEntityComponent.Add(new LoreEntityComponent
                    {
                        Type = LoreType.Story,
                        Data = 1,
                        Data2 = 10
                    });

                    bool foundDelay = false;
                    foreach (var delayEntity in SystemAPI.Query<RefRO<LoreDelayEntityComponent>>())
                    {
                        if (delayEntity.ValueRO.DelayEntityID == poiEntity.Index)
                        {
                            foundDelay = true;
                            break;
                        }
                    }

                    if (!foundDelay)
                    {
                        loreEntityComponent.Add(new LoreEntityComponent
                        {
                            Type = LoreType.MapPosition,
                            Data = 3,
                            Data2 = (int)nightmareFragmentComponent.Type
                        });

                        Entity delayEntity = entityCommandBuffer.CreateEntity();
                        entityCommandBuffer.AddComponent(delayEntity, new LoreDelayEntityComponent
                        {
                            DelayEntityID = poiEntity.Index
                        });
                        entityCommandBuffer.AddComponent(delayEntity, new TimeCounterComponent
                        {
                            ElapsedTime = 0,
                            EndTime = 60
                        });
                    }
                }
            }

            if (IsCollisionPlayerWithPOIChest(entityA, entityB))
            {
                Entity poiChestEntity = GetPOIChestEntity(entityA, entityB);

                NightmareFragmentComponent nightmareFragmentComponent = _entityManager.GetComponentData<NightmareFragmentComponent>(poiChestEntity);

                if (nightmareFragmentComponent.IsCompleted || nightmareFragmentComponent.IsActive)
                {
                    continue;
                }

                if (SystemAPI.TryGetSingletonEntity<InputComponent>(out Entity inputEntity))
                {
                    InputComponent inputComponent = _entityManager.GetComponentData<InputComponent>(inputEntity);

                    if (inputComponent.pressingInteract)
                    {
                        nightmareFragmentComponent.IsActive = true;
                        nightmareFragmentComponent.StartedAtTime = Time.time;
                        entityCommandBuffer.SetComponent(poiChestEntity, nightmareFragmentComponent);
                    }
                }

                Entity interactEntity = entityCommandBuffer.CreateEntity();
                entityCommandBuffer.AddComponent(interactEntity, new UIInteractComponent());
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
        collisions.Dispose();
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

    [BurstCompile]
    private bool IsCollisionEntityWithSpell(Entity entityA, Entity entityB)
    {
        return (_entityManager.HasComponent<MonsterComponent>(entityA) || _entityManager.HasComponent<PlayerComponent>(entityA)) && (_entityManager.HasComponent<SpellAoEEntityComponent>(entityB) || _entityManager.HasComponent<SpellSkillShotEntityComponent>(entityB)) ||
               (_entityManager.HasComponent<MonsterComponent>(entityB) || _entityManager.HasComponent<PlayerComponent>(entityB)) && (_entityManager.HasComponent<SpellAoEEntityComponent>(entityA) || _entityManager.HasComponent<SpellSkillShotEntityComponent>(entityA));
    }

    [BurstCompile]
    private bool IsCollisionExperienceWithPlayer(Entity entityA, Entity entityB)
    {
        return _entityManager.HasComponent<ExperienceShardEntityComponent>(entityA) && _entityManager.HasComponent<PlayerComponent>(entityB) ||
               _entityManager.HasComponent<ExperienceShardEntityComponent>(entityB) && _entityManager.HasComponent<PlayerComponent>(entityA);
    }

    [BurstCompile]
    private bool IsCollisionExperienceAoEWithPlayer(Entity entityA, Entity entityB)
    {
        return _entityManager.HasComponent<ExperienceShardPickUpAreaComponent>(entityA) && _entityManager.HasComponent<PlayerComponent>(entityB) ||
               _entityManager.HasComponent<ExperienceShardPickUpAreaComponent>(entityB) && _entityManager.HasComponent<PlayerComponent>(entityA);
    }

    [BurstCompile]
    private bool IsCollisionPlayerWithCheckpoint(Entity entityA, Entity entityB)
    {
        return _entityManager.HasComponent<PlayerComponent>(entityA) && _entityManager.HasComponent<MapCheckpointEntityComponent>(entityB) ||
               _entityManager.HasComponent<PlayerComponent>(entityB) && _entityManager.HasComponent<MapCheckpointEntityComponent>(entityA);
    }

    [BurstCompile]
    private bool IsCollisionPlayerWithPOIArea(Entity entityA, Entity entityB)
    {
        return _entityManager.HasComponent<PlayerComponent>(entityA) && _entityManager.HasComponent<NightmareFragmentAreaComponent>(entityB) ||
               _entityManager.HasComponent<PlayerComponent>(entityB) && _entityManager.HasComponent<NightmareFragmentAreaComponent>(entityA);
    }

    [BurstCompile]
    private bool IsCollisionPlayerWithPOIChest(Entity entityA, Entity entityB)
    {
        return _entityManager.HasComponent<PlayerComponent>(entityA) && _entityManager.HasComponent<NightmareFragmentComponent>(entityB) ||
               _entityManager.HasComponent<PlayerComponent>(entityB) && _entityManager.HasComponent<NightmareFragmentComponent>(entityA);
    }

    [BurstCompile]
    private Entity GetSpellEntity(Entity entityA, Entity entityB)
    {
        return _entityManager.HasComponent<SpellAoEEntityComponent>(entityA) || _entityManager.HasComponent<SpellSkillShotEntityComponent>(entityA) ? entityA : entityB;
    }

    [BurstCompile]
    private Entity GetMonsterEntity(Entity entityA, Entity entityB)
    {
        return _entityManager.HasComponent<MonsterComponent>(entityA) ? entityA : entityB;
    }

    [BurstCompile]
    private bool TargetIsMonsterEntity(Entity entityA, Entity entityB)
    {
        return _entityManager.HasComponent<MonsterComponent>(entityA) || _entityManager.HasComponent<MonsterComponent>(entityB);
    }

    [BurstCompile]
    private Entity GetPlayerEntity(Entity entityA, Entity entityB)
    {
        return _entityManager.HasComponent<PlayerComponent>(entityA) ? entityA : entityB;
    }

    [BurstCompile]
    private Entity GetExperienceEntity(Entity entityA, Entity entityB)
    {
        return _entityManager.HasComponent<ExperienceShardEntityComponent>(entityA) ? entityA : entityB;
    }

    [BurstCompile]
    private Entity GetExperienceAoEEntity(Entity entityA, Entity entityB)
    {
        return _entityManager.HasComponent<ExperienceShardPickUpAreaComponent>(entityA) ? entityA : entityB;
    }

    [BurstCompile]
    private Entity GetCheckpointEntity(Entity entityA, Entity entityB)
    {
        return _entityManager.HasComponent<MapCheckpointEntityComponent>(entityA) ? entityA : entityB;
    }

    [BurstCompile]
    private Entity GetPOIAreaEntity(Entity entityA, Entity entityB)
    {
        return _entityManager.HasComponent<NightmareFragmentAreaComponent>(entityA) ? entityA : entityB;
    }

    [BurstCompile]
    private Entity GetPOIChestEntity(Entity entityA, Entity entityB)
    {
        return _entityManager.HasComponent<NightmareFragmentComponent>(entityA) ? entityA : entityB;
    }

    [BurstCompile]
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
