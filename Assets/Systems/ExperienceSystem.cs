using System;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;

partial struct ExperienceSystem : ISystem
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

        foreach (var (_, experience, experienceGain, entity) in SystemAPI.Query<RefRO<LevelComponent>, RefRW<ExperienceComponent>, DynamicBuffer<ExperienceGainComponent>>().WithEntityAccess())
        {
            bool levelUp = false;
            bool experienceUpdated = false;
            for (int i = 0; i < experienceGain.Length; i++)
            {
                experience.ValueRW.Experience += experienceGain[i].ExperienceGain;
                experienceUpdated = true;

                if (experience.ValueRO.Experience >= experience.ValueRO.ExperienceToNextLevel)
                {
                    int experienceOverflow = Math.Max(0, experience.ValueRO.Experience - experience.ValueRO.ExperienceToNextLevel);
                    entityCommandBuffer.AddComponent(entity, new LevelUpComponent { OverflowExperience = experienceOverflow });
                    levelUp = true;
                    experienceUpdated = false;
                    experienceGain.RemoveAt(i);
                    break;
                }
                experienceGain.RemoveAt(i);
            }

            if (!levelUp && experienceUpdated)
            {
                entityCommandBuffer.AddComponent(entity, new ExperienceUpdatedComponent { CurrentExperience = experience.ValueRW.Experience, MaxExperience = experience.ValueRW.ExperienceToNextLevel });
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
