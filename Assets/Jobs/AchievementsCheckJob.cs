using Unity.Burst;
using Unity.Entities;

[BurstCompile]
public partial struct AchievementsCheckJob : IJobEntity
{
    public int TotalSpellsUsed;
    public int TotalGoldCollected;
    public int TotalGoldUsed;
    public int TotalEnemiesKilled;
    public int TotalEnemiesKilledNoDamage;
    public float TotalTraveledDistance;
    public int TotalPOIsVisited;
    public int TotalPOIsCleared;
    public int TotalBuffsCollected;
    public int TotalCheckpointsReached;
    public int TotalLevelsUp;
    public int TotalSpellsUnlocked;

    public void Execute(ref DynamicBuffer<TriggerComponent> triggerComponent, ref AchievementComponent achievement, ref AchievementTriggerComponent achievementTriggerComponent)
    {
        if (achievement.IsProcessed) return;

        int numTriggers = triggerComponent.Length;
        int numSatisfied = 0;
        for (int i = 0; i < triggerComponent.Length; i++)
        {
            var trigger = triggerComponent[i];

            if (trigger.Satisfied)
            {
                numSatisfied++;
                continue;
            }

            switch (trigger.TriggerType)
            {
                case TriggerType.KillAny:
                    if (TotalEnemiesKilled >= trigger.Value)
                    {
                        trigger.Satisfied = true;
                        numSatisfied++;
                    }
                    break;
                case TriggerType.Gold:
                    if (TotalGoldCollected >= trigger.Value)
                    {
                        trigger.Satisfied = true;
                        numSatisfied++;
                    }
                    break;
                case TriggerType.Clear:
                    if (trigger.Value == 3)
                    {
                        if (TotalPOIsCleared >= trigger.Value2)
                        {
                            trigger.Satisfied = true;
                            numSatisfied++;
                        }
                    }
                    break;
                case TriggerType.AnySkillUsed:
                    if (TotalSpellsUsed >= trigger.Value)
                    {
                        trigger.Satisfied = true;
                        numSatisfied++;
                    }
                    break;
                case TriggerType.GoldUsed:
                    if (TotalGoldUsed >= trigger.Value)
                    {
                        trigger.Satisfied = true;
                        numSatisfied++;
                    }
                    break;
                case TriggerType.Explore:
                    if (trigger.Value == 1)
                    {
                        if (TotalCheckpointsReached >= 1)
                        {
                            trigger.Satisfied = true;
                            numSatisfied++;
                        }
                    }

                    if (trigger.Value == 2)
                    {
                        if (TotalBuffsCollected >= 1)
                        {
                            trigger.Satisfied = true;
                            numSatisfied++;
                        }
                    }

                    if (trigger.Value == 3)
                    {
                        if (TotalPOIsVisited >= 1)
                        {
                            trigger.Satisfied = true;
                            numSatisfied++;
                        }
                    }
                    break;
                case TriggerType.Travel:
                    if (TotalTraveledDistance >= trigger.Value)
                    {
                        trigger.Satisfied = true;
                        numSatisfied++;
                    }
                    break;
                case TriggerType.KillAnyNoDamage:
                    if (TotalEnemiesKilledNoDamage >= trigger.Value)
                    {
                        trigger.Satisfied = true;
                        numSatisfied++;
                    }
                    break;
                case TriggerType.UnlockAllSpells:
                    if (TotalSpellsUnlocked >= trigger.Value)
                    {
                        trigger.Satisfied = true;
                        numSatisfied++;
                    }
                    break;
                case TriggerType.Level:
                    if (TotalLevelsUp >= trigger.Value)
                    {
                        trigger.Satisfied = true;
                        numSatisfied++;
                    }
                    break;
            }

            triggerComponent[i] = trigger;
        }

        if (numSatisfied == numTriggers)
        {
            achievement.IsCompleted = true;
            achievementTriggerComponent.ShouldActivate = true;
            achievementTriggerComponent.TriggerProcessed = false;
        }
        else
        {
            achievement.IsCompleted = false;
            achievementTriggerComponent.ShouldActivate = false;
            achievementTriggerComponent.TriggerProcessed = false;
        }
    }
}
