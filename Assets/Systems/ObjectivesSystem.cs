using System;
using System.Collections.Generic;
using Unity.Entities;

partial struct ObjectivesSystem : ISystem
{
    private EntityManager _entityManager;

    private bool _initialized;

    public void OnCreate(ref SystemState state)
    {
    }

    private OnCompleteRewardComponent BuildRewardsComponent()
    {
        OnCompleteRewards type = BuildRewardType();

        OnCompleteRewardComponent rewardsComponent = new OnCompleteRewardComponent
        {
            Type = type,
            Value = BuildRewardValue(type)
        };

        return rewardsComponent;
    }

    private readonly OnCompleteRewards BuildRewardType()
    {
        Array values = Enum.GetValues(typeof(OnCompleteRewards));
        OnCompleteRewards randomReward = (OnCompleteRewards)values.GetValue(UnityEngine.Random.Range(0, values.Length));
        return randomReward;
    }

    private readonly int BuildRewardValue(OnCompleteRewards type)
    {
        var value = type switch
        {
            OnCompleteRewards.ExploreChance1 or OnCompleteRewards.ExploreChance2 or OnCompleteRewards.ExploreChance3 => UnityEngine.Random.Range(5, 10),
            OnCompleteRewards.AnySpellCostReduce => UnityEngine.Random.Range(5, 10),
            OnCompleteRewards.MoveSpeed => UnityEngine.Random.Range(10, 20),
            OnCompleteRewards.KillAnyDamage => UnityEngine.Random.Range(5, 25),
            OnCompleteRewards.ReduceExpPerLevel => UnityEngine.Random.Range(5, 10),
            OnCompleteRewards.GoldBonusOnKill => UnityEngine.Random.Range(5, 10),
            _ => 0,
        };

        return value;
    }

    public void OnUpdate(ref SystemState state)
    {
        if (!_initialized)
        {
            _entityManager = state.EntityManager;

            Dictionary<int, Entity> pickedObjectives = new Dictionary<int, Entity>();
            Dictionary<int, Entity> availableObjectives = new Dictionary<int, Entity>();
            foreach (var (objective, objectiveEntity) in SystemAPI.Query<RefRO<ObjectiveComponent>>().WithEntityAccess())
            {
                availableObjectives.Add(objective.ValueRO.ObjectiveID, objectiveEntity);
            }

            if (availableObjectives.Count == 0)
            {
                return;
            }

            for (int i = 0; i < 3; i++)
            {
                int randomIndex = UnityEngine.Random.Range(1, availableObjectives.Count);

                while (pickedObjectives.ContainsKey(randomIndex))
                {
                    randomIndex = UnityEngine.Random.Range(1, availableObjectives.Count);
                }

                pickedObjectives.Add(randomIndex, availableObjectives[randomIndex]);
            }

            foreach (var pickedObjective in pickedObjectives)
            {
                _entityManager.SetComponentEnabled<ObjectiveEnabledComponent>(pickedObjective.Value, true);
                _entityManager.AddComponent<OnCompleteRewardComponent>(pickedObjective.Value);
                _entityManager.SetComponentData(pickedObjective.Value, BuildRewardsComponent());
            }

            _initialized = true;
        }
        else
        {
            _entityManager = state.EntityManager;


            foreach (var (objectiveEnabledComponent, entity) in SystemAPI.Query<RefRW<ObjectiveEnabledComponent>>().WithEntityAccess())
            {
                if (objectiveEnabledComponent.ValueRO.Processed) continue;

                if (objectiveEnabledComponent.ValueRO.Completed)
                {
                    objectiveEnabledComponent.ValueRW.Processed = true;

                    if (SystemAPI.TryGetSingletonEntity<PlayerComponent>(out Entity playerEntity))
                    {
                        OnCompleteRewardComponent rewardComponent = _entityManager.GetComponentData<OnCompleteRewardComponent>(entity);
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
            }


            Entity mapEntity = _entityManager.CreateEntityQuery(typeof(MapEntityComponent)).GetSingletonEntity();
            MapEntityComponent mapEntityComponent = _entityManager.GetComponentData<MapEntityComponent>(mapEntity);

            var objectiveCheckJob = new ObjectivesCheckJob
            {
                CurrentSpellsUsed = mapEntityComponent.CurrentSpellsUsed,
                CurrentGoldCollected = mapEntityComponent.CurrentGoldCollected,
                CurrentGoldUsed = mapEntityComponent.CurrentGoldUsed,
                CurrentEnemiesKilled = mapEntityComponent.CurrentEnemiesKilled,
                CurrentEnemiesKilledNoDamage = mapEntityComponent.CurrentEnemiesKilledNoDamage,
                CurrentTraveledDistance = mapEntityComponent.CurrentTraveledDistance,
                CurrentPOIsVisited = mapEntityComponent.CurrentPOIsVisited,
                CurrentPOIsCleared = mapEntityComponent.CurrentPOIsCleared,
                CurrentBuffsCollected = mapEntityComponent.CurrentBuffsCollected,
                CurrentCheckpointsReached = mapEntityComponent.CurrentCheckpointsReached,
                CurrentLevelsUp = mapEntityComponent.CurrentLevelsUp,
                CurrentSpellsUnlocked = mapEntityComponent.CurrentSpellsUnlocked,
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
            objectiveCheckJob.Schedule();
        }
    }

    public void OnDestroy(ref SystemState state)
    {

    }
}
