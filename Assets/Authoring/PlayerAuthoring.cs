using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

class PlayerAuthoring : MonoBehaviour
{
    public float speed;
}

class PlayerAuthoringBaker : Baker<PlayerAuthoring>
{
    public override void Bake(PlayerAuthoring authoring)
    {
        Entity playerEntity = GetEntity(TransformUsageFlags.Dynamic);

        AddComponent(playerEntity, new VelocityComponent { Velocity = authoring.speed });
        AddComponent(playerEntity, new PositionComponent { Position = new float2(0, 0) });
        AddComponent(playerEntity, new MovementTypeComponent { MovementType = MovementType.PlayerInput });
        AddComponent(playerEntity, new PlayerComponent());
    }
}
