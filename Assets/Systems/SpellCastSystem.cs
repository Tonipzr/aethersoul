using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Transforms;
using Unity.VisualScripting;
using UnityEngine;

partial struct SpellCastSystem : ISystem
{
    private EntityManager _entityManager;

    public void OnCreate(ref SystemState state)
    {

    }

    public void OnUpdate(ref SystemState state)
    {
        _entityManager = state.EntityManager;
        EntityCommandBuffer entityCommandBuffer = new EntityCommandBuffer(Allocator.Temp);

        if (SystemAPI.TryGetSingletonEntity<MapEntityComponent>(out Entity mapEntity))
        {
            MapEntityGameStateComponent mapGameState = _entityManager.GetComponentData<MapEntityGameStateComponent>(mapEntity);

            if (mapGameState.IsPaused) return;
        }


        if (SystemAPI.TryGetSingletonEntity<PlayerComponent>(out Entity playerEntity))
        {
            DynamicBuffer<SelectedSpellsComponent> selectedSpellsBuffer = _entityManager.GetBuffer<SelectedSpellsComponent>(playerEntity);

            if (selectedSpellsBuffer.Length > 0)
            {
                Entity inputEntity = SystemAPI.GetSingletonEntity<InputComponent>();
                InputComponent inputComponent = _entityManager.GetComponentData<InputComponent>(inputEntity);

                if (inputComponent.pressingSpellSlot1 && selectedSpellsBuffer.Length >= 1)
                {
                    if (CastSpell(selectedSpellsBuffer[0].SpellID, playerEntity, ref state, entityCommandBuffer))
                        entityCommandBuffer.AppendToBuffer(playerEntity, new CastAttemptComponent { SpellID = selectedSpellsBuffer[0].SpellID });
                }

                if (inputComponent.pressingSpellSlot2 && selectedSpellsBuffer.Length >= 2)
                {
                    if (CastSpell(selectedSpellsBuffer[1].SpellID, playerEntity, ref state, entityCommandBuffer))
                        entityCommandBuffer.AppendToBuffer(playerEntity, new CastAttemptComponent { SpellID = selectedSpellsBuffer[1].SpellID });
                }

                if (inputComponent.pressingSpellSlot3 && selectedSpellsBuffer.Length >= 3)
                {
                    if (CastSpell(selectedSpellsBuffer[2].SpellID, playerEntity, ref state, entityCommandBuffer))
                        entityCommandBuffer.AppendToBuffer(playerEntity, new CastAttemptComponent { SpellID = selectedSpellsBuffer[2].SpellID });
                }

                if (inputComponent.pressingSpellSlot4 && selectedSpellsBuffer.Length >= 4)
                {
                    if (CastSpell(selectedSpellsBuffer[3].SpellID, playerEntity, ref state, entityCommandBuffer))
                        entityCommandBuffer.AppendToBuffer(playerEntity, new CastAttemptComponent { SpellID = selectedSpellsBuffer[3].SpellID });
                }
            }
        }

        if (SystemAPI.TryGetSingletonEntity<BossComponent>(out Entity bossEntity))
        {
            DynamicBuffer<SelectedSpellsComponent> selectedSpellsBuffer = _entityManager.GetBuffer<SelectedSpellsComponent>(bossEntity);

            if (selectedSpellsBuffer.Length > 0)
            {
                PositionComponent playerPosition = _entityManager.GetComponentData<PositionComponent>(SystemAPI.GetSingletonEntity<PlayerComponent>());
                PositionComponent bossPosition = _entityManager.GetComponentData<PositionComponent>(bossEntity);

                for (int i = 0; i < selectedSpellsBuffer.Length; i++)
                {
                    if (selectedSpellsBuffer[i].SpellID == 7 && math.distance(playerPosition.Position, bossPosition.Position) < 5)
                    {
                        if (CastSpell(selectedSpellsBuffer[i].SpellID, bossEntity, ref state, entityCommandBuffer))
                            entityCommandBuffer.AppendToBuffer(bossEntity, new CastAttemptComponent { SpellID = selectedSpellsBuffer[i].SpellID });
                    }

                    if (selectedSpellsBuffer[i].SpellID != 7)
                    {
                        if (CastSpell(selectedSpellsBuffer[i].SpellID, bossEntity, ref state, entityCommandBuffer))
                            entityCommandBuffer.AppendToBuffer(bossEntity, new CastAttemptComponent { SpellID = selectedSpellsBuffer[i].SpellID });
                    }
                }
            }
        }

        foreach (var (castAttempt, entity) in SystemAPI.Query<DynamicBuffer<CastAttemptComponent>>().WithEntityAccess())
        {
            if (castAttempt.Length == 0)
            {
                continue;
            }

            for (int i = 0; i < castAttempt.Length; i++)
            {
                foreach (var (spell, spellTarget, spellEntity) in SystemAPI.Query<RefRO<SpellComponent>, RefRO<SpellTargetComponent>>().WithEntityAccess())
                {
                    if (spell.ValueRO.SpellID == castAttempt[i].SpellID)
                    {
                        if (spellTarget.ValueRO.Target == SpellTarget.MousePosition) CastMousePositionSpell(entity, spellEntity, entityCommandBuffer);
                        if (spellTarget.ValueRO.Target == SpellTarget.Self) CastSelfSpell(entity, spellEntity, entityCommandBuffer);
                        if (spellTarget.ValueRO.Target == SpellTarget.SelfBoss) CastSelfBossSpell(entity, spellEntity, entityCommandBuffer);
                        if (spellTarget.ValueRO.Target == SpellTarget.RandomAroundTarget) CastRandomAroundTarget(entity, spellEntity, entityCommandBuffer);

                        RemoveMana(entity, spellEntity, entityCommandBuffer);

                        if (_entityManager.HasComponent<PlayerComponent>(entity))
                        {
                            var job = new UpdateMapStatsJob
                            {
                                Type = MapStatsType.CurrentSpellsUsed,
                                Value = 1,
                                Incremental = true
                            };
                            job.Schedule();
                        }

                        if (_entityManager.HasComponent<SpellElementComponent>(spellEntity))
                        {
                            SpellElementComponent spellElement = _entityManager.GetComponentData<SpellElementComponent>(spellEntity);
                            Entity audioEntity = entityCommandBuffer.CreateEntity();

                            AudioType audiotype = spellElement.Element switch
                            {
                                Element.Fire => AudioType.FireSpell,
                                Element.Air => AudioType.WindSpell,
                                _ => AudioType.FireSpell
                            };

                            entityCommandBuffer.AddComponent(audioEntity, new AudioComponent
                            {
                                Volume = 1,
                                Audio = audiotype
                            });

                            if (_entityManager.HasComponent<PlayerComponent>(entity) && spellElement.Element == Element.Fire)
                            {
                                if (SystemAPI.TryGetSingletonEntity<LoreEntityComponent>(out Entity loreEntity))
                                {
                                    DynamicBuffer<LoreEntityComponent> loreEntityComponent = _entityManager.GetBuffer<LoreEntityComponent>(loreEntity);

                                    loreEntityComponent.Add(new LoreEntityComponent
                                    {
                                        Type = LoreType.Story,
                                        Data = 1,
                                        Data2 = UnityEngine.Random.Range(1, 4)
                                    });
                                }
                            }
                        }
                    }
                }
            }

            entityCommandBuffer.RemoveComponent<CastAttemptComponent>(entity);
            entityCommandBuffer.AddBuffer<CastAttemptComponent>(entity);
        }

        entityCommandBuffer.Playback(_entityManager);
        entityCommandBuffer.Dispose();
    }

