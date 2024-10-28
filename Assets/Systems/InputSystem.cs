using Unity.Entities;
using UnityEngine;

public partial class InputSystem : SystemBase
{
    private InputManager controls;

    protected override void OnCreate()
    {
        if (!SystemAPI.TryGetSingleton<InputComponent>(out InputComponent inputComponent))
        {
            EntityManager.CreateEntity(typeof(InputComponent));
        }

        controls = new InputManager();
        controls.Enable();
    }

    protected override void OnUpdate()
    {
        Vector2 movementVector = controls.FindAction("Movement").ReadValue<Vector2>();
        Vector2 mousePosition = controls.FindAction("MousePosition").ReadValue<Vector2>();
        bool pressingSpace = controls.FindAction("Dash").triggered;
        bool pressingInteract = controls.FindAction("Interact").triggered;
        bool pressingSpellBookToggle = controls.FindAction("ToggleSpellBook").triggered;

        bool pressingSpellSlot1 = controls.FindAction("Spell_Slot_1").triggered;
        bool pressingSpellSlot2 = controls.FindAction("Spell_Slot_2").triggered;
        bool pressingSpellSlot3 = controls.FindAction("Spell_Slot_3").triggered;
        bool pressingSpellSlot4 = controls.FindAction("Spell_Slot_4").triggered;

        SystemAPI.SetSingleton(new InputComponent
        {
            movement = movementVector,
            mousePosition = mousePosition,
            pressingSpace = pressingSpace,
            pressingInteract = pressingInteract,
            pressingSpellBookToggle = pressingSpellBookToggle,
            pressingSpellSlot1 = pressingSpellSlot1,
            pressingSpellSlot2 = pressingSpellSlot2,
            pressingSpellSlot3 = pressingSpellSlot3,
            pressingSpellSlot4 = pressingSpellSlot4
        });
    }
}
