using Unity.Collections;
using Unity.Entities;
using Unity.Transforms;
using UnityEngine;

partial struct EnemyAnimationSystem : ISystem
{
    private EntityManager _entityManager;

    public void OnUpdate(ref SystemState state)
    {
        if (!SystemAPI.ManagedAPI.TryGetSingleton(out AnimationVisualsPrefabs animationVisualsPrefabs))
        {
            return;
        }

        _entityManager = state.EntityManager;

        EntityCommandBuffer entityCommandBuffer = new EntityCommandBuffer(Allocator.Temp);

        foreach (var (transform, monsterComponent, entity) in SystemAPI.Query<LocalTransform, MonsterComponent>().WithEntityAccess())
        {
            if (!_entityManager.HasComponent<VisualsReferenceComponent>(entity))
            {
                GameObject monsterVisuals;
                switch (monsterComponent.MonsterType)
                {
                    case MonsterType.Bat:
                        monsterVisuals = Object.Instantiate(animationVisualsPrefabs.Bat);
                        break;
                    case MonsterType.Crab:
                        monsterVisuals = Object.Instantiate(animationVisualsPrefabs.Crab);
                        break;
                    case MonsterType.Golem:
                        monsterVisuals = Object.Instantiate(animationVisualsPrefabs.Golem);
                        break;
                    case MonsterType.Rat:
                        monsterVisuals = Object.Instantiate(animationVisualsPrefabs.Rat);
                        break;
                    case MonsterType.Slime:
                        monsterVisuals = Object.Instantiate(animationVisualsPrefabs.Slime);
                        break;
                    default:
                        monsterVisuals = Object.Instantiate(animationVisualsPrefabs.Bat);
                        break;
                }

                entityCommandBuffer.AddComponent(entity, new VisualsReferenceComponent { gameObject = monsterVisuals });
            }
            else
            {
                VisualsReferenceComponent visualsReferenceComponent = _entityManager.GetComponentData<VisualsReferenceComponent>(entity);
                DirectionComponent directionComponent = _entityManager.GetComponentData<DirectionComponent>(entity);

                if (directionComponent.Direction == Direction.Left)
                {
                    visualsReferenceComponent.gameObject.GetComponent<SpriteRenderer>().flipX = true;
                }
                else if (directionComponent.Direction == Direction.Right)
                {
                    visualsReferenceComponent.gameObject.GetComponent<SpriteRenderer>().flipX = false;
                }

                visualsReferenceComponent.gameObject.transform.position = transform.Position;
                visualsReferenceComponent.gameObject.transform.rotation = transform.Rotation;
                visualsReferenceComponent.gameObject.transform.localScale = new Vector3(transform.Scale, transform.Scale, transform.Scale);

                visualsReferenceComponent.gameObject.GetComponent<Animator>().SetFloat("Movement", 1);
            }
        }

        entityCommandBuffer.Playback(_entityManager);
        entityCommandBuffer.Dispose();
    }
}