    private void RemoveMana(Entity caster, Entity spell, EntityCommandBuffer entityCommandBuffer)
    {
        if (
            !_entityManager.HasComponent<ManaComponent>(caster) ||
            !_entityManager.HasComponent<SpellCostComponent>(spell)
        )
        {
            return;
        }

        SpellCostComponent spellCostComponent = _entityManager.GetComponentData<SpellCostComponent>(spell);

        entityCommandBuffer.AddComponent(caster, new ManaRestoreComponent()
        {
            RestoreAmount = -spellCostComponent.Cost
        });
    }

    [BurstCompile]
    private bool CastSpell(int spellID, Entity casterEntity, ref SystemState state, EntityCommandBuffer entityCommandBuffer)
    {
        bool castedSpell = false;

        foreach (var (spell, cooldown, _, entity) in SystemAPI.Query<RefRO<SpellComponent>, RefRO<SpellCooldownComponent>, RefRO<SpellOnCooldownComponent>>().WithEntityAccess().WithOptions(EntityQueryOptions.IgnoreComponentEnabledState))
        {
            if (spell.ValueRO.SpellID != spellID) continue;

            if (_entityManager.IsComponentEnabled<SpellOnCooldownComponent>(entity))
            {
                continue;
            }

            if (!_entityManager.IsComponentEnabled<SpellOnCooldownComponent>(entity) && HasEnoughMana(entity, casterEntity, ref state))
            {
                entityCommandBuffer.SetComponentEnabled<SpellOnCooldownComponent>(entity, true);
                entityCommandBuffer.AddComponent(entity, new TimeCounterComponent { ElapsedTime = 0, EndTime = cooldown.ValueRO.Cooldown, isInfinite = false });
                castedSpell = true;
            }
        }

        return castedSpell;
    }

    [BurstCompile]
    private bool HasEnoughMana(Entity spellEntity, Entity casterEntity, ref SystemState state)
    {
        if (!state.EntityManager.HasComponent<ManaComponent>(casterEntity) || !state.EntityManager.HasComponent<SpellCostComponent>(spellEntity))
        {
            return false;
        }

        ManaComponent mana = state.EntityManager.GetComponentData<ManaComponent>(casterEntity);
        SpellCostComponent spellCost = state.EntityManager.GetComponentData<SpellCostComponent>(spellEntity);

        return mana.CurrentMana >= spellCost.Cost;
    }

