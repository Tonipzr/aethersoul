using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Transforms;
using UnityEngine;

partial struct MovementSystem : ISystem
{
    private EntityManager _entityManager;

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        _entityManager = state.EntityManager;

        foreach (var (position, velocity, movementType, entity) in SystemAPI.Query<RefRW<PositionComponent>, RefRW<VelocityComponent>, RefRO<MovementTypeComponent>>().WithEntityAccess())
        {
            switch (movementType.ValueRO.MovementType)
            {
                case MovementType.PlayerInput:
                    MovePlayer(velocity.ValueRW.Velocity, ref state);
                    break;
            }
        }
    }

    [BurstCompile]
    private void MovePlayer(float velocity, ref SystemState state)
    {
        Entity playerEntity = SystemAPI.GetSingletonEntity<PlayerComponent>();
        Entity inputEntity = SystemAPI.GetSingletonEntity<InputComponent>();
        InputComponent inputComponent = _entityManager.GetComponentData<InputComponent>(inputEntity);

        float3 previousPosition = float3.zero;
        float3 newPosition = float3.zero;
        if (inputComponent.pressingSpace)
        {
            // Update the player's position
            LocalTransform playerTransform = _entityManager.GetComponentData<LocalTransform>(playerEntity);
            previousPosition = playerTransform.Position;
            playerTransform.Position += new float3(inputComponent.movement * velocity * SystemAPI.Time.DeltaTime * 100, 0);

            _entityManager.SetComponentData(playerEntity, playerTransform);

            // Update the player's position component
            PositionComponent positionComponent = _entityManager.GetComponentData<PositionComponent>(playerEntity);
            positionComponent.Position = new float2(playerTransform.Position.x, playerTransform.Position.y);
            newPosition = playerTransform.Position;

            _entityManager.SetComponentData(playerEntity, positionComponent);
        }
        else
        {
            LocalTransform playerTransform = _entityManager.GetComponentData<LocalTransform>(playerEntity);
            previousPosition = playerTransform.Position;

            PhysicsVelocity physicsVelocity = _entityManager.GetComponentData<PhysicsVelocity>(playerEntity);
            float3 movementDirection = new float3(inputComponent.movement.x, inputComponent.movement.y, 0);

            physicsVelocity.Linear = movementDirection * velocity;

            _entityManager.SetComponentData(playerEntity, physicsVelocity);

            PositionComponent positionComponent = _entityManager.GetComponentData<PositionComponent>(playerEntity);
            positionComponent.Position = new float2(playerTransform.Position.x, playerTransform.Position.y);
            newPosition = new float3(playerTransform.Position.x + physicsVelocity.Linear.x * SystemAPI.Time.DeltaTime, playerTransform.Position.y + physicsVelocity.Linear.y * SystemAPI.Time.DeltaTime, playerTransform.Position.z);

            _entityManager.SetComponentData(playerEntity, positionComponent);
        }

        float distanceMoved = math.distance(previousPosition, newPosition);

        var job = new UpdateMapStatsJob
        {
            Type = MapStatsType.CurrentTraveledDistance,
            ValueFloat = distanceMoved,
            Incremental = true
        };
        job.Schedule();
    }
}
