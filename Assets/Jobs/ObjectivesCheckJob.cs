using Unity.Burst;
using Unity.Entities;

[BurstCompile]
public partial struct ObjectivesCheckJob : IJobEntity
{
    public int CurrentSpellsUsed;
    public int CurrentGoldCollected;
    public int CurrentGoldUsed;
    public int CurrentEnemiesKilled;
    public int CurrentEnemiesKilledNoDamage;
    public float CurrentTraveledDistance;
    public int CurrentPOIsVisited;
    public int CurrentPOIsCleared;
    public int CurrentBuffsCollected;
    public int CurrentCheckpointsReached;
    public int CurrentLevelsUp;
    public int CurrentSpellsUnlocked;
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

    public void Execute(ref DynamicBuffer<TriggerComponent> triggerComponent, ref ObjectiveEnabledComponent objectiveEnabled)
    {
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
                    if (CurrentEnemiesKilled >= trigger.Value)
                    {
                        trigger.Satisfied = true;
                        numSatisfied++;
                    }
                    break;
                case TriggerType.Gold:
                    if (CurrentGoldCollected >= trigger.Value)
                    {
                        trigger.Satisfied = true;
                        numSatisfied++;
                    }
                    break;
                case TriggerType.Clear:
                    if (trigger.Value == 3)
                    {
                        if (CurrentPOIsCleared >= trigger.Value2)
                        {
                            trigger.Satisfied = true;
                            numSatisfied++;
                        }
                    }
                    break;
                case TriggerType.AnySkillUsed:
                    if (CurrentSpellsUsed >= trigger.Value)
                    {
                        trigger.Satisfied = true;
                        numSatisfied++;
                    }
                    break;
                case TriggerType.GoldUsed:
                    if (CurrentGoldUsed >= trigger.Value)
                    {
                        trigger.Satisfied = true;
                        numSatisfied++;
                    }
                    break;
                case TriggerType.Explore:
                    if (trigger.Value == 1)
                    {
                        if (CurrentCheckpointsReached >= 1)
                        {
                            trigger.Satisfied = true;
                            numSatisfied++;
                        }
                    }

                    if (trigger.Value == 2)
                    {
                        if (CurrentBuffsCollected >= 1)
                        {
                            trigger.Satisfied = true;
                            numSatisfied++;
                        }
                    }

                    if (trigger.Value == 3)
                    {
                        if (CurrentPOIsVisited >= 1)
                        {
                            trigger.Satisfied = true;
                            numSatisfied++;
                        }
                    }
                    break;
                case TriggerType.Travel:
                    if (CurrentTraveledDistance >= trigger.Value)
                    {
                        trigger.Satisfied = true;
                        numSatisfied++;
                    }
                    break;
                case TriggerType.KillAnyNoDamage:
                    if (CurrentEnemiesKilledNoDamage >= trigger.Value)
                    {
                        trigger.Satisfied = true;
                        numSatisfied++;
                    }
                    break;
                case TriggerType.UnlockAllSpells:
                    if (CurrentSpellsUnlocked >= trigger.Value)
                    {
                        trigger.Satisfied = true;
                        numSatisfied++;
                    }
                    break;
                case TriggerType.Level:
                    if (CurrentLevelsUp >= trigger.Value)
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
            objectiveEnabled.Completed = true;
        }
        else
        {
            objectiveEnabled.Completed = false;
        }
    }
}