    private void CastRandomAroundTarget(Entity caster, Entity spell, EntityCommandBuffer entityCommandBuffer)
    {
        if (!SystemAPI.ManagedAPI.TryGetSingleton(out SpellAnimationVisualsPrefabs spellAnimationVisualsPrefabs)) return;

        SpellComponent spellComponent = _entityManager.GetComponentData<SpellComponent>(spell);
        SpellElementComponent spellElement = _entityManager.GetComponentData<SpellElementComponent>(spell);
        SpellDamageComponent spellDamage = _entityManager.GetComponentData<SpellDamageComponent>(spell);

        PositionComponent casterPosition = _entityManager.GetComponentData<PositionComponent>(caster);
        PositionComponent playerPosition = _entityManager.GetComponentData<PositionComponent>(SystemAPI.GetSingletonEntity<PlayerComponent>());
        SpellRangeComponent spellRange = _entityManager.GetComponentData<SpellRangeComponent>(spell);
        SpellDurationComponent spellDuration = _entityManager.GetComponentData<SpellDurationComponent>(spell);

        UnityEngine.Debug.Log("Casting cast around target spell");

        int numberOfCasts = UnityEngine.Random.Range(5, 10);
        NativeArray<float2> positions = new NativeArray<float2>(numberOfCasts, Allocator.Temp);

        for (int i = 0; i < numberOfCasts; i++)
        {
            float x = UnityEngine.Random.Range(playerPosition.Position.x - spellRangeToNumber(spellRange).x, playerPosition.Position.x + spellRangeToNumber(spellRange).x);
            float y = UnityEngine.Random.Range(playerPosition.Position.y - spellRangeToNumber(spellRange).y, playerPosition.Position.y + spellRangeToNumber(spellRange).y);

            positions[i] = new float2(x, y);
        }

        for (int i = 0; i < numberOfCasts; i++)
        {
            GameObject spellVisuals = Object.Instantiate(spellAnimationVisualsPrefabs.GetSpellPrefab(spellComponent.SpellID));
            Entity spellEntity = _entityManager.CreateEntity(typeof(PhysicsWorldIndex));

            entityCommandBuffer.AddComponent(spellEntity, new LocalTransform
            {
                Position = new float3(positions[i].x, positions[i].y, 0),
                Rotation = quaternion.identity,
                Scale = 3
            });
            entityCommandBuffer.AddComponent(spellEntity, new SpellAoEEntityComponent
            {
                ToPosition = new float2(positions[i].x, positions[i].y),
                AreaOfEffect = spellRangeToNumber(spellRange)
            });
            entityCommandBuffer.AddComponent(spellEntity, new SpellDamageComponent
            {
                Damage = spellDamage.Damage
            });
            entityCommandBuffer.AddComponent(spellEntity, new SpellEntityGameObjectReferenceComponent
            {
                GameObject = spellVisuals
            });
            entityCommandBuffer.AddComponent(spellEntity, new TimeCounterComponent
            {
                ElapsedTime = 0,
                EndTime = spellDuration.Duration,
                isInfinite = false,
            });
            entityCommandBuffer.AddComponent(spellEntity, new SpellElementComponent
            {
                Element = spellElement.Element
            });

            var collider = new PhysicsCollider
            {
                Value = Unity.Physics.BoxCollider.Create(new BoxGeometry
                {
                    Center = new float3(0, 0, 0),
                    Size = new float3(1, 1, 1),
                    Orientation = quaternion.identity,
                    BevelRadius = 0,
                }, new CollisionFilter
                {
                    BelongsTo = 128,
                    CollidesWith = 1,
                    GroupIndex = 0
                })
            };
            collider.Value.Value.SetCollisionResponse(CollisionResponsePolicy.RaiseTriggerEvents);
            entityCommandBuffer.AddComponent(spellEntity, collider);

            entityCommandBuffer.AddComponent<PhysicsDamping>(spellEntity);
            entityCommandBuffer.SetComponent(spellEntity, new PhysicsDamping
            {
                Linear = 0.01f,
                Angular = 0.05f
            });

            entityCommandBuffer.AddComponent<PhysicsGravityFactor>(spellEntity);
            entityCommandBuffer.SetComponent(spellEntity, new PhysicsGravityFactor
            {
                Value = 0
            });

            entityCommandBuffer.AddComponent<PhysicsMass>(spellEntity);
            entityCommandBuffer.SetComponent(spellEntity, new PhysicsMass
            {
                InverseInertia = 6,
                InverseMass = 1,
                AngularExpansionFactor = 0,
                InertiaOrientation = quaternion.identity,
            });

            entityCommandBuffer.AddComponent<PhysicsVelocity>(spellEntity);
            entityCommandBuffer.SetComponent(spellEntity, new PhysicsVelocity
            {
                Linear = new float3(0, 0, 0),
                Angular = new float3(0, 0, 0)
            });
            spellVisuals.gameObject.transform.position = new Vector3(positions[i].x, positions[i].y, 0);
        }

        positions.Dispose();
    }

