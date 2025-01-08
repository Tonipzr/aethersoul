using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Transforms;
using Unity.VisualScripting;
using UnityEngine;

partial struct AutoAttackSystem : ISystem
{
    private EntityManager _entityManager;

    public void OnCreate(ref SystemState state)
    {

    }

    public void OnUpdate(ref SystemState state)
    {
        _entityManager = state.EntityManager;
        EntityCommandBuffer entityCommandBuffer = new EntityCommandBuffer(Allocator.Temp);

        if (!SystemAPI.TryGetSingletonEntity<PlayerComponent>(out Entity player))
        {
            return;
        }

        if (
            !_entityManager.HasComponent<MousePositionComponent>(player) ||
            !SystemAPI.ManagedAPI.TryGetSingleton(out SpellAnimationVisualsPrefabs spellAnimationVisualsPrefabs)
        )
        {
            return;
        }

        // Search the AA Spell entity
        Entity spell = Entity.Null;
        foreach (var (spellComponentLookup, spellEntityLookup) in SystemAPI.Query<RefRO<SpellComponent>>().WithEntityAccess())
        {
            if (spellComponentLookup.ValueRO.SpellID == 6)
            {
                spell = spellEntityLookup;
                break;
            }
        }

        if (spell == Entity.Null)
        {
            return;
        }

        if (_entityManager.IsComponentEnabled<SpellOnCooldownComponent>(spell))
        {
            return;
        }

        SpellComponent spellComponent = _entityManager.GetComponentData<SpellComponent>(spell);
        SpellDamageComponent spellDamage = _entityManager.GetComponentData<SpellDamageComponent>(spell);
        SpellElementComponent spellElement = _entityManager.GetComponentData<SpellElementComponent>(spell);

        GameObject spellVisuals = Object.Instantiate(spellAnimationVisualsPrefabs.GetSpellPrefab(spellComponent.SpellID));
        Entity spellEntity = _entityManager.CreateEntity(typeof(PhysicsWorldIndex));

        MousePositionComponent mousePosition = _entityManager.GetComponentData<MousePositionComponent>(player);
        SpellDurationComponent spellDuration = _entityManager.GetComponentData<SpellDurationComponent>(spell);
        SpellCooldownComponent cooldown = _entityManager.GetComponentData<SpellCooldownComponent>(spell);

        UnityEngine.Debug.Log("Casting AA at mouse position");

        entityCommandBuffer.AddComponent(spellEntity, new SpellDamageComponent
        {
            Damage = spellDamage.Damage
        });
        if (spellComponent.SpellType == SpellType.AreaOfEffect)
        {
            entityCommandBuffer.AddComponent(spellEntity, new LocalTransform
            {
                Position = new float3(mousePosition.Position.x, mousePosition.Position.y, 0),
                Rotation = quaternion.identity,
                Scale = 1
            });

            entityCommandBuffer.AddComponent(spellEntity, new SpellAoEEntityComponent
            {
                ToPosition = new float2(mousePosition.Position.x, mousePosition.Position.y),
                AreaOfEffect = 5
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

            entityCommandBuffer.AddComponent(spellEntity, new PhysicsDamping
            {
                Linear = 0.01f,
                Angular = 0.05f
            });
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

            spellVisuals.gameObject.transform.position = new Vector3(mousePosition.Position.x, mousePosition.Position.y, 0);

            entityCommandBuffer.SetComponentEnabled<SpellOnCooldownComponent>(spell, true);
            entityCommandBuffer.AddComponent(spell, new TimeCounterComponent { ElapsedTime = 0, EndTime = cooldown.Cooldown, isInfinite = false });
        }

        entityCommandBuffer.Playback(_entityManager);
        entityCommandBuffer.Dispose();
    }

    public void OnDestroy(ref SystemState state)
    {

    }
}
