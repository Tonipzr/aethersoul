using Unity.Burst;
using Unity.Entities;

partial struct VelocitySystem : ISystem
{
    private EntityManager _entityManager;

    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
    }

    public void OnUpdate(ref SystemState state)
    {
        _entityManager = state.EntityManager;

        foreach (var (velocity, entity) in SystemAPI.Query<RefRW<VelocityComponent>>().WithEntityAccess())
        {
            float velocityCalc = velocity.ValueRO.BaseVelocity;

            if (_entityManager.HasComponent<PlayerComponent>(entity))
            {
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
            }

            if (_entityManager.HasComponent<MonsterComponent>(entity))
            {
                velocityCalc = velocity.ValueRO.BaseVelocity * (PlayerPrefsManager.Instance.GetMonsterSpeed() / 100f);
            }

            velocity.ValueRW.Velocity = velocityCalc;
        }
    }

    [BurstCompile]
    public void OnDestroy(ref SystemState state)
    {

    }
}
