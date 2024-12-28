using Unity.Entities;
using UnityEngine;

public partial class InputSystem : SystemBase
{
    protected override void OnCreate()
    {
        if (!SystemAPI.TryGetSingleton<InputComponent>(out InputComponent inputComponent))
        {
            EntityManager.CreateEntity(typeof(InputComponent));
        }
    }

    protected override void OnUpdate()
    {
        SystemAPI.SetSingleton(new InputComponent
        {
            movement = UserInputManager.Instance.MovementInput,
            mousePosition = UserInputManager.Instance.MousePosition,
            pressingSpace = UserInputManager.Instance.DashInput,
            pressingInteract = UserInputManager.Instance.InteractInput,
            pressingSpellBookToggle = UserInputManager.Instance.ToggleSpellBookInput,
            pressingSpellSlot1 = UserInputManager.Instance.SpellSlot1Input,
            pressingSpellSlot2 = UserInputManager.Instance.SpellSlot2Input,
            pressingSpellSlot3 = UserInputManager.Instance.SpellSlot3Input,
            pressingSpellSlot4 = UserInputManager.Instance.SpellSlot4Input,
            pressingOpenMenu = UserInputManager.Instance.OpenMenuInput
        });
    }

    protected override void OnDestroy()
    {
    }
}
