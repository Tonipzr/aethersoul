using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
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

        LocalTransform monsterTransform = _entityManager.GetComponentData<LocalTransform>(monsterEntity);
        Vector3 moveVector = Vector3.MoveTowards(monsterTransform.Position, new Vector3(playerPosition.Position.x, playerPosition.Position.y, 0), velocity * Time.deltaTime);
        monsterTransform.Position = new float3(moveVector.x, moveVector.y, 0);
        _entityManager.SetComponentData(monsterEntity, monsterTransform);

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