    private void CastSelfBossSpell(Entity caster, Entity spell, EntityCommandBuffer entityCommandBuffer)
    {
        if (!SystemAPI.ManagedAPI.TryGetSingleton(out SpellAnimationVisualsPrefabs spellAnimationVisualsPrefabs)) return;

        SpellComponent spellComponent = _entityManager.GetComponentData<SpellComponent>(spell);
        SpellElementComponent spellElement = _entityManager.GetComponentData<SpellElementComponent>(spell);
        SpellDamageComponent spellDamage = _entityManager.GetComponentData<SpellDamageComponent>(spell);

        PositionComponent casterPosition = _entityManager.GetComponentData<PositionComponent>(caster);
        SpellRangeComponent spellRange = _entityManager.GetComponentData<SpellRangeComponent>(spell);
        SpellDurationComponent spellDuration = _entityManager.GetComponentData<SpellDurationComponent>(spell);

        UnityEngine.Debug.Log("Casting self boss spell position");

        float angle = 0;
        float angleStep = 360 / 16;
        for (int i = 0; i < 16; i++)
        {
            GameObject spellVisuals = Object.Instantiate(spellAnimationVisualsPrefabs.GetSpellPrefab(spellComponent.SpellID));
            Entity spellEntity = _entityManager.CreateEntity(typeof(PhysicsWorldIndex));

            float x = casterPosition.Position.x + math.cos(math.radians(angle)) * spellRangeToNumber(spellRange).x;
            float y = casterPosition.Position.y + math.sin(math.radians(angle)) * spellRangeToNumber(spellRange).y;

            entityCommandBuffer.AddComponent(spellEntity, new LocalTransform
            {
                Position = new float3(x, y, 0),
                Rotation = quaternion.identity,
                Scale = 3
            });
            entityCommandBuffer.AddComponent(spellEntity, new SpellAoEEntityComponent
            {
                ToPosition = new float2(x, y),
                AreaOfEffect = spellRangeToNumber(spellRange)
            });
            entityCommandBuffer.AddComponent(spellEntity, new SpellDamageComponent
            {
                Damage = spellDamage.Damage
            });
            entityCommandBuffer.AddComponent(spellEntity, new SpellEntityGameObjectReferenceComponent
            {
                GameObject = spellVisuals
            });
            entityCommandBuffer.AddComponent(spellEntity, new TimeCounterComponent
            {
                ElapsedTime = 0,
                EndTime = spellDuration.Duration,
                isInfinite = false,
            });
            entityCommandBuffer.AddComponent(spellEntity, new SpellElementComponent
            {
                Element = spellElement.Element
            });

            var collider = new PhysicsCollider
            {
                Value = Unity.Physics.BoxCollider.Create(new BoxGeometry
                {
                    Center = new float3(0, 0, 0),
                    Size = new float3(1, 1, 1),
                    Orientation = quaternion.identity,
                    BevelRadius = 0,
                }, new CollisionFilter
                {
                    BelongsTo = 128,
                    CollidesWith = 1,
                    GroupIndex = 0
                })
            };
            collider.Value.Value.SetCollisionResponse(CollisionResponsePolicy.RaiseTriggerEvents);
            entityCommandBuffer.AddComponent(spellEntity, collider);

            entityCommandBuffer.AddComponent<PhysicsDamping>(spellEntity);
            entityCommandBuffer.SetComponent(spellEntity, new PhysicsDamping
            {
                Linear = 0.01f,
                Angular = 0.05f
            });

            entityCommandBuffer.AddComponent<PhysicsGravityFactor>(spellEntity);
            entityCommandBuffer.SetComponent(spellEntity, new PhysicsGravityFactor
            {
                Value = 0
            });

            entityCommandBuffer.AddComponent<PhysicsMass>(spellEntity);
            entityCommandBuffer.SetComponent(spellEntity, new PhysicsMass
            {
                InverseInertia = 6,
                InverseMass = 1,
                AngularExpansionFactor = 0,
                InertiaOrientation = quaternion.identity,
            });

            entityCommandBuffer.AddComponent<PhysicsVelocity>(spellEntity);
            entityCommandBuffer.SetComponent(spellEntity, new PhysicsVelocity
            {
                Linear = new float3(0, 0, 0),
                Angular = new float3(0, 0, 0)
            });
            spellVisuals.gameObject.transform.position = new Vector3(x, y, 0);

            angle += angleStep;
        }
    }

