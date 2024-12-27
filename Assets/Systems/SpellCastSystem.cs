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

        if (!SystemAPI.TryGetSingletonEntity<PlayerComponent>(out Entity playerEntity)) return;

        // Entity playerEntity = SystemAPI.GetSingletonEntity<PlayerComponent>();
        DynamicBuffer<PlayerSelectedSpellsComponent> selectedSpellsBuffer = _entityManager.GetBuffer<PlayerSelectedSpellsComponent>(playerEntity);

        if (selectedSpellsBuffer.Length == 0)
        {
            return;
        }

        EntityCommandBuffer entityCommandBuffer = new EntityCommandBuffer(Allocator.Temp);

        if (inputComponent.pressingSpellSlot1 && selectedSpellsBuffer.Length >= 1)
        {
            if (CastSpell(selectedSpellsBuffer[0].SpellID, playerEntity, ref state, entityCommandBuffer))
                entityCommandBuffer.AppendToBuffer(playerEntity, new PlayerCastAttemptComponent { SpellID = selectedSpellsBuffer[0].SpellID });
        }

        if (inputComponent.pressingSpellSlot2 && selectedSpellsBuffer.Length >= 2)
        {
            if (CastSpell(selectedSpellsBuffer[1].SpellID, playerEntity, ref state, entityCommandBuffer))
                entityCommandBuffer.AppendToBuffer(playerEntity, new PlayerCastAttemptComponent { SpellID = selectedSpellsBuffer[1].SpellID });
        }

        if (inputComponent.pressingSpellSlot3 && selectedSpellsBuffer.Length >= 3)
        {
            if (CastSpell(selectedSpellsBuffer[2].SpellID, playerEntity, ref state, entityCommandBuffer))
                entityCommandBuffer.AppendToBuffer(playerEntity, new PlayerCastAttemptComponent { SpellID = selectedSpellsBuffer[2].SpellID });
        }

        if (inputComponent.pressingSpellSlot4 && selectedSpellsBuffer.Length >= 4)
        {
            if (CastSpell(selectedSpellsBuffer[3].SpellID, playerEntity, ref state, entityCommandBuffer))
                entityCommandBuffer.AppendToBuffer(playerEntity, new PlayerCastAttemptComponent { SpellID = selectedSpellsBuffer[3].SpellID });
        }

        entityCommandBuffer.Playback(_entityManager);
        entityCommandBuffer.Dispose();
    }

    [BurstCompile]
    private bool CastSpell(int spellID, Entity playerEntity, ref SystemState state, EntityCommandBuffer entityCommandBuffer)
    {
        bool castedSpell = false;

        foreach (var (spell, cooldown, _, entity) in SystemAPI.Query<RefRO<SpellComponent>, RefRO<SpellCooldownComponent>, RefRO<SpellOnCooldownComponent>>().WithEntityAccess().WithOptions(EntityQueryOptions.IgnoreComponentEnabledState))
        {
            if (spell.ValueRO.SpellID != spellID) continue;

            if (_entityManager.IsComponentEnabled<SpellOnCooldownComponent>(entity))
            {
                continue;
            }

            if (!_entityManager.IsComponentEnabled<SpellOnCooldownComponent>(entity) && HasEnoughMana(entity, playerEntity, ref state))
            {
                entityCommandBuffer.SetComponentEnabled<SpellOnCooldownComponent>(entity, true);
                entityCommandBuffer.AddComponent(entity, new TimeCounterComponent { ElapsedTime = 0, EndTime = cooldown.ValueRO.Cooldown, isInfinite = false });
                castedSpell = true;
            }
        }

        return castedSpell;
    }

    [BurstCompile]
    private bool HasEnoughMana(Entity spellEntity, Entity casterEntity, ref SystemState state)
    {
        if (!state.EntityManager.HasComponent<ManaComponent>(casterEntity) || !state.EntityManager.HasComponent<SpellCostComponent>(spellEntity))
        {
            return false;
        }

        ManaComponent mana = state.EntityManager.GetComponentData<ManaComponent>(casterEntity);
        SpellCostComponent spellCost = state.EntityManager.GetComponentData<SpellCostComponent>(spellEntity);

        return mana.CurrentMana >= spellCost.Cost;
    }

    [BurstCompile]
    public void OnDestroy(ref SystemState state)
    {

    }
}
