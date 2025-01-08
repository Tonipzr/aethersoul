using Unity.Burst;
using Unity.Collections;
using Unity.Entities;

partial struct DashCooldownSystem : ISystem
{
    private EntityManager _entityManager;

    public void OnCreate(ref SystemState state)
    {
    }

    public void OnUpdate(ref SystemState state)
    {
        _entityManager = state.EntityManager;
        float deltaTime = SystemAPI.Time.DeltaTime;

        EntityCommandBuffer entityCommandBuffer = new EntityCommandBuffer(Allocator.Temp);

        foreach (var (dashCooldownComponent, entity) in SystemAPI.Query<RefRW<DashCooldownComponent>>().WithEntityAccess())
        {
            dashCooldownComponent.ValueRW.CurrentTimeOnCooldown += deltaTime;

            if (dashCooldownComponent.ValueRW.CurrentTimeOnCooldown >= dashCooldownComponent.ValueRW.Cooldown)
            {
                dashCooldownComponent.ValueRW.CurrentTimeOnCooldown = 0;
                entityCommandBuffer.SetComponentEnabled<DashCooldownComponent>(entity, false);
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
