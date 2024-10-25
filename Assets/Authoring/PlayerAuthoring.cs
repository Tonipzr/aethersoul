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
        AddComponent(playerEntity, new HealthComponent { MaxHealth = 100, CurrentHealth = 100 });
        AddComponent(playerEntity, new ManaComponent { MaxMana = 100, CurrentMana = 100 });
        AddComponent(playerEntity, new LevelComponent { Level = 1 });
        AddComponent(playerEntity, new ExperienceComponent { Experience = 0, ExperienceToNextLevel = ExperienceToNextLevel.CalculateExperienceToNextLevel(0) });
        AddComponent(playerEntity, new PlayerComponent());
    }
}
