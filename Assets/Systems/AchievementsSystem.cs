using System;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Entities.UniversalDelegates;
using UnityEngine.AI;

partial struct AchievementsSystem : ISystem
{
    private EntityManager _entityManager;

    public void OnCreate(ref SystemState state)
    {
    }

    public void OnUpdate(ref SystemState state)
    {
        _entityManager = state.EntityManager;


        foreach (var (achievementComponent, entity) in SystemAPI.Query<RefRW<AchievementComponent>>().WithEntityAccess())
        {
            if (achievementComponent.ValueRO.IsProcessed) continue;

            if (!achievementComponent.ValueRO.IsCompleted) continue;

            achievementComponent.ValueRW.IsProcessed = true;

            if (SystemAPI.TryGetSingletonEntity<PlayerComponent>(out Entity playerEntity))
            {
                OnCompleteRewardComponent rewardComponent = _entityManager.GetComponentData<OnCompleteRewardComponent>(entity);

                if (rewardComponent.Type == OnCompleteRewards.None) continue;

                Enum.TryParse(rewardComponent.Type.ToString(), out UpgradeType upgradeType);

                var buffer = _entityManager.GetBuffer<ActiveUpgradesComponent>(playerEntity);

                var upgradeID = upgradeType switch
                {
                    UpgradeType.ExploreChance1 => 18,
                    UpgradeType.ExploreChance2 => 19,
                    UpgradeType.ExploreChance3 => 20,
                    UpgradeType.AnySpellCostReduce => 21,
                    UpgradeType.MoveSpeed => 22,
                    UpgradeType.KillAnyDamage => 23,
                    UpgradeType.ReduceExpPerLevel => 24,
                    UpgradeType.GoldBonusOnKill => 1,
                    _ => 0,
                };

                float upgradeLevel = 0;
                for (int i = 0; i < buffer.Length; i++)
                {
                    if (buffer[i].UpgradeID == upgradeID)
                    {
                        upgradeLevel = buffer[i].Value;
                        buffer.RemoveAt(i);
                        break;
                    }
                }

                buffer.Add(new ActiveUpgradesComponent { UpgradeID = upgradeID, Type = upgradeType, Value = rewardComponent.Value + upgradeLevel });
            }
        }

        Entity mapEntity = _entityManager.CreateEntityQuery(typeof(MapEntityComponent)).GetSingletonEntity();
        MapEntityComponent mapEntityComponent = _entityManager.GetComponentData<MapEntityComponent>(mapEntity);

        var achievementCheckJob = new AchievementsCheckJob
        {
            TotalSpellsUsed = mapEntityComponent.TotalSpellsUsed,
            TotalGoldCollected = mapEntityComponent.TotalGoldCollected,
            TotalGoldUsed = mapEntityComponent.TotalGoldUsed,
            TotalEnemiesKilled = mapEntityComponent.TotalEnemiesKilled,
            TotalEnemiesKilledNoDamage = mapEntityComponent.TotalEnemiesKilledNoDamage,
            TotalTraveledDistance = mapEntityComponent.TotalTraveledDistance,
            TotalPOIsVisited = mapEntityComponent.TotalPOIsVisited,
            TotalPOIsCleared = mapEntityComponent.TotalPOIsCleared,
            TotalBuffsCollected = mapEntityComponent.TotalBuffsCollected,
            TotalCheckpointsReached = mapEntityComponent.TotalCheckpointsReached,
            TotalLevelsUp = mapEntityComponent.TotalLevelsUp,
            TotalSpellsUnlocked = mapEntityComponent.TotalSpellsUnlocked
        };
        achievementCheckJob.Schedule();
    }

    public void OnDestroy(ref SystemState state)
    {

    }
}
