using Unity.Collections;
using Unity.Entities;

// This is a DEBUG system that mocks experience to entities.
partial struct ExperienceToLogSystem : ISystem
{
    private EntityManager _entityManager;

    public void OnUpdate(ref SystemState state)
    {
        _entityManager = state.EntityManager;

        EntityCommandBuffer entityCommandBuffer = new EntityCommandBuffer(Allocator.Temp);

        foreach (var (level, experience, entity) in SystemAPI.Query<RefRO<LevelComponent>, RefRW<ExperienceComponent>>().WithEntityAccess())
        {
            UnityEngine.Debug.Log("Entity: " + entity.Index + " Current Experience: " + experience.ValueRO.Experience + " Level: " + level.ValueRO.Level);

            int random = UnityEngine.Random.Range(0, 100);

            if (random < 5)
            {
                entityCommandBuffer.AddComponent(entity, new ExperienceGainComponent { ExperienceGain = 3 });
            }
        }

        entityCommandBuffer.Playback(_entityManager);
        entityCommandBuffer.Dispose();
    }
}
