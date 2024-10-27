using Unity.Burst;
using Unity.Entities;

partial struct MousePositionSystem : ISystem
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

        foreach (var mousePosition in SystemAPI.Query<RefRW<MousePositionComponent>>())
        {
            mousePosition.ValueRW.Position = inputComponent.mousePosition;
        }
    }

    [BurstCompile]
    public void OnDestroy(ref SystemState state)
    {

    }
}
