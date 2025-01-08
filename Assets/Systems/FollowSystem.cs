using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Transforms;
using UnityEngine;

partial struct FollowSystem : ISystem
{
    private EntityManager _entityManager;

    public void OnUpdate(ref SystemState state)
    {
        _entityManager = state.EntityManager;

        foreach (var (transform, position, velocity, follow, entity) in SystemAPI.Query<RefRO<LocalTransform>, RefRW<PositionComponent>, RefRO<VelocityComponent>, RefRO<FollowComponent>>().WithEntityAccess())
        {
            if (!_entityManager.HasComponent<PositionComponent>(follow.ValueRO.Target)) continue;

            if (
                _entityManager.HasComponent<DeathComponent>(follow.ValueRO.Target) ||
                _entityManager.HasComponent<DestroyAfterDelayComponent>(follow.ValueRO.Target) ||
                _entityManager.HasComponent<DeathComponent>(entity)
                )
            {
                continue;
            }

            PositionComponent targetPosition = _entityManager.GetComponentData<PositionComponent>(follow.ValueRO.Target);
            PhysicsVelocity physicsVelocity = _entityManager.GetComponentData<PhysicsVelocity>(entity);

            float3 direction = new float3(targetPosition.Position.x, targetPosition.Position.y, 0) - transform.ValueRO.Position;
            direction = math.normalize(direction);

            if (math.distance(transform.ValueRO.Position, new float3(targetPosition.Position.x, targetPosition.Position.y, 0)) <= follow.ValueRO.MinDistance)
            {
                physicsVelocity.Linear = float3.zero;
            }
            else
            {
                physicsVelocity.Linear = direction * velocity.ValueRO.Velocity;
            }

            _entityManager.SetComponentData(entity, physicsVelocity);
            position.ValueRW.Position = new float2(transform.ValueRO.Position.x, transform.ValueRO.Position.y);

            if (_entityManager.HasComponent<VisualsReferenceComponent>(entity))
            {
                VisualsReferenceComponent visualsReferenceComponent = _entityManager.GetComponentData<VisualsReferenceComponent>(entity);

                visualsReferenceComponent.gameObject.transform.position = transform.ValueRO.Position;
                visualsReferenceComponent.gameObject.transform.rotation = transform.ValueRO.Rotation;
            }


            if (_entityManager.HasComponent<DirectionComponent>(entity))
            {
                DirectionComponent directionComponent = _entityManager.GetComponentData<DirectionComponent>(entity);
                if (transform.ValueRO.Position.x < targetPosition.Position.x)
                {
                    directionComponent.Direction = Direction.Right;
                }
                else
                {
                    directionComponent.Direction = Direction.Left;
                }
                _entityManager.SetComponentData(entity, directionComponent);
            }
        }
    }
}
