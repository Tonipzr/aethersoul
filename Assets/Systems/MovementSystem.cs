using System.Diagnostics;
using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

partial struct MovementSystem : ISystem
{
    private EntityManager _entityManager;

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        _entityManager = state.EntityManager;

        foreach (var (position, velocity, movementType) in SystemAPI.Query<RefRW<PositionComponent>, RefRW<VelocityComponent>, RefRO<MovementTypeComponent>>())
        {
            switch (movementType.ValueRO.MovementType)
            {
                case MovementType.PlayerInput:
                    MovePlayer(velocity.ValueRW.Velocity, ref state);
                    break;
                case MovementType.AIControlled:
                    UnityEngine.Debug.Log("AI controlled movement not implemented");
                    break;
            }
        }
    }

    private void MovePlayer(float velocity, ref SystemState state)
    {
        Entity playerEntity = SystemAPI.GetSingletonEntity<PlayerComponent>();
        Entity inputEntity = SystemAPI.GetSingletonEntity<InputComponent>();
        InputComponent inputComponent = _entityManager.GetComponentData<InputComponent>(inputEntity);

        if (inputComponent.pressingSpace)
        {
            // Update the player's position
            LocalTransform playerTransform = _entityManager.GetComponentData<LocalTransform>(playerEntity);
            playerTransform.Position += new float3(inputComponent.movement * velocity * SystemAPI.Time.DeltaTime * 100, 0);

            _entityManager.SetComponentData(playerEntity, playerTransform);

            // Update the player's position component
            PositionComponent positionComponent = _entityManager.GetComponentData<PositionComponent>(playerEntity);
            positionComponent.Position = new float2(playerTransform.Position.x, playerTransform.Position.y);

            _entityManager.SetComponentData(playerEntity, positionComponent);
        }
        else
        {
            // Update the player's position
            LocalTransform playerTransform = _entityManager.GetComponentData<LocalTransform>(playerEntity);
            playerTransform.Position += new float3(inputComponent.movement * velocity * SystemAPI.Time.DeltaTime, 0);

            _entityManager.SetComponentData(playerEntity, playerTransform);

            // Update the player's position component
            PositionComponent positionComponent = _entityManager.GetComponentData<PositionComponent>(playerEntity);
            positionComponent.Position = new float2(playerTransform.Position.x, playerTransform.Position.y);

            _entityManager.SetComponentData(playerEntity, positionComponent);
        }
    }
}
