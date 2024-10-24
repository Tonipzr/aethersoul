using Unity.Burst;
using Unity.Collections;
using Unity.Entities;

partial struct LevelUpSystem : ISystem
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

        foreach (var (level, experience, levelUp, entity) in SystemAPI.Query<RefRW<LevelComponent>, RefRW<ExperienceComponent>, RefRO<LevelUpComponent>>().WithEntityAccess())
        {
            entityCommandBuffer.RemoveComponent<LevelUpComponent>(entity);

            experience.ValueRW.Experience = levelUp.ValueRO.OverflowExperience;
            level.ValueRW.Level++;
            experience.ValueRW.ExperienceToNextLevel = ExperienceToNextLevel.CalculateExperienceToNextLevel(level.ValueRO.Level);
        }

        entityCommandBuffer.Playback(_entityManager);
        entityCommandBuffer.Dispose();
    }

    [BurstCompile]
    public void OnDestroy(ref SystemState state)
    {

    }
}
