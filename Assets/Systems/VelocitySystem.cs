using Unity.Burst;
using Unity.Entities;

partial struct VelocitySystem : ISystem
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

        foreach (var (velocity, entity) in SystemAPI.Query<RefRW<VelocityComponent>>().WithEntityAccess())
        {
            if (_entityManager.HasComponent<PlayerComponent>(entity))
            {
                float velocityCalc = velocity.ValueRO.BaseVelocity;

                if (_entityManager.HasComponent<ActiveUpgradesComponent>(entity))
                {
                    var activeUpgrades = _entityManager.GetBuffer<ActiveUpgradesComponent>(entity);

                    foreach (var upgrade in activeUpgrades)
                    {
                        if (upgrade.Type == UpgradeType.MoveSpeed)
                        {
                            velocityCalc *= 1 + (upgrade.Value / 100);
                        }
                    }
                }

                velocity.ValueRW.Velocity = velocityCalc;
            }
        }
    }

    [BurstCompile]
    public void OnDestroy(ref SystemState state)
    {

    }
}
