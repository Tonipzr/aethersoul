using Unity.Entities;

public struct ActiveUpgradesComponent : IBufferElementData
{
    public int UpgradeID;
    public UpgradeType Type;
    public float Value;
}

public enum UpgradeType
{
    None,
    GoldBonusOnKill,
    FireDamage,
    WaterDamage,
    EarthDamage,
    AirDamage,
    MaxHealth,
    HealthRegen,
    MaxMana,
    ManaRegen,
    Lifeleech,
    Manaleech,
    ExploreChance1,
    ExploreChance2,
    ExploreChance3,
    AnySpellCostReduce,
    MoveSpeed,
    KillAnyDamage,
    ReduceExpPerLevel,
}
