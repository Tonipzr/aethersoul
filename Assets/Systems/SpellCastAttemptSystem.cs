using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

partial struct SpellCastAttemptSystem : ISystem
{
    private EntityManager _entityManager;

    public void OnCreate(ref SystemState state)
    {

    }

    public void OnUpdate(ref SystemState state)
    {
        _entityManager = state.EntityManager;
        EntityCommandBuffer entityCommandBuffer = new EntityCommandBuffer(Allocator.Temp);

        foreach (var (castAttempt, entity) in SystemAPI.Query<DynamicBuffer<PlayerCastAttemptComponent>>().WithEntityAccess())
        {
            if (castAttempt.Length == 0)
            {
                break;
            }

            for (int i = 0; i < castAttempt.Length; i++)
            {
                foreach (var (spell, spellTarget, spellEntity) in SystemAPI.Query<RefRO<SpellComponent>, RefRO<SpellTargetComponent>>().WithEntityAccess())
                {
                    if (spell.ValueRO.SpellID == castAttempt[i].SpellID)
                    {
                        if (spellTarget.ValueRO.Target == SpellTarget.MousePosition) CastMousePositionSpell(entity, spellEntity, entityCommandBuffer);
                        if (spellTarget.ValueRO.Target == SpellTarget.Self) CastSelfSpell(entity, spellEntity, entityCommandBuffer);
                    }
                }
            }

            entityCommandBuffer.RemoveComponent<PlayerCastAttemptComponent>(entity);
            entityCommandBuffer.AddBuffer<PlayerCastAttemptComponent>(entity);
        }

        entityCommandBuffer.Playback(_entityManager);
        entityCommandBuffer.Dispose();
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
        Entity spellEntity = _entityManager.CreateEntity();

        PositionComponent casterPosition = _entityManager.GetComponentData<PositionComponent>(caster);
        SpellRangeComponent spellRange = _entityManager.GetComponentData<SpellRangeComponent>(spell);
        SpellDurationComponent spellDuration = _entityManager.GetComponentData<SpellDurationComponent>(spell);

        UnityEngine.Debug.Log("Casting spell at self position");

        entityCommandBuffer.AddComponent(spellEntity, new SpellAoEEntityComponent
        {
            ToPosition = new float2(casterPosition.Position.x, casterPosition.Position.y),
            AreaOfEffect = spellRangeToNumber(spellRange)
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
        spellVisuals.gameObject.transform.position = new Vector3(casterPosition.Position.x, casterPosition.Position.y, 0);
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

        GameObject spellVisuals = Object.Instantiate(spellAnimationVisualsPrefabs.GetSpellPrefab(spellComponent.SpellID));
        Entity spellEntity = _entityManager.CreateEntity();

        MousePositionComponent mousePosition = _entityManager.GetComponentData<MousePositionComponent>(caster);
        SpellRangeComponent spellRange = _entityManager.GetComponentData<SpellRangeComponent>(spell);
        SpellDurationComponent spellDuration = _entityManager.GetComponentData<SpellDurationComponent>(spell);

        UnityEngine.Debug.Log("Casting spell at mouse position");

        if (spellComponent.SpellType == SpellType.AreaOfEffect)
        {
            entityCommandBuffer.AddComponent(spellEntity, new SpellAoEEntityComponent
            {
                ToPosition = new float2(mousePosition.Position.x, mousePosition.Position.y),
                AreaOfEffect = spellRangeToNumber(spellRange)
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
            spellVisuals.gameObject.transform.position = new Vector3(mousePosition.Position.x, mousePosition.Position.y, 0);
        }

        if (spellComponent.SpellType == SpellType.SkillShot)
        {
            PositionComponent casterPosition = _entityManager.GetComponentData<PositionComponent>(caster);

            entityCommandBuffer.AddComponent(spellEntity, new SpellSkillShotEntityComponent
            {
                ToPosition = new float2(mousePosition.Position.x, mousePosition.Position.y),
                FromPosition = new float2(casterPosition.Position.x, casterPosition.Position.y)
            });
            entityCommandBuffer.AddComponent(spellEntity, new SpellEntityGameObjectReferenceComponent
            {
                GameObject = spellVisuals
            });
            entityCommandBuffer.AddComponent(spellEntity, new PositionComponent
            {
                Position = new float2(casterPosition.Position.x, casterPosition.Position.y)
            });
            entityCommandBuffer.AddComponent(spellEntity, new VelocityComponent
            {
                Velocity = 10
            });
            spellVisuals.gameObject.transform.position = new Vector3(casterPosition.Position.x, casterPosition.Position.y, 0);
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

    public void OnDestroy(ref SystemState state)
    {

    }
}