using Unity.Burst;
using Unity.Collections;
using Unity.Entities;

partial struct LeechCooldownSystem : ISystem
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

        foreach (var (healthCooldownComponent, entity) in SystemAPI.Query<RefRW<HealthLeechCooldownComponent>>().WithEntityAccess())
        {
            healthCooldownComponent.ValueRW.CurrentTimeOnCooldown += deltaTime;

            if (healthCooldownComponent.ValueRW.CurrentTimeOnCooldown >= healthCooldownComponent.ValueRW.Cooldown)
            {
                healthCooldownComponent.ValueRW.CurrentTimeOnCooldown = 0;
                entityCommandBuffer.SetComponentEnabled<HealthLeechCooldownComponent>(entity, false);
            }
        }

        foreach (var (manaCooldownComponent, entity) in SystemAPI.Query<RefRW<ManaLeechCooldownComponent>>().WithEntityAccess())
        {
            manaCooldownComponent.ValueRW.CurrentTimeOnCooldown += deltaTime;

            if (manaCooldownComponent.ValueRW.CurrentTimeOnCooldown >= manaCooldownComponent.ValueRW.Cooldown)
            {
                manaCooldownComponent.ValueRW.CurrentTimeOnCooldown = 0;
                entityCommandBuffer.SetComponentEnabled<ManaLeechCooldownComponent>(entity, false);
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
