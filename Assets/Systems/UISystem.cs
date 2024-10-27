using Unity.Collections;
using Unity.Entities;

partial class UISystem : SystemBase
{
    private EntityManager _entityManager;
    private bool _isInitialized;

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
            UIManager.Instance.UpdateSpellSlot(1, "X");
            UIManager.Instance.UpdateSpellSlot(2, "X");
            UIManager.Instance.UpdateSpellSlot(3, "X");
            UIManager.Instance.UpdateSpellSlot(4, "X");
        }

        EntityCommandBuffer entityCommandBuffer = new EntityCommandBuffer(Allocator.Temp);

        Entity inputEntity = SystemAPI.GetSingletonEntity<InputComponent>();
        InputComponent inputComponent = _entityManager.GetComponentData<InputComponent>(inputEntity);

        if (inputComponent.pressingSpellBookToggle)
        {
            UIManager.Instance.ToggleSpellBook();
        }

        foreach (var (health, _, entity) in SystemAPI.Query<RefRO<HealthUpdatedComponent>, RefRO<PlayerComponent>>().WithEntityAccess())
        {
            UIManager.Instance.UpdateHP(health.ValueRO.CurrentHealth, health.ValueRO.MaxHealth);

            entityCommandBuffer.RemoveComponent<HealthUpdatedComponent>(entity);
        }

        foreach (var (experience, _, entity) in SystemAPI.Query<RefRO<ExperienceUpdatedComponent>, RefRO<PlayerComponent>>().WithEntityAccess())
        {
            UIManager.Instance.UpdateMana(experience.ValueRO.CurrentExperience, experience.ValueRO.MaxExperience);

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

        entityCommandBuffer.Playback(_entityManager);
        entityCommandBuffer.Dispose();
    }
}
