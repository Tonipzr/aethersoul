using Unity.Entities;
using Unity.Mathematics;

public struct InputComponent : IComponentData
{
    public float2 movement;
    public bool pressingSpace;

    public bool pressingInteract;

    public bool pressingSpellBookToggle;

    public bool pressingSpellSlot1;
    public bool pressingSpellSlot2;
    public bool pressingSpellSlot3;
    public bool pressingSpellSlot4;
}
