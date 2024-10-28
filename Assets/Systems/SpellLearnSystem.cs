using System.Diagnostics;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;

partial struct SpellLearnSystem : ISystem
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
        EntityCommandBuffer entityCommandBuffer = new EntityCommandBuffer(Allocator.Temp);

        foreach (var (_, spellLearn, selectedSpellsBuffer, _, entity) in SystemAPI.Query<RefRO<PlayerComponent>, RefRO<SpellLearnComponent>, DynamicBuffer<PlayerSelectedSpellsComponent>, DynamicBuffer<PlayerAvailableSpellsComponent>>().WithEntityAccess())
        {
            entityCommandBuffer.RemoveComponent<SpellLearnComponent>(entity);
            entityCommandBuffer.AppendToBuffer(entity, new PlayerAvailableSpellsComponent { SpellID = spellLearn.ValueRO.SpellID });

            if (selectedSpellsBuffer.Length >= 4)
            {
                continue;
            }

            entityCommandBuffer.AppendToBuffer(entity, new PlayerSelectedSpellsComponent { SpellID = spellLearn.ValueRO.SpellID });
        }

        entityCommandBuffer.Playback(_entityManager);
        entityCommandBuffer.Dispose();
    }

    [BurstCompile]
    public void OnDestroy(ref SystemState state)
    {

    }
}
