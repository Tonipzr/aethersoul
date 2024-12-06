using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics.Authoring;
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
        AddComponent(playerEntity, new MousePositionComponent { Position = new float2(0, 0) });
        AddComponent(playerEntity, new MovementTypeComponent { MovementType = MovementType.PlayerInput });
        AddComponent(playerEntity, new HealthComponent { MaxHealth = 100, CurrentHealth = 100, BaseMaxHealth = 100 });
        AddComponent(playerEntity, new ManaComponent { MaxMana = 100, CurrentMana = 100, BaseMaxMana = 100 });
        AddComponent(playerEntity, new LevelComponent { Level = 1 });
        AddComponent(playerEntity, new ExperienceComponent { Experience = 0, ExperienceToNextLevel = ExperienceToNextLevel.CalculateExperienceToNextLevel(0) });
        AddBuffer<PlayerSelectedSpellsComponent>(playerEntity);
        AddBuffer<PlayerAvailableSpellsComponent>(playerEntity);
        AddComponent(playerEntity, new SpellLearnComponent { SpellID = 5 });
        AddBuffer<ActiveUpgradesComponent>(playerEntity);
        AddBuffer<PlayerCastAttemptComponent>(playerEntity);
        AddComponent(playerEntity, new WeatherComponent { Weather = WeatherType.Clear });
        AddComponent(playerEntity, new IsInSafeZoneComponent());
        SetComponentEnabled<IsInSafeZoneComponent>(playerEntity, false);
        AddComponent(playerEntity, new InvulnerableStateComponent { Duration = 1, ElapsedTime = 0 });
        SetComponentEnabled<InvulnerableStateComponent>(playerEntity, false);
        AddComponent(playerEntity, new PlayerComponent());
    }
}
