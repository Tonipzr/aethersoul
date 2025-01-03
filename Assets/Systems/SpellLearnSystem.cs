using System.Linq;
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

        foreach (var (_, spellLearn, selectedSpellsBuffer, availableSpells, entity) in SystemAPI.Query<RefRO<PlayerComponent>, RefRO<SpellLearnComponent>, DynamicBuffer<SelectedSpellsComponent>, DynamicBuffer<PlayerAvailableSpellsComponent>>().WithEntityAccess())
        {
            entityCommandBuffer.RemoveComponent<SpellLearnComponent>(entity);

            bool alreadyLearned = false;
            for (int i = 0; i < availableSpells.Length; i++)
            {
                if (availableSpells[i].SpellID == spellLearn.ValueRO.SpellID)
                {
                    alreadyLearned = true;
                    break;
                }
            }

            if (alreadyLearned)
            {
                foreach (var spell in SystemAPI.Query<RefRW<SpellComponent>>())
                {
                    if (spell.ValueRO.SpellID == spellLearn.ValueRO.SpellID)
                    {
                        spell.ValueRW.UpgradeLevel++;
                        break;
                    }
                }
            }
            else
            {

                entityCommandBuffer.AppendToBuffer(entity, new PlayerAvailableSpellsComponent { SpellID = spellLearn.ValueRO.SpellID });

                var job = new UpdateMapStatsJob
                {
                    Type = MapStatsType.CurrentSpellsUnlocked,
                    Value = 1,
                    Incremental = true
                };
                job.Schedule();

                if (selectedSpellsBuffer.Length >= 4)
                {
                    continue;
                }

                entityCommandBuffer.AppendToBuffer(entity, new SelectedSpellsComponent { SpellID = spellLearn.ValueRO.SpellID });
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