    private void CastSelfSpell(Entity caster, Entity spell, EntityCommandBuffer entityCommandBuffer)
    {
        if (
            !SystemAPI.ManagedAPI.TryGetSingleton(out SpellAnimationVisualsPrefabs spellAnimationVisualsPrefabs)
        )
        {
            return;
        }

        SpellComponent spellComponent = _entityManager.GetComponentData<SpellComponent>(spell);

        GameObject spellVisuals = Object.Instantiate(spellAnimationVisualsPrefabs.GetSpellPrefab(spellComponent.SpellID));
        Entity spellEntity = _entityManager.CreateEntity(typeof(PhysicsWorldIndex));
        SpellElementComponent spellElement = _entityManager.GetComponentData<SpellElementComponent>(spell);
        SpellDamageComponent spellDamage = _entityManager.GetComponentData<SpellDamageComponent>(spell);

        PositionComponent casterPosition = _entityManager.GetComponentData<PositionComponent>(caster);
        SpellRangeComponent spellRange = _entityManager.GetComponentData<SpellRangeComponent>(spell);
        SpellDurationComponent spellDuration = _entityManager.GetComponentData<SpellDurationComponent>(spell);

        UnityEngine.Debug.Log("Casting spell at self position");

        entityCommandBuffer.AddComponent(spellEntity, new LocalTransform
        {
            Position = new float3(casterPosition.Position.x, casterPosition.Position.y, 0),
            Rotation = quaternion.identity,
            Scale = 3
        });
        entityCommandBuffer.AddComponent(spellEntity, new SpellAoEEntityComponent
        {
            ToPosition = new float2(casterPosition.Position.x, casterPosition.Position.y),
            AreaOfEffect = spellRangeToNumber(spellRange)
        });
        entityCommandBuffer.AddComponent(spellEntity, new SpellDamageComponent
        {
            Damage = spellDamage.Damage
        });
        entityCommandBuffer.AddComponent(spellEntity, new SpellEntityGameObjectReferenceComponent
        {
            GameObject = spellVisuals
        });
        entityCommandBuffer.AddComponent(spellEntity, new TimeCounterComponent
        {
            ElapsedTime = 0,
            EndTime = spellDuration.Duration,
            isInfinite = false,
        });
        entityCommandBuffer.AddComponent(spellEntity, new SpellElementComponent
        {
            Element = spellElement.Element
        });

        var collider = new PhysicsCollider
        {
            Value = Unity.Physics.BoxCollider.Create(new BoxGeometry
            {
                Center = new float3(0, 0, 0),
                Size = new float3(1, 1, 1),
                Orientation = quaternion.identity,
                BevelRadius = 0,
            }, new CollisionFilter
            {
                BelongsTo = 4,
                CollidesWith = 2,
                GroupIndex = 0
            })
        };
        collider.Value.Value.SetCollisionResponse(CollisionResponsePolicy.RaiseTriggerEvents);
        entityCommandBuffer.AddComponent(spellEntity, collider);

        entityCommandBuffer.AddComponent<PhysicsDamping>(spellEntity);
        entityCommandBuffer.SetComponent(spellEntity, new PhysicsDamping
        {
            Linear = 0.01f,
            Angular = 0.05f
        });

        entityCommandBuffer.AddComponent<PhysicsGravityFactor>(spellEntity);
        entityCommandBuffer.SetComponent(spellEntity, new PhysicsGravityFactor
        {
            Value = 0
        });

        entityCommandBuffer.AddComponent<PhysicsMass>(spellEntity);
        entityCommandBuffer.SetComponent(spellEntity, new PhysicsMass
        {
            InverseInertia = 6,
            InverseMass = 1,
            AngularExpansionFactor = 0,
            InertiaOrientation = quaternion.identity,
        });

        entityCommandBuffer.AddComponent<PhysicsVelocity>(spellEntity);
        entityCommandBuffer.SetComponent(spellEntity, new PhysicsVelocity
        {
            Linear = new float3(0, 0, 0),
            Angular = new float3(0, 0, 0)
        });
        spellVisuals.gameObject.transform.position = new Vector3(casterPosition.Position.x, casterPosition.Position.y, 0);

        if (spellComponent.UpgradeLevel >= 1 && spellElement.Element == Element.Fire)
        {
            float2[] directions = new float2[]
            {
                new float2(0, 10),
                new float2(0, -10),
                new float2(10, 0),
                new float2(-10, 0)
            };

            foreach (var offset in directions)
            {
                float2 targetPosition = casterPosition.Position + offset;

                GameObject spellVisuals2 = Object.Instantiate(spellAnimationVisualsPrefabs.GetSpellPrefab(9));
                Entity spellEntity2 = _entityManager.CreateEntity(typeof(PhysicsWorldIndex));
                SkillShotToDirection(casterPosition.Position, targetPosition, spellEntity2, spellVisuals2, spellDamage.Damage, spellElement.Element, entityCommandBuffer);
            }
        }

        if (spellComponent.UpgradeLevel >= 2 && spellElement.Element == Element.Fire)
        {
            float2 directionNorth = new float2(0, 10);

            float2[] diagonals = new float2[]
            {
                RotateVector(directionNorth, math.radians(45)),
                RotateVector(directionNorth, math.radians(-45)),
                RotateVector(directionNorth, math.radians(135)),
                RotateVector(directionNorth, math.radians(-135))
            };

            foreach (var offset in diagonals)
            {
                float2 targetPosition = casterPosition.Position + offset;

                GameObject spellVisuals3 = Object.Instantiate(spellAnimationVisualsPrefabs.GetSpellPrefab(9));
                Entity spellEntity3 = _entityManager.CreateEntity(typeof(PhysicsWorldIndex));
                SkillShotToDirection(casterPosition.Position, targetPosition, spellEntity3, spellVisuals3, spellDamage.Damage, spellElement.Element, entityCommandBuffer);
            }
        }
    }

