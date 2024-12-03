using Unity.Burst;
using Unity.Collections;
using Unity.Entities;

partial struct InvulnerableStateSystem : ISystem
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

        foreach (var (invulnerable, entity) in SystemAPI.Query<RefRW<InvulnerableStateComponent>>().WithEntityAccess())
        {
            invulnerable.ValueRW.ElapsedTime += SystemAPI.Time.DeltaTime;

            if (invulnerable.ValueRO.Duration <= invulnerable.ValueRO.ElapsedTime)
            {
                entityCommandBuffer.SetComponentEnabled<InvulnerableStateComponent>(entity, false);
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
