using Unity.Burst;
using Unity.Collections;
using Unity.Entities;

partial struct SpellCastSystem : ISystem
{
    private EntityManager _entityManager;

    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {

    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        _entityManager = state.EntityManager;

        Entity inputEntity = SystemAPI.GetSingletonEntity<InputComponent>();
        InputComponent inputComponent = _entityManager.GetComponentData<InputComponent>(inputEntity);
        Entity playerEntity = SystemAPI.GetSingletonEntity<PlayerComponent>();
        DynamicBuffer<PlayerSelectedSpellsComponent> selectedSpellsBuffer = _entityManager.GetBuffer<PlayerSelectedSpellsComponent>(playerEntity);

        if (selectedSpellsBuffer.Length == 0)
        {
            return;
        }

        if (inputComponent.pressingSpellSlot1 && selectedSpellsBuffer.Length >= 1)
        {
            CastSpell(selectedSpellsBuffer[0].SpellID, ref state);
        }

        if (inputComponent.pressingSpellSlot2 && selectedSpellsBuffer.Length >= 2)
        {
            CastSpell(selectedSpellsBuffer[1].SpellID, ref state);
        }

        if (inputComponent.pressingSpellSlot3 && selectedSpellsBuffer.Length >= 3)
        {
            CastSpell(selectedSpellsBuffer[2].SpellID, ref state);
        }

        if (inputComponent.pressingSpellSlot4 && selectedSpellsBuffer.Length >= 4)
        {
            CastSpell(selectedSpellsBuffer[3].SpellID, ref state);
        }
    }

    [BurstCompile]
    private void CastSpell(int spellID, ref SystemState state)
    {
        EntityCommandBuffer entityCommandBuffer = new EntityCommandBuffer(Allocator.Temp);

        foreach (var (spell, cooldown, _, entity) in SystemAPI.Query<RefRO<SpellComponent>, RefRO<SpellCooldownComponent>, RefRO<SpellOnCooldownComponent>>().WithEntityAccess().WithOptions(EntityQueryOptions.IgnoreComponentEnabledState))
        {
            if (spell.ValueRO.SpellID != spellID) continue;

            if (_entityManager.IsComponentEnabled<SpellOnCooldownComponent>(entity))
            {
                continue;
            }

            if (!_entityManager.IsComponentEnabled<SpellOnCooldownComponent>(entity))
            {
                entityCommandBuffer.SetComponentEnabled<SpellOnCooldownComponent>(entity, true);
                entityCommandBuffer.AddComponent(entity, new TimeCounterComponent { ElapsedTime = 0, EndTime = cooldown.ValueRO.Cooldown, isInfinite = false });
            }
        }

        entityCommandBuffer.Playback(_entityManager);
        entityCommandBuffer.Dispose();
    }

    [BurstCompile]
    public void OnDestroy(ref SystemState state)
    {

    }
}
