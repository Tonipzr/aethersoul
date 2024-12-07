using System.Diagnostics;
using Unity.Collections;
using Unity.Entities;
using UnityEngine;
using UnityEngine.Tilemaps;

partial class AudioSystem : SystemBase
{
    private EntityManager _entityManager;
    private float _timeSinceLastStep = 0;

    protected override void OnUpdate()
    {
        _entityManager = EntityManager;

        EntityCommandBuffer entityCommandBuffer = new EntityCommandBuffer(Allocator.Temp);

        foreach (var (audio, entity) in SystemAPI.Query<RefRW<AudioComponent>>().WithEntityAccess())
        {
            if (audio.ValueRO.IsProcessed)
            {
                continue;
            }

            AudioManager.Instance.PlayAudio(audio.ValueRO.Audio);
            audio.ValueRW.IsProcessed = true;

            entityCommandBuffer.DestroyEntity(entity);
        }


        if (SystemAPI.TryGetSingletonEntity<InputComponent>(out Entity inputEntity))
        {
            InputComponent inputComponent = _entityManager.GetComponentData<InputComponent>(inputEntity);

            if (inputComponent.movement.x > 0 || inputComponent.movement.y > 0)
            {
                if (_timeSinceLastStep < 0.5f)
                {
                    _timeSinceLastStep += World.Time.DeltaTime;
                    return;
                }

                _timeSinceLastStep = 0;

                if (SystemAPI.TryGetSingletonEntity<PlayerComponent>(out Entity playerEntity))
                {
                    PositionComponent playerPosition = _entityManager.GetComponentData<PositionComponent>(playerEntity);

                    MapHandler mapHandler = Object.FindObjectOfType<MapHandler>();
                    if (mapHandler != null)
                    {
                        TileBase tile = mapHandler.GetTileAtPosition(playerPosition.Position);
                        if (tile.name == "StoneTile")
                        {
                            Entity audioEntity = entityCommandBuffer.CreateEntity();
                            entityCommandBuffer.AddComponent(audioEntity, new AudioComponent
                            {
                                Volume = 1,
                                Audio = AudioType.StepRock
                            });
                        }
                        else
                        {
                            Entity audioEntity = entityCommandBuffer.CreateEntity();
                            entityCommandBuffer.AddComponent(audioEntity, new AudioComponent
                            {
                                Volume = 1,
                                Audio = AudioType.StepGrass
                            });
                        }
                    }
                }
            }
        }

        entityCommandBuffer.Playback(_entityManager);
        entityCommandBuffer.Dispose();
    }
}