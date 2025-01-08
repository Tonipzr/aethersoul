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
            UIManager.Instance.UpdateExp(0, ExperienceToNextLevel.CalculateExperienceToNextLevel(1, 0));
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
            UIManager.Instance.UpdateExp(experience.ValueRO.CurrentExperience, experience.ValueRO.MaxExperience);

            entityCommandBuffer.RemoveComponent<ExperienceUpdatedComponent>(entity);
        }

        foreach (var (level, _, entity) in SystemAPI.Query<RefRO<LevelUpdatedComponent>, RefRO<PlayerComponent>>().WithEntityAccess())
        {
            UIManager.Instance.UpdateLevel(level.ValueRO.NewLevel);
            UIManager.Instance.UpdateCoins(DreamCityStatsGameObject.CurrentCoins);
            UIManager.Instance.ToggleCardPicker();

            entityCommandBuffer.RemoveComponent<LevelUpdatedComponent>(entity);
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

        foreach (var (_, selectedSpellsBuffer) in SystemAPI.Query<RefRO<PlayerComponent>, DynamicBuffer<SelectedSpellsComponent>>())
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

        if (SystemAPI.TryGetSingletonEntity<LoreEntityComponent>(out Entity loreEntity))
        {
            DynamicBuffer<LoreEntityComponent> lore = _entityManager.GetBuffer<LoreEntityComponent>(loreEntity);

            for (int i = 0; i < lore.Length; i++)
            {
                if (lore[i].Type == LoreType.MapPosition)
                {
                    UIManager.Instance.ShowMiddleText(lore[i].Data2);
                }

                if (lore[i].Type == LoreType.Story)
                {
                    UIManager.Instance.ShowLore((WhoSpeaksLores)lore[i].Data, lore[i].Data2);
                }
            }

            lore.Clear();
        }

        bool isInteracting = false;
        foreach (var (uiInteract, entity) in SystemAPI.Query<RefRO<UIInteractComponent>>().WithEntityAccess())
        {
            UIManager.Instance.ToggleInteractImage(true);
            isInteracting = true;

            entityCommandBuffer.DestroyEntity(entity);
        }

        if (!isInteracting)
        {
            UIManager.Instance.ToggleInteractImage(false);
        }

        foreach (var (achievementTriggerComponent, achievementEntity) in SystemAPI.Query<RefRW<AchievementTriggerComponent>>().WithEntityAccess())
        {
            if (!achievementTriggerComponent.ValueRO.ShouldActivate) continue;
            if (achievementTriggerComponent.ValueRO.TriggerProcessed) continue;

            if (!SystemAPI.HasComponent<AchievementComponent>(achievementEntity)) continue;

            AchievementComponent achievementComponent = _entityManager.GetComponentData<AchievementComponent>(achievementEntity);

            achievementTriggerComponent.ValueRW.TriggerProcessed = true;
            UIManager.Instance.ShowAchievement(achievementComponent.AchievementID);
        }

        entityCommandBuffer.Playback(_entityManager);
        entityCommandBuffer.Dispose();
    }
}
