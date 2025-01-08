using Unity.Entities;

public struct OnCompleteRewardComponent : IComponentData
{
    public OnCompleteRewards Type;
    public int Value;
}

public enum OnCompleteRewards
{
    None,
    ExploreChance1,
    ExploreChance2,
    ExploreChance3,
    AnySpellCostReduce,
    MoveSpeed,
    KillAnyDamage,
    ReduceExpPerLevel,
    GoldBonusOnKill
}
