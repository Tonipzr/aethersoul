using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Transforms;
using UnityEngine;

partial struct EscarlinaAnimationSystem : ISystem
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

        foreach (var (transform, playerComponent, entity) in SystemAPI.Query<LocalTransform, PlayerComponent>().WithEntityAccess())
        {
            if (!_entityManager.HasComponent<VisualsReferenceComponent>(entity))
            {
                GameObject playerVisuals = Object.Instantiate(animationVisualsPrefabs.Escarlina);

                entityCommandBuffer.AddComponent(entity, new VisualsReferenceComponent { gameObject = playerVisuals });
            }
            else
            {
                VisualsReferenceComponent visualsReferenceComponent = _entityManager.GetComponentData<VisualsReferenceComponent>(entity);

                Entity inputEntity = SystemAPI.GetSingletonEntity<InputComponent>();
                InputComponent inputComponent = _entityManager.GetComponentData<InputComponent>(inputEntity);

                float movementValue = Mathf.Abs(inputComponent.movement.x) > 0 || Mathf.Abs(inputComponent.movement.y) > 0 ?
                    Mathf.Abs(inputComponent.movement.x) + Mathf.Abs(inputComponent.movement.y) : -1;

                visualsReferenceComponent.gameObject.transform.position = transform.Position;
                visualsReferenceComponent.gameObject.transform.rotation = transform.Rotation;

                if (inputComponent.movement.x < 0)
                {
                    visualsReferenceComponent.gameObject.GetComponent<SpriteRenderer>().flipX = true;
                }
                else if (inputComponent.movement.x > 0)
                {
                    visualsReferenceComponent.gameObject.GetComponent<SpriteRenderer>().flipX = false;
                }

                visualsReferenceComponent.gameObject.GetComponent<Animator>().SetFloat("Movement", movementValue);
                visualsReferenceComponent.gameObject.GetComponent<Animator>().SetBool("Dash", inputComponent.pressingSpace);
                visualsReferenceComponent.gameObject.GetComponent<Animator>().SetBool("Action", inputComponent.pressingInteract);
            }
        }

        entityCommandBuffer.Playback(_entityManager);
        entityCommandBuffer.Dispose();
    }
}
