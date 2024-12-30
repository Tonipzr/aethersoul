using Unity.Entities;

public struct TriggerComponent : IBufferElementData
{
    public TriggerType TriggerType;
    public int Value;
    public int Value2;
    public bool Satisfied;
}

public enum TriggerType
{
    None,
    KillAny,
    Gold,
    Clear,
    AnySkillUsed,
    GoldUsed,
    Explore,
    Travel,
    KillAnyNoDamage,
    UnlockAllSpells,
    Level
}
