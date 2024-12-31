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
                case MovementType.AIControlled:
                    if (!_entityManager.HasComponent<DeathComponent>(entity) && !_entityManager.HasComponent<DestroyAfterDelayComponent>(entity))
                    {
                        MoveAI(velocity.ValueRW.Velocity, entity, ref state);
                    }
                    break;
            }
        }
    }

    [BurstCompile]
    private void MoveAI(float velocity, Entity monsterEntity, ref SystemState state)
    {
        Entity playerEntity = SystemAPI.GetSingletonEntity<PlayerComponent>();
        PositionComponent playerPosition = _entityManager.GetComponentData<PositionComponent>(playerEntity);

        PhysicsVelocity monsterVelocity = _entityManager.GetComponentData<PhysicsVelocity>(monsterEntity);
        LocalTransform monsterTransform = _entityManager.GetComponentData<LocalTransform>(monsterEntity);

        float3 direction = new float3(playerPosition.Position.x, playerPosition.Position.y, 0) - monsterTransform.Position;
        direction = math.normalize(direction);

        float3 velocityCalc = direction * velocity;
        monsterVelocity.Linear = velocityCalc;

        _entityManager.SetComponentData(monsterEntity, monsterVelocity);

        PositionComponent positionComponent = _entityManager.GetComponentData<PositionComponent>(monsterEntity);
        positionComponent.Position = new float2(monsterTransform.Position.x, monsterTransform.Position.y);
        _entityManager.SetComponentData(monsterEntity, positionComponent);

        DirectionComponent directionComponent = _entityManager.GetComponentData<DirectionComponent>(monsterEntity);
        if (monsterTransform.Position.x < playerPosition.Position.x)
        {
            directionComponent.Direction = Direction.Right;
        }
        else
        {
            directionComponent.Direction = Direction.Left;
        }
        _entityManager.SetComponentData(monsterEntity, directionComponent);
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
