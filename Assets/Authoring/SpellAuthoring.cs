using System;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

class SpellAuthoring : MonoBehaviour
{
    public TextAsset Spells;
}

class SpellAuthoringBaker : Baker<SpellAuthoring>
{
    public override void Bake(SpellAuthoring authoring)
    {
        HashSet<int> usedIds = new HashSet<int>();

        SpellDataCollection spells = JsonUtility.FromJson<SpellDataCollection>(authoring.Spells.text);

        foreach (SpellData spell in spells.Spells)
        {
            if (usedIds.Contains(spell.SpellID))
            {
                Debug.LogError("Spell with id " + spell.SpellID + " already exists.");
                continue;
            }

            usedIds.Add(spell.SpellID);

            Debug.Log("Found spell: " + spell.Name + " with id " + spell.SpellID);

            Entity spellEntity = CreateAdditionalEntity(TransformUsageFlags.None);

            Enum.TryParse(spell.SpellType, out SpellType spellType);
            Enum.TryParse(spell.Range, out SpellRange spellRange);
            Enum.TryParse(spell.Target, out SpellTarget spellTarget);
            Enum.TryParse(spell.Element, out SpellElement element);

            AddComponent(spellEntity, new SpellComponent { SpellID = spell.SpellID, SpellType = spellType });

            if (spellType != SpellType.Passive)
            {
                AddComponent(spellEntity, new SpellTargetComponent { Target = spellTarget });
                AddComponent(spellEntity, new SpellRangeComponent { Range = spellRange });
                AddComponent(spellEntity, new SpellDamageComponent { Damage = spell.Damage });
                AddComponent(spellEntity, new SpellCostComponent { Cost = spell.Cost });
                AddComponent(spellEntity, new SpellCooldownComponent { Cooldown = spell.Cooldown });
                AddComponent(spellEntity, new SpellElementComponent { Element = element });
            }
        }
    }
}
