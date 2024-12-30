using System.Collections.Generic;
using Unity.Collections;
using Unity.Entities;

partial class UISystem : SystemBase
{
    private EntityManager _entityManager;
    private bool _isInitialized;
    private bool objectivesAssigned;

    protected override void OnCreate()
    {
    }

    protected override void OnUpdate()
    {
        if (UIManager.Instance == null) return;

        if (!_isInitialized)
        {
            _isInitialized = true;
            _entityManager = EntityManager;

            UIManager.Instance.UpdateHP(100, 100);
            UIManager.Instance.UpdateMana(100, 100);
            UIManager.Instance.UpdateExp(0, ExperienceToNextLevel.CalculateExperienceToNextLevel(1));
            UIManager.Instance.UpdateSpellSlot(1, "0");
            UIManager.Instance.UpdateSpellSlot(2, "0");
            UIManager.Instance.UpdateSpellSlot(3, "0");
            UIManager.Instance.UpdateSpellSlot(4, "0");
            UIManager.Instance.UpdateCoins(DreamCityStatsGameObject.CurrentCoins);
        }

        EntityCommandBuffer entityCommandBuffer = new EntityCommandBuffer(Allocator.Temp);

        Entity inputEntity = SystemAPI.GetSingletonEntity<InputComponent>();
        InputComponent inputComponent = _entityManager.GetComponentData<InputComponent>(inputEntity);

        if (inputComponent.pressingSpellBookToggle)
        {
            UIManager.Instance.ToggleSpellBook();
        }

        if (inputComponent.pressingOpenMenu)
        {
            UIManager.Instance.ToggleMenu();
        }

        foreach (var (health, _, entity) in SystemAPI.Query<RefRO<HealthUpdatedComponent>, RefRO<PlayerComponent>>().WithEntityAccess())
        {
            UIManager.Instance.UpdateHP(health.ValueRO.CurrentHealth, health.ValueRO.MaxHealth);

            entityCommandBuffer.RemoveComponent<HealthUpdatedComponent>(entity);
        }

        foreach (var (experience, _, entity) in SystemAPI.Query<RefRO<ExperienceUpdatedComponent>, RefRO<PlayerComponent>>().WithEntityAccess())
        {
            UIManager.Instance.UpdateExp(experience.ValueRO.CurrentExperience, experience.ValueRO.MaxExperience, experience.ValueRO.CurrentLevelUpdated);

            if (experience.ValueRO.CurrentLevelUpdated)
            {
                UIManager.Instance.UpdateCoins(DreamCityStatsGameObject.CurrentCoins);

                UIManager.Instance.ToggleCardPicker();
            }

            entityCommandBuffer.RemoveComponent<ExperienceUpdatedComponent>(entity);
        }

        foreach (var (mana, _, entity) in SystemAPI.Query<RefRO<ManaUpdatedComponent>, RefRO<PlayerComponent>>().WithEntityAccess())
        {
            UIManager.Instance.UpdateMana(mana.ValueRO.CurrentMana, mana.ValueRO.MaxMana);

            entityCommandBuffer.RemoveComponent<ManaUpdatedComponent>(entity);
        }

        foreach (var (availableSpells, _) in SystemAPI.Query<DynamicBuffer<PlayerAvailableSpellsComponent>, RefRO<PlayerComponent>>())
        {
            if (availableSpells.Length == 0) continue;

            for (int i = 0; i < availableSpells.Length; i++)
            {
                UIManager.Instance.LearnSpell(availableSpells[i].SpellID);
            }
        }

        foreach (var (_, selectedSpellsBuffer) in SystemAPI.Query<RefRO<PlayerComponent>, DynamicBuffer<PlayerSelectedSpellsComponent>>())
        {
            for (int i = 0; i < selectedSpellsBuffer.Length; i++)
            {
                UIManager.Instance.UpdateSpellSlot(i + 1, selectedSpellsBuffer[i].SpellID.ToString());
            }
        }

        foreach (var (spell, _, timer) in SystemAPI.Query<RefRO<SpellComponent>, RefRO<SpellOnCooldownComponent>, RefRO<TimeCounterComponent>>())
        {
            UIManager.Instance.UpdateSpellCooldown(spell.ValueRO.SpellID, timer.ValueRO.ElapsedTime, timer.ValueRO.EndTime);
        }

        if (!objectivesAssigned)
        {
            List<int> objectiveIDs = new List<int>();
            foreach (var (objective, _) in SystemAPI.Query<RefRO<ObjectiveComponent>, RefRO<ObjectiveEnabledComponent>>())
            {
                objectiveIDs.Add(objective.ValueRO.ObjectiveID);
            }

            if (objectiveIDs.Count == 3)
            {
                UIManager.Instance.AddObjectives(objectiveIDs.ToArray());
                objectivesAssigned = true;
            }
        }
        else
        {
            foreach (var (objectiveComponent, objectiveEnabled, objectiveEntity) in SystemAPI.Query<RefRO<ObjectiveComponent>, RefRO<ObjectiveEnabledComponent>>().WithEntityAccess())
            {
                if (objectiveEnabled.ValueRO.Completed && objectiveEnabled.ValueRO.Processed)
                {
                    UIManager.Instance.CompleteObjective(objectiveComponent.ValueRO.ObjectiveID);

                    entityCommandBuffer.SetComponentEnabled<ObjectiveEnabledComponent>(objectiveEntity, false);
                }
            }
        }

        entityCommandBuffer.Playback(_entityManager);
        entityCommandBuffer.Dispose();
    }
}
