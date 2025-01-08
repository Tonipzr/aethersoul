using System;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

class AchievementAuthoring : MonoBehaviour
{
    public TextAsset Achievements;
}

class AchievementAuthoringBaker : Baker<AchievementAuthoring>
{
    public override void Bake(AchievementAuthoring authoring)
    {
        HashSet<int> usedIds = new HashSet<int>();

        AchievementDataCollection achievements = JsonUtility.FromJson<AchievementDataCollection>(authoring.Achievements.text);

        foreach (AchievementData achievement in achievements.Achievements)
        {
            if (usedIds.Contains(achievement.AchievementID))
            {
                Debug.LogError("Achievement with id " + achievement.AchievementID + " already exists.");
                continue;
            }

            usedIds.Add(achievement.AchievementID);

            Entity achievementEntity = CreateAdditionalEntity(TransformUsageFlags.None);

            AddComponent(achievementEntity, new AchievementComponent { AchievementID = achievement.AchievementID, IsCompleted = false });
            AddComponent(achievementEntity, new AchievementTriggerComponent { ShouldActivate = false, AchievementEntity = achievementEntity, TriggerProcessed = false });
            AddBuffer<TriggerComponent>(achievementEntity);

            string[] triggers = achievement.Trigger.Split(',');

            foreach (string trigger in triggers)
            {
                string[] triggerData = trigger.Split('=');

                if (triggerData.Length != 2)
                {
                    Debug.LogError("Invalid trigger data: " + trigger);
                    continue;
                }

                Enum.TryParse(triggerData[0], out TriggerType triggerType);

                int Value;
                int Value2;
                if (triggerData[1].Contains("|"))
                {
                    string[] triggerValues = triggerData[1].Split('|');

                    if (triggerValues.Length != 2)
                    {
                        Debug.LogError("Invalid trigger values: " + triggerData[1]);
                        continue;
                    }

                    Value = int.Parse(triggerValues[0]);
                    Value2 = int.Parse(triggerValues[1]);
                }
                else
                {
                    Value = int.Parse(triggerData[1]);
                    Value2 = 0;
                }

                AppendToBuffer(achievementEntity, new TriggerComponent { TriggerType = triggerType, Value = Value, Value2 = Value2, Satisfied = false });
            }

            string[] reward = achievement.Reward.Split('=');

            if (reward.Length != 2 && achievement.Reward != "None")
            {
                Debug.LogError("Invalid reward data: " + achievement.Reward);
                continue;
            }

            Enum.TryParse(reward[0], out OnCompleteRewards upgradeType);

            if (upgradeType == OnCompleteRewards.None)
            {
                AddComponent(achievementEntity, new OnCompleteRewardComponent
                {
                    Type = upgradeType,
                    Value = 0
                });
            }
            else
            {
                AddComponent(achievementEntity, new OnCompleteRewardComponent
                {
                    Type = upgradeType,
                    Value = int.Parse(reward[1])
                });
            }
        }
    }
}
