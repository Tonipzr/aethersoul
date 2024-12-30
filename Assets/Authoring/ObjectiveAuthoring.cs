using System;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

class ObjectiveAuthoring : MonoBehaviour
{
    public TextAsset Objectives;
}

class ObjectiveAuthoringBaker : Baker<ObjectiveAuthoring>
{
    public override void Bake(ObjectiveAuthoring authoring)
    {
        HashSet<int> usedIds = new HashSet<int>();

        ObjectiveDataCollection objectives = JsonUtility.FromJson<ObjectiveDataCollection>(authoring.Objectives.text);

        foreach (ObjectiveData objective in objectives.Objectives)
        {
            if (usedIds.Contains(objective.ObjectiveID))
            {
                Debug.LogError("Objective with id " + objective.ObjectiveID + " already exists.");
                continue;
            }

            usedIds.Add(objective.ObjectiveID);

            Entity objectEntity = CreateAdditionalEntity(TransformUsageFlags.None);

            AddComponent(objectEntity, new ObjectiveComponent { ObjectiveID = objective.ObjectiveID });
            AddComponent(objectEntity, new ObjectiveEnabledComponent { Completed = false, Processed = false });
            SetComponentEnabled<ObjectiveEnabledComponent>(objectEntity, false);
            AddBuffer<TriggerComponent>(objectEntity);

            List<string> triggerTypes = new List<string>();

            string[] triggers = objective.Trigger.Split(',');

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

                AppendToBuffer(objectEntity, new TriggerComponent { TriggerType = triggerType, Value = Value, Value2 = Value2, Satisfied = false });
            }
        }
    }
}
