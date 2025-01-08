public enum SpellType
{
    SkillShot,
    AreaOfEffect,
    Buff,
    Healing,
    Passive,
    ScreenSweep
}

public enum SpellRange
{
    Melee,
    Three,
    Five,
    Ten,
    Infinite
}

public enum SpellTarget
{
    Self,
    Targeted,
    MousePosition,
    SelfBoss,
    RandomAroundTarget,
    Player
}
