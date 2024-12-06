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
}