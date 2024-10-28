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

        foreach (var (_, experience, experienceGain, entity) in SystemAPI.Query<RefRO<LevelComponent>, RefRW<ExperienceComponent>, RefRO<ExperienceGainComponent>>().WithEntityAccess())
        {
            entityCommandBuffer.RemoveComponent<ExperienceGainComponent>(entity);

            experience.ValueRW.Experience += experienceGain.ValueRO.ExperienceGain;

            if (experience.ValueRO.Experience >= experience.ValueRO.ExperienceToNextLevel)
            {
                int experienceOverflow = Math.Max(0, experience.ValueRO.Experience - experience.ValueRO.ExperienceToNextLevel);
                entityCommandBuffer.AddComponent(entity, new LevelUpComponent { OverflowExperience = experienceOverflow });
            }
            else
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
