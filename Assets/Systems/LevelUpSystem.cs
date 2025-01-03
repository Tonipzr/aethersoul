using Unity.Collections;
using Unity.Entities;

partial class LevelUpSystem : SystemBase
{
    private EntityManager _entityManager;

    protected override void OnCreate()
    {
    }

    protected override void OnUpdate()
    {
        _entityManager = EntityManager;

        EntityCommandBuffer entityCommandBuffer = new EntityCommandBuffer(Allocator.Temp);

        foreach (var (level, experience, levelUp, entity) in SystemAPI.Query<RefRW<LevelComponent>, RefRW<ExperienceComponent>, RefRO<LevelUpComponent>>().WithEntityAccess())
        {
            entityCommandBuffer.RemoveComponent<LevelUpComponent>(entity);

            experience.ValueRW.Experience = levelUp.ValueRO.OverflowExperience;
            level.ValueRW.Level++;

            int reduction = 0;
            if (_entityManager.HasComponent<ActiveUpgradesComponent>(entity))
            {
                var activeUpgrades = _entityManager.GetBuffer<ActiveUpgradesComponent>(entity);

                foreach (var upgrade in activeUpgrades)
                {
                    if (upgrade.Type == UpgradeType.ReduceExpPerLevel)
                    {
                        reduction += (int)upgrade.Value;
                    }
                }
            }

            experience.ValueRW.ExperienceToNextLevel = ExperienceToNextLevel.CalculateExperienceToNextLevel(level.ValueRO.Level, reduction);

            var jobLevel = new UpdateMapStatsJob
            {
                Type = MapStatsType.CurrentLevelsUp,
                Value = 1,
                Incremental = true
            };
            jobLevel.Schedule();

            entityCommandBuffer.AddComponent(entity, new ExperienceUpdatedComponent { CurrentExperience = experience.ValueRW.Experience, MaxExperience = experience.ValueRW.ExperienceToNextLevel, CurrentLevelUpdated = true });

            int coins = 5 + (level.ValueRO.Level - 1);
            if (_entityManager.HasComponent<ActiveUpgradesComponent>(entity))
            {
                var activeUpgrades = _entityManager.GetBuffer<ActiveUpgradesComponent>(entity);

                foreach (var upgrade in activeUpgrades)
                {
                    if (upgrade.Type == UpgradeType.GoldBonusOnKill)
                    {
                        coins = (int)(coins * (1 + ((float)upgrade.Value / 100)));
                    }
                }
            }
            DreamCityStatsGameObject.IncreaseCoins(coins);

            var jobGold = new UpdateMapStatsJob
            {
                Type = MapStatsType.CurrentGoldCollected,
                Value = coins,
                Incremental = true
            };
            jobGold.Schedule();
        }

        entityCommandBuffer.Playback(_entityManager);
        entityCommandBuffer.Dispose();
    }

    protected override void OnDestroy()
    {

    }
}
