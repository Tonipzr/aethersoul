using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using UnityEngine;

partial struct HandleBuffInteractSystem : ISystem
{
    private EntityManager _entityManager;

    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {

    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        _entityManager = state.EntityManager;

        EntityCommandBuffer entityCommandBuffer = new EntityCommandBuffer(Allocator.Temp);

        Entity inputEntity = SystemAPI.GetSingletonEntity<InputComponent>();
        InputComponent inputComponent = _entityManager.GetComponentData<InputComponent>(inputEntity);

        if (inputComponent.pressingInteract)
        {
            if (!SystemAPI.TryGetSingletonEntity<PlayerComponent>(out Entity playerEntity)) return;
            PositionComponent playerPositionComponent = _entityManager.GetComponentData<PositionComponent>(playerEntity);

            foreach (var (buff, _) in SystemAPI.Query<RefRW<MapBuffEntityComponent>>().WithEntityAccess())
            {
                if (buff.ValueRO.IsUsed) continue;

                Vector2 playerPosition = new Vector2(playerPositionComponent.Position.x, playerPositionComponent.Position.y);
                Vector2 buffPosition = new Vector2(buff.ValueRO.Coordinates.x, buff.ValueRO.Coordinates.y);

                if (Vector3.Distance(playerPosition, buffPosition) < 1.5f)
                {
                    buff.ValueRW.IsUsed = true;

                    var jobBuffs = new UpdateMapStatsJob
                    {
                        Type = MapStatsType.CurrentBuffsCollected,
                        Value = 1,
                        Incremental = true
                    };
                    jobBuffs.Schedule();

                    if (SystemAPI.HasComponent<ExperienceComponent>(playerEntity))
                    {
                        ExperienceComponent experienceComponent = _entityManager.GetComponentData<ExperienceComponent>(playerEntity);

                        entityCommandBuffer.AddComponent(playerEntity, new ExperienceGainComponent
                        {
                            ExperienceGain = experienceComponent.ExperienceToNextLevel - experienceComponent.Experience
                        });
                    }

                    foreach (var (experienceShard, experienceShardEntity) in SystemAPI.Query<RefRW<ExperienceShardEntityComponent>>().WithEntityAccess())
                    {
                        if (experienceShard.ValueRO.IsProcessed) continue;

                        entityCommandBuffer.AddComponent(experienceShardEntity, new FollowComponent
                        {
                            Target = playerEntity,
                            MinDistance = 0,
                            MaxDistance = 0
                        });
                    }
                }
            }
        }

        entityCommandBuffer.Playback(_entityManager);
        entityCommandBuffer.Dispose();
    }

    [BurstCompile]
    public void OnDestroy(ref SystemState state)
    {

    }
}