    private void CastMousePositionSpell(Entity caster, Entity spell, EntityCommandBuffer entityCommandBuffer)
    {
        if (
            !_entityManager.HasComponent<MousePositionComponent>(caster) ||
            !SystemAPI.ManagedAPI.TryGetSingleton(out SpellAnimationVisualsPrefabs spellAnimationVisualsPrefabs)
        )
        {
            return;
        }

        SpellComponent spellComponent = _entityManager.GetComponentData<SpellComponent>(spell);
        SpellDamageComponent spellDamage = _entityManager.GetComponentData<SpellDamageComponent>(spell);
        SpellElementComponent spellElement = _entityManager.GetComponentData<SpellElementComponent>(spell);

        GameObject spellVisuals = Object.Instantiate(spellAnimationVisualsPrefabs.GetSpellPrefab(spellComponent.SpellID));
        Entity spellEntity = _entityManager.CreateEntity(typeof(PhysicsWorldIndex));

        MousePositionComponent mousePosition = _entityManager.GetComponentData<MousePositionComponent>(caster);
        SpellRangeComponent spellRange = _entityManager.GetComponentData<SpellRangeComponent>(spell);
        SpellDurationComponent spellDuration = _entityManager.GetComponentData<SpellDurationComponent>(spell);

        UnityEngine.Debug.Log("Casting spell at mouse position");

        if (spellComponent.SpellType == SpellType.AreaOfEffect)
        {
            AoESpellToPosition(new float2(mousePosition.Position.x, mousePosition.Position.y), spellDuration, spellRange, spellElement, spellDamage, spellEntity, spellVisuals, entityCommandBuffer);

            if (spellComponent.UpgradeLevel >= 2)
            {
                GameObject spellVisuals2 = Object.Instantiate(spellAnimationVisualsPrefabs.GetSpellPrefab(spellComponent.SpellID));
                Entity spellEntity2 = _entityManager.CreateEntity(typeof(PhysicsWorldIndex));
                GameObject spellVisuals3 = Object.Instantiate(spellAnimationVisualsPrefabs.GetSpellPrefab(spellComponent.SpellID));
                Entity spellEntity3 = _entityManager.CreateEntity(typeof(PhysicsWorldIndex));
                GameObject spellVisuals4 = Object.Instantiate(spellAnimationVisualsPrefabs.GetSpellPrefab(spellComponent.SpellID));
                Entity spellEntity4 = _entityManager.CreateEntity(typeof(PhysicsWorldIndex));
                GameObject spellVisuals5 = Object.Instantiate(spellAnimationVisualsPrefabs.GetSpellPrefab(spellComponent.SpellID));
                Entity spellEntity5 = _entityManager.CreateEntity(typeof(PhysicsWorldIndex));

                AoESpellToPosition(new float2(mousePosition.Position.x + 1, mousePosition.Position.y + 1), spellDuration, spellRange, spellElement, spellDamage, spellEntity2, spellVisuals2, entityCommandBuffer);
                AoESpellToPosition(new float2(mousePosition.Position.x - 1, mousePosition.Position.y - 1), spellDuration, spellRange, spellElement, spellDamage, spellEntity3, spellVisuals3, entityCommandBuffer);

                AoESpellToPosition(new float2(mousePosition.Position.x - 1, mousePosition.Position.y + 1), spellDuration, spellRange, spellElement, spellDamage, spellEntity4, spellVisuals4, entityCommandBuffer);
                AoESpellToPosition(new float2(mousePosition.Position.x + 1, mousePosition.Position.y - 1), spellDuration, spellRange, spellElement, spellDamage, spellEntity5, spellVisuals5, entityCommandBuffer);
            }


        }

        if (spellComponent.SpellType == SpellType.SkillShot)
        {
            PositionComponent casterPosition = _entityManager.GetComponentData<PositionComponent>(caster);

            SkillShotToDirection(casterPosition.Position, new float2(mousePosition.Position.x, mousePosition.Position.y), spellEntity, spellVisuals, spellDamage.Damage, spellElement.Element, entityCommandBuffer);

            if (spellComponent.UpgradeLevel >= 1)
            {
                GameObject spellVisuals2 = Object.Instantiate(spellAnimationVisualsPrefabs.GetSpellPrefab(spellComponent.SpellID));
                Entity spellEntity2 = _entityManager.CreateEntity(typeof(PhysicsWorldIndex));

                float2 backwardsPosition = new float2(casterPosition.Position.x - (mousePosition.Position.x - casterPosition.Position.x), casterPosition.Position.y - (mousePosition.Position.y - casterPosition.Position.y));
                SkillShotToDirection(casterPosition.Position, backwardsPosition, spellEntity2, spellVisuals2, spellDamage.Damage, spellElement.Element, entityCommandBuffer);
            }

            if (spellComponent.UpgradeLevel >= 2)
            {
                GameObject spellVisuals3 = Object.Instantiate(spellAnimationVisualsPrefabs.GetSpellPrefab(spellComponent.SpellID));
                Entity spellEntity3 = _entityManager.CreateEntity(typeof(PhysicsWorldIndex));

                var forward = math.normalize(mousePosition.Position - casterPosition.Position);
                var distanceOriginal = math.distance(mousePosition.Position, casterPosition.Position);
                var diagonal1 = RotateVector(forward, math.radians(90));
                SkillShotToDirection(casterPosition.Position, casterPosition.Position + diagonal1 * distanceOriginal, spellEntity3, spellVisuals3, spellDamage.Damage, spellElement.Element, entityCommandBuffer);

                GameObject spellVisuals4 = Object.Instantiate(spellAnimationVisualsPrefabs.GetSpellPrefab(spellComponent.SpellID));
                Entity spellEntity4 = _entityManager.CreateEntity(typeof(PhysicsWorldIndex));

                var diagonal2 = RotateVector(forward, math.radians(-90));
                SkillShotToDirection(casterPosition.Position, casterPosition.Position + diagonal2 * distanceOriginal, spellEntity4, spellVisuals4, spellDamage.Damage, spellElement.Element, entityCommandBuffer);
            }

            if (spellComponent.UpgradeLevel >= 3)
            {
                GameObject spellVisuals5 = Object.Instantiate(spellAnimationVisualsPrefabs.GetSpellPrefab(spellComponent.SpellID));
                Entity spellEntity5 = _entityManager.CreateEntity(typeof(PhysicsWorldIndex));

                var forward = math.normalize(mousePosition.Position - casterPosition.Position);
                var distanceOriginal = math.distance(mousePosition.Position, casterPosition.Position);
                var diagonal1 = RotateVector(forward, math.radians(45));
                SkillShotToDirection(casterPosition.Position, casterPosition.Position + diagonal1 * distanceOriginal, spellEntity5, spellVisuals5, spellDamage.Damage, spellElement.Element, entityCommandBuffer);

                GameObject spellVisuals6 = Object.Instantiate(spellAnimationVisualsPrefabs.GetSpellPrefab(spellComponent.SpellID));
                Entity spellEntity6 = _entityManager.CreateEntity(typeof(PhysicsWorldIndex));

                var diagonal2 = RotateVector(forward, math.radians(-45));
                SkillShotToDirection(casterPosition.Position, casterPosition.Position + diagonal2 * distanceOriginal, spellEntity6, spellVisuals6, spellDamage.Damage, spellElement.Element, entityCommandBuffer);

                GameObject spellVisuals7 = Object.Instantiate(spellAnimationVisualsPrefabs.GetSpellPrefab(spellComponent.SpellID));
                Entity spellEntity7 = _entityManager.CreateEntity(typeof(PhysicsWorldIndex));

                var diagonal3 = RotateVector(forward, math.radians(135));
                SkillShotToDirection(casterPosition.Position, casterPosition.Position + diagonal3 * distanceOriginal, spellEntity7, spellVisuals7, spellDamage.Damage, spellElement.Element, entityCommandBuffer);

                GameObject spellVisuals8 = Object.Instantiate(spellAnimationVisualsPrefabs.GetSpellPrefab(spellComponent.SpellID));
                Entity spellEntity8 = _entityManager.CreateEntity(typeof(PhysicsWorldIndex));

                var diagonal4 = RotateVector(forward, math.radians(-135));
                SkillShotToDirection(casterPosition.Position, casterPosition.Position + diagonal4 * distanceOriginal, spellEntity8, spellVisuals8, spellDamage.Damage, spellElement.Element, entityCommandBuffer);
            }
        }
    }

