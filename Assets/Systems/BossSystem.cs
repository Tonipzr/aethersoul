using Unity.Burst;
using Unity.Collections;
using Unity.Entities;

partial struct BossSystem : ISystem
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

        if (!SystemAPI.TryGetSingletonEntity<BossComponent>(out Entity bossEntity))
        {
            return;
        }

        DynamicBuffer<SelectedSpellsComponent> selectedSpellsBuffer = _entityManager.GetBuffer<SelectedSpellsComponent>(bossEntity);

        // Add default spells if none are selected
        if (selectedSpellsBuffer.Length == 0)
        {
            entityCommandBuffer.AppendToBuffer(bossEntity, new SelectedSpellsComponent { SpellID = 7 });
            entityCommandBuffer.AppendToBuffer(bossEntity, new SelectedSpellsComponent { SpellID = 8 });
            entityCommandBuffer.AppendToBuffer(bossEntity, new SelectedSpellsComponent { SpellID = 10 });
        }

        entityCommandBuffer.Playback(_entityManager);
        entityCommandBuffer.Dispose();
    }

    [BurstCompile]
    public void OnDestroy(ref SystemState state)
    {

    }
}
