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
        public NativeArray<Entity> entitiesColliding;

        [BurstCompile]
        public void Execute(TriggerEvent collisionEvent)
        {
            // UnityEngine.Debug.Log("Collision detected " + collisionEvent.EntityA.Index + " " + collisionEvent.EntityB.Index);
            Entity entityA = collisionEvent.EntityA;
            Entity entityB = collisionEvent.EntityB;

            entitiesColliding[0] = entityA;
            entitiesColliding[1] = entityB;
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
        NativeArray<Entity> entitiesColliding = new NativeArray<Entity>(2, Allocator.TempJob);
        JobHandle jobHandle = new CountNumTriggerEvents
        {
            entitiesColliding = entitiesColliding
        }.Schedule(simuilationSingleton, state.Dependency);
        jobHandle.Complete();

        if (entitiesColliding[0] == Entity.Null || entitiesColliding[1] == Entity.Null)
        {
            return;
        }

        if (IsCollisionEnemyWithPlayer(entitiesColliding[0], entitiesColliding[1]))
        {
            Entity playerEntity = GetPlayerEntity(entitiesColliding[0], entitiesColliding[1]);
            Entity monsterEntity = GetMonsterEntity(entitiesColliding[0], entitiesColliding[1]);
            MonsterStatsComponent monsterStatsComponent = _entityManager.GetComponentData<MonsterStatsComponent>(monsterEntity);

            entityCommandBuffer.AddComponent(playerEntity, new DamageComponent
            {
                DamageAmount = monsterStatsComponent.Damage
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
            MonsterComponent monsterComponent = _entityManager.GetComponentData<MonsterComponent>(monsterEntity);

            VisualsReferenceComponent visualsReferenceComponent = _entityManager.GetComponentData<VisualsReferenceComponent>(monsterEntity);

            entityCommandBuffer.AddComponent(monsterEntity, new DamageComponent
            {
                DamageAmount = spellDamage.Damage
            });

            visualsReferenceComponent.gameObject.GetComponent<Animator>().SetTrigger("Hit");

            entityCommandBuffer.AddComponent<DestroySpellEntityComponent>(spellEntity);
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
