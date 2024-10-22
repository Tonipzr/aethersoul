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
        bool pressingSpace = controls.FindAction("Dash").ReadValue<float>() > 0;
        bool pressingInteract = controls.FindAction("Interact").triggered;

        SystemAPI.SetSingleton(new InputComponent
        {
            movement = movementVector,
            pressingSpace = pressingSpace,
            pressingInteract = pressingInteract
        });
    }
}
