using Unity.Burst;
using Unity.Collections;
using Unity.Entities;

// This is a DEBUG system that mocks damage and healing to entities with HealthComponent.
partial struct HealthToLogSystem : ISystem
{
    private EntityManager _entityManager;

    public void OnUpdate(ref SystemState state)
    {
        _entityManager = state.EntityManager;

        EntityCommandBuffer entityCommandBuffer = new EntityCommandBuffer(Allocator.Temp);

        foreach (var (health, entity) in SystemAPI.Query<RefRW<HealthComponent>>().WithEntityAccess())
        {
            UnityEngine.Debug.Log("Entity: " + entity.Index + " Current Health: " + health.ValueRO.CurrentHealth + " Max Health: " + health.ValueRO.MaxHealth);

            int random = UnityEngine.Random.Range(0, 100);

            if (random < 5)
            {
                entityCommandBuffer.AddComponent(entity, new DamageComponent { DamageAmount = 5 });
                entityCommandBuffer.AddComponent(entity, new HealthRestoreComponent { HealAmount = 3 });
            }
        }

        entityCommandBuffer.Playback(_entityManager);
        entityCommandBuffer.Dispose();
    }
}
