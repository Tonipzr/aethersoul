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
        }

        EntityCommandBuffer entityCommandBuffer = new EntityCommandBuffer(Allocator.Temp);

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

        entityCommandBuffer.Playback(_entityManager);
        entityCommandBuffer.Dispose();
    }
}