    private float2 spellRangeToNumber(SpellRangeComponent spellRange)
    {
        float2 range = new float2(0, 0);

        switch (spellRange.Range)
        {
            case SpellRange.Melee:
                range = new float2(1, 1);
                break;
            case SpellRange.Three:
                range = new float2(3, 3);
                break;
            case SpellRange.Five:
                range = new float2(5, 5);
                break;
            case SpellRange.Ten:
                range = new float2(10, 10);
                break;
            case SpellRange.Infinite:
                range = new float2(float.MaxValue, float.MaxValue);
                break;
        }

        return range;
    }

    private float2 RotateVector(float2 vector, float angleRadians)
    {
        var cos = math.cos(angleRadians);
        var sin = math.sin(angleRadians);

        return new float2(
            cos * vector.x - sin * vector.y,
            sin * vector.x + cos * vector.y
        );
    }

    private void AoESpellToPosition(float2 position, SpellDurationComponent spellDuration, SpellRangeComponent spellRange, SpellElementComponent spellElement, SpellDamageComponent spellDamage, Entity entity, GameObject visuals, EntityCommandBuffer entityCommandBuffer)
    {
        entityCommandBuffer.AddComponent(entity, new SpellDamageComponent
        {
            Damage = spellDamage.Damage
        });

        entityCommandBuffer.AddComponent(entity, new LocalTransform
        {
            Position = new float3(position.x, position.y, 0),
            Rotation = quaternion.identity,
            Scale = 1
        });

        entityCommandBuffer.AddComponent(entity, new SpellAoEEntityComponent
        {
            ToPosition = new float2(position.x, position.y),
            AreaOfEffect = spellRangeToNumber(spellRange)
        });
        entityCommandBuffer.AddComponent(entity, new SpellEntityGameObjectReferenceComponent
        {
            GameObject = visuals
        });
        entityCommandBuffer.AddComponent(entity, new TimeCounterComponent
        {
            ElapsedTime = 0,
            EndTime = spellDuration.Duration,
            isInfinite = false,
        });
        entityCommandBuffer.AddComponent(entity, new SpellElementComponent
        {
            Element = spellElement.Element
        });

        var collider = new PhysicsCollider
        {
            Value = Unity.Physics.BoxCollider.Create(new BoxGeometry
            {
                Center = new float3(0, 0, 0),
                Size = new float3(1, 1, 1),
                Orientation = quaternion.identity,
                BevelRadius = 0,
            }, new CollisionFilter
            {
                BelongsTo = 4,
                CollidesWith = 2,
                GroupIndex = 0
            })
        };
        collider.Value.Value.SetCollisionResponse(CollisionResponsePolicy.RaiseTriggerEvents);
        entityCommandBuffer.AddComponent(entity, collider);

        entityCommandBuffer.AddComponent(entity, new PhysicsDamping
        {
            Linear = 0.01f,
            Angular = 0.05f
        });
        entityCommandBuffer.SetComponent(entity, new PhysicsDamping
        {
            Linear = 0.01f,
            Angular = 0.05f
        });

        entityCommandBuffer.AddComponent<PhysicsGravityFactor>(entity);
        entityCommandBuffer.SetComponent(entity, new PhysicsGravityFactor
        {
            Value = 0
        });

        entityCommandBuffer.AddComponent<PhysicsMass>(entity);
        entityCommandBuffer.SetComponent(entity, new PhysicsMass
        {
            InverseInertia = 6,
            InverseMass = 1,
            AngularExpansionFactor = 0,
            InertiaOrientation = quaternion.identity,
        });

        entityCommandBuffer.AddComponent<PhysicsVelocity>(entity);
        entityCommandBuffer.SetComponent(entity, new PhysicsVelocity
        {
            Linear = new float3(0, 0, 0),
            Angular = new float3(0, 0, 0)
        });

        visuals.gameObject.transform.position = new Vector3(position.x, position.y, 0);
    }

    private void SkillShotToDirection(float2 casterPosition, float2 targetDirection, Entity entity, GameObject visuals, int Damage, Element element, EntityCommandBuffer entityCommandBuffer)
    {
        entityCommandBuffer.AddComponent(entity, new SpellDamageComponent
        {
            Damage = Damage
        });

        entityCommandBuffer.AddComponent(entity, new LocalTransform
        {
            Position = new float3(casterPosition.x, casterPosition.y, 0),
            Rotation = quaternion.identity,
            Scale = 1
        });

        entityCommandBuffer.AddComponent(entity, new SpellSkillShotEntityComponent
        {
            ToPosition = targetDirection,
            FromPosition = new float2(casterPosition.x, casterPosition.y)
        });
        entityCommandBuffer.AddComponent(entity, new SpellEntityGameObjectReferenceComponent
        {
            GameObject = visuals
        });
        entityCommandBuffer.AddComponent(entity, new PositionComponent
        {
            Position = new float2(casterPosition.x, casterPosition.y)
        });
        entityCommandBuffer.AddComponent(entity, new VelocityComponent
        {
            Velocity = 10,
            BaseVelocity = 10
        });
        entityCommandBuffer.AddComponent(entity, new SpellElementComponent
        {
            Element = element
        });

        var collider = new PhysicsCollider
        {
            Value = Unity.Physics.BoxCollider.Create(new BoxGeometry
            {
                Center = new float3(0, 0, 0),
                Size = new float3(1, 1, 1),
                Orientation = quaternion.identity,
                BevelRadius = 0,
            }, new CollisionFilter
            {
                BelongsTo = 4,
                CollidesWith = 2,
                GroupIndex = 0
            })
        };
        collider.Value.Value.SetCollisionResponse(CollisionResponsePolicy.RaiseTriggerEvents);
        entityCommandBuffer.AddComponent(entity, collider);

        entityCommandBuffer.AddComponent(entity, new PhysicsDamping
        {
            Linear = 0.01f,
            Angular = 0.05f
        });
        entityCommandBuffer.SetComponent(entity, new PhysicsDamping
        {
            Linear = 0.01f,
            Angular = 0.05f
        });

        entityCommandBuffer.AddComponent<PhysicsGravityFactor>(entity);
        entityCommandBuffer.SetComponent(entity, new PhysicsGravityFactor
        {
            Value = 0
        });

        entityCommandBuffer.AddComponent<PhysicsMass>(entity);
        entityCommandBuffer.SetComponent(entity, new PhysicsMass
        {
            InverseInertia = 6,
            InverseMass = 1,
            AngularExpansionFactor = 0,
            InertiaOrientation = quaternion.identity,
        });

        entityCommandBuffer.AddComponent<PhysicsVelocity>(entity);
        entityCommandBuffer.SetComponent(entity, new PhysicsVelocity
        {
            Linear = new float3(0, 0, 0),
            Angular = new float3(0, 0, 0)
        });

        visuals.gameObject.transform.position = new Vector3(casterPosition.x, casterPosition.y, 0);
    }

    public void OnDestroy(ref SystemState state)
    {

    }
}
