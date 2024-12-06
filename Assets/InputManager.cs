//------------------------------------------------------------------------------
// <auto-generated>
//     This code was auto-generated by com.unity.inputsystem:InputActionCodeGenerator
//     version 1.11.2
//     from Assets/InputManager.inputactions
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Utilities;

public partial class @InputManager: IInputActionCollection2, IDisposable
{
    public InputActionAsset asset { get; }
    public @InputManager()
    {
        asset = InputActionAsset.FromJson(@"{
    ""name"": ""InputManager"",
    ""maps"": [
        {
            ""name"": ""PlayInputMap"",
            ""id"": ""a60b076e-f8cb-4084-9652-553d84dc722c"",
            ""actions"": [
                {
                    ""name"": ""Movement"",
                    ""type"": ""Value"",
                    ""id"": ""d46360ec-bacd-45e9-8602-411bdc9db125"",
                    ""expectedControlType"": ""Vector2"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": true
                },
                {
                    ""name"": ""Dash"",
                    ""type"": ""Button"",
                    ""id"": ""37cc8e17-52dc-4029-9867-d4a1df17a27e"",
                    ""expectedControlType"": """",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": true
                },
                {
                    ""name"": ""Interact"",
                    ""type"": ""Button"",
                    ""id"": ""2c3bd690-fb2d-4357-a2cc-610f2c968f15"",
                    ""expectedControlType"": """",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": true
                },
                {
                    ""name"": ""Spell_Slot_1"",
                    ""type"": ""Button"",
                    ""id"": ""d9af80a9-1a4d-4232-b0bd-2891b11c49a1"",
                    ""expectedControlType"": """",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                },
                {
                    ""name"": ""Spell_Slot_2"",
                    ""type"": ""Button"",
                    ""id"": ""84d48669-7554-4be4-83cc-09e1ee93fff1"",
                    ""expectedControlType"": """",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                },
                {
                    ""name"": ""Spell_Slot_3"",
                    ""type"": ""Button"",
                    ""id"": ""4be861f1-402e-4bc1-8843-0aac6dbf7912"",
                    ""expectedControlType"": """",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                },
                {
                    ""name"": ""Spell_Slot_4"",
                    ""type"": ""Button"",
                    ""id"": ""aa6fe8a4-3910-4003-b8c4-92a44991f2dc"",
                    ""expectedControlType"": """",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                },
                {
                    ""name"": ""ToggleSpellBook"",
                    ""type"": ""Button"",
                    ""id"": ""c0f3a02e-3c73-40a4-87e4-598f21e537a2"",
                    ""expectedControlType"": """",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                },
                {
                    ""name"": ""MousePosition"",
                    ""type"": ""Value"",
                    ""id"": ""0e755665-1bbc-4157-a426-d00fdb1b805b"",
                    ""expectedControlType"": ""Vector2"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": true
                },
                {
                    ""name"": ""OpenMenu"",
                    ""type"": ""Button"",
                    ""id"": ""3030100e-cd7e-4377-9d12-13c2bf8c8202"",
                    ""expectedControlType"": """",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                }
            ],
            ""bindings"": [
                {
                    ""name"": ""2D Vector"",
                    ""id"": ""cf19c65c-664c-4cee-a4fd-6eb6656156b3"",
                    ""path"": ""2DVector"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Movement"",
                    ""isComposite"": true,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": ""up"",
                    ""id"": ""6fdf492c-68fc-48b1-916d-776b717e0615"",
                    ""path"": ""<Keyboard>/w"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Movement"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""down"",
                    ""id"": ""0a375a76-d59b-420f-943a-b33eefe00639"",
                    ""path"": ""<Keyboard>/s"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Movement"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""left"",
                    ""id"": ""98b02948-c685-46f6-8f92-6c495d9922bf"",
                    ""path"": ""<Keyboard>/a"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Movement"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""right"",
                    ""id"": ""4ae5c5ac-1778-47a6-b1a4-d8ae8411d500"",
                    ""path"": ""<Keyboard>/d"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Movement"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""2D Vector"",
                    ""id"": ""5b4249ee-3799-4fcd-8688-97c0c670507f"",
                    ""path"": ""2DVector"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Movement"",
                    ""isComposite"": true,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": ""up"",
                    ""id"": ""64641f7c-a0f4-428b-949b-15a246622992"",
                    ""path"": ""<Keyboard>/upArrow"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Movement"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""down"",
                    ""id"": ""46f50841-7ddb-4625-aad5-e6d0dfc3994c"",
                    ""path"": ""<Keyboard>/downArrow"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Movement"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""left"",
                    ""id"": ""e794613c-c124-429a-aeac-6b9a46a74764"",
                    ""path"": ""<Keyboard>/leftArrow"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Movement"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""right"",
                    ""id"": ""eea8c2d0-bfa8-44a9-b11c-cb5ddd8de79b"",
                    ""path"": ""<Keyboard>/rightArrow"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Movement"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": """",
                    ""id"": ""42da7a52-2f61-49d3-86cf-d25373e56545"",
                    ""path"": ""<Keyboard>/space"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Dash"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""af7b202c-a920-49ad-96b5-5fc1e5a0aa92"",
                    ""path"": ""<Keyboard>/e"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Interact"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""4dac1199-86fd-43f9-92bf-274e73bca45c"",
                    ""path"": ""<Keyboard>/1"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Spell_Slot_1"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""219c52b5-5e70-46d4-8249-fd7934772c44"",
                    ""path"": ""<Keyboard>/2"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Spell_Slot_2"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""6cc97ae6-6f66-4db0-b81f-286ab92e3878"",
                    ""path"": ""<Keyboard>/3"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Spell_Slot_3"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""b2d03b9b-8f99-45fe-b55d-216912ed0bd8"",
                    ""path"": ""<Keyboard>/4"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Spell_Slot_4"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""66e81bdf-060f-4de3-8814-0a8fab43aa10"",
                    ""path"": ""<Keyboard>/p"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""ToggleSpellBook"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""e4bedde4-9df9-430b-93ad-641e91cd215a"",
                    ""path"": ""<Mouse>/position"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""MousePosition"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""ef479e57-6806-42aa-a055-ccec2e93330d"",
                    ""path"": ""<Keyboard>/escape"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""OpenMenu"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                }
            ]
        }
    ],
    ""controlSchemes"": []
}");
        // PlayInputMap
        m_PlayInputMap = asset.FindActionMap("PlayInputMap", throwIfNotFound: true);
        m_PlayInputMap_Movement = m_PlayInputMap.FindAction("Movement", throwIfNotFound: true);
        m_PlayInputMap_Dash = m_PlayInputMap.FindAction("Dash", throwIfNotFound: true);
        m_PlayInputMap_Interact = m_PlayInputMap.FindAction("Interact", throwIfNotFound: true);
        m_PlayInputMap_Spell_Slot_1 = m_PlayInputMap.FindAction("Spell_Slot_1", throwIfNotFound: true);
        m_PlayInputMap_Spell_Slot_2 = m_PlayInputMap.FindAction("Spell_Slot_2", throwIfNotFound: true);
        m_PlayInputMap_Spell_Slot_3 = m_PlayInputMap.FindAction("Spell_Slot_3", throwIfNotFound: true);
        m_PlayInputMap_Spell_Slot_4 = m_PlayInputMap.FindAction("Spell_Slot_4", throwIfNotFound: true);
        m_PlayInputMap_ToggleSpellBook = m_PlayInputMap.FindAction("ToggleSpellBook", throwIfNotFound: true);
        m_PlayInputMap_MousePosition = m_PlayInputMap.FindAction("MousePosition", throwIfNotFound: true);
        m_PlayInputMap_OpenMenu = m_PlayInputMap.FindAction("OpenMenu", throwIfNotFound: true);
    }

    ~@InputManager()
    {
        UnityEngine.Debug.Assert(!m_PlayInputMap.enabled, "This will cause a leak and performance issues, InputManager.PlayInputMap.Disable() has not been called.");
    }

    public void Dispose()
    {
        UnityEngine.Object.Destroy(asset);
    }

    public InputBinding? bindingMask
    {
        get => asset.bindingMask;
        set => asset.bindingMask = value;
    }

    public ReadOnlyArray<InputDevice>? devices
    {
        get => asset.devices;
        set => asset.devices = value;
    }

    public ReadOnlyArray<InputControlScheme> controlSchemes => asset.controlSchemes;

    public bool Contains(InputAction action)
    {
        return asset.Contains(action);
    }

    public IEnumerator<InputAction> GetEnumerator()
    {
        return asset.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    public void Enable()
    {
        asset.Enable();
    }

    public void Disable()
    {
        asset.Disable();
    }

    public IEnumerable<InputBinding> bindings => asset.bindings;

    public InputAction FindAction(string actionNameOrId, bool throwIfNotFound = false)
    {
        return asset.FindAction(actionNameOrId, throwIfNotFound);
    }

    public int FindBinding(InputBinding bindingMask, out InputAction action)
    {
        return asset.FindBinding(bindingMask, out action);
    }

    // PlayInputMap
    private readonly InputActionMap m_PlayInputMap;
    private List<IPlayInputMapActions> m_PlayInputMapActionsCallbackInterfaces = new List<IPlayInputMapActions>();
    private readonly InputAction m_PlayInputMap_Movement;
    private readonly InputAction m_PlayInputMap_Dash;
    private readonly InputAction m_PlayInputMap_Interact;
    private readonly InputAction m_PlayInputMap_Spell_Slot_1;
    private readonly InputAction m_PlayInputMap_Spell_Slot_2;
    private readonly InputAction m_PlayInputMap_Spell_Slot_3;
    private readonly InputAction m_PlayInputMap_Spell_Slot_4;
    private readonly InputAction m_PlayInputMap_ToggleSpellBook;
    private readonly InputAction m_PlayInputMap_MousePosition;
    private readonly InputAction m_PlayInputMap_OpenMenu;
    public struct PlayInputMapActions
    {
        private @InputManager m_Wrapper;
        public PlayInputMapActions(@InputManager wrapper) { m_Wrapper = wrapper; }
        public InputAction @Movement => m_Wrapper.m_PlayInputMap_Movement;
        public InputAction @Dash => m_Wrapper.m_PlayInputMap_Dash;
        public InputAction @Interact => m_Wrapper.m_PlayInputMap_Interact;
        public InputAction @Spell_Slot_1 => m_Wrapper.m_PlayInputMap_Spell_Slot_1;
        public InputAction @Spell_Slot_2 => m_Wrapper.m_PlayInputMap_Spell_Slot_2;
        public InputAction @Spell_Slot_3 => m_Wrapper.m_PlayInputMap_Spell_Slot_3;
        public InputAction @Spell_Slot_4 => m_Wrapper.m_PlayInputMap_Spell_Slot_4;
        public InputAction @ToggleSpellBook => m_Wrapper.m_PlayInputMap_ToggleSpellBook;
        public InputAction @MousePosition => m_Wrapper.m_PlayInputMap_MousePosition;
        public InputAction @OpenMenu => m_Wrapper.m_PlayInputMap_OpenMenu;
        public InputActionMap Get() { return m_Wrapper.m_PlayInputMap; }
        public void Enable() { Get().Enable(); }
        public void Disable() { Get().Disable(); }
        public bool enabled => Get().enabled;
        public static implicit operator InputActionMap(PlayInputMapActions set) { return set.Get(); }
        public void AddCallbacks(IPlayInputMapActions instance)
        {
            if (instance == null || m_Wrapper.m_PlayInputMapActionsCallbackInterfaces.Contains(instance)) return;
            m_Wrapper.m_PlayInputMapActionsCallbackInterfaces.Add(instance);
            @Movement.started += instance.OnMovement;
            @Movement.performed += instance.OnMovement;
            @Movement.canceled += instance.OnMovement;
            @Dash.started += instance.OnDash;
            @Dash.performed += instance.OnDash;
            @Dash.canceled += instance.OnDash;
            @Interact.started += instance.OnInteract;
            @Interact.performed += instance.OnInteract;
            @Interact.canceled += instance.OnInteract;
            @Spell_Slot_1.started += instance.OnSpell_Slot_1;
            @Spell_Slot_1.performed += instance.OnSpell_Slot_1;
            @Spell_Slot_1.canceled += instance.OnSpell_Slot_1;
            @Spell_Slot_2.started += instance.OnSpell_Slot_2;
            @Spell_Slot_2.performed += instance.OnSpell_Slot_2;
            @Spell_Slot_2.canceled += instance.OnSpell_Slot_2;
            @Spell_Slot_3.started += instance.OnSpell_Slot_3;
            @Spell_Slot_3.performed += instance.OnSpell_Slot_3;
            @Spell_Slot_3.canceled += instance.OnSpell_Slot_3;
            @Spell_Slot_4.started += instance.OnSpell_Slot_4;
            @Spell_Slot_4.performed += instance.OnSpell_Slot_4;
            @Spell_Slot_4.canceled += instance.OnSpell_Slot_4;
            @ToggleSpellBook.started += instance.OnToggleSpellBook;
            @ToggleSpellBook.performed += instance.OnToggleSpellBook;
            @ToggleSpellBook.canceled += instance.OnToggleSpellBook;
            @MousePosition.started += instance.OnMousePosition;
            @MousePosition.performed += instance.OnMousePosition;
            @MousePosition.canceled += instance.OnMousePosition;
            @OpenMenu.started += instance.OnOpenMenu;
            @OpenMenu.performed += instance.OnOpenMenu;
            @OpenMenu.canceled += instance.OnOpenMenu;
        }

        private void UnregisterCallbacks(IPlayInputMapActions instance)
        {
            @Movement.started -= instance.OnMovement;
            @Movement.performed -= instance.OnMovement;
            @Movement.canceled -= instance.OnMovement;
            @Dash.started -= instance.OnDash;
            @Dash.performed -= instance.OnDash;
            @Dash.canceled -= instance.OnDash;
            @Interact.started -= instance.OnInteract;
            @Interact.performed -= instance.OnInteract;
            @Interact.canceled -= instance.OnInteract;
            @Spell_Slot_1.started -= instance.OnSpell_Slot_1;
            @Spell_Slot_1.performed -= instance.OnSpell_Slot_1;
            @Spell_Slot_1.canceled -= instance.OnSpell_Slot_1;
            @Spell_Slot_2.started -= instance.OnSpell_Slot_2;
            @Spell_Slot_2.performed -= instance.OnSpell_Slot_2;
            @Spell_Slot_2.canceled -= instance.OnSpell_Slot_2;
            @Spell_Slot_3.started -= instance.OnSpell_Slot_3;
            @Spell_Slot_3.performed -= instance.OnSpell_Slot_3;
            @Spell_Slot_3.canceled -= instance.OnSpell_Slot_3;
            @Spell_Slot_4.started -= instance.OnSpell_Slot_4;
            @Spell_Slot_4.performed -= instance.OnSpell_Slot_4;
            @Spell_Slot_4.canceled -= instance.OnSpell_Slot_4;
            @ToggleSpellBook.started -= instance.OnToggleSpellBook;
            @ToggleSpellBook.performed -= instance.OnToggleSpellBook;
            @ToggleSpellBook.canceled -= instance.OnToggleSpellBook;
            @MousePosition.started -= instance.OnMousePosition;
            @MousePosition.performed -= instance.OnMousePosition;
            @MousePosition.canceled -= instance.OnMousePosition;
            @OpenMenu.started -= instance.OnOpenMenu;
            @OpenMenu.performed -= instance.OnOpenMenu;
            @OpenMenu.canceled -= instance.OnOpenMenu;
        }

        public void RemoveCallbacks(IPlayInputMapActions instance)
        {
            if (m_Wrapper.m_PlayInputMapActionsCallbackInterfaces.Remove(instance))
                UnregisterCallbacks(instance);
        }

        public void SetCallbacks(IPlayInputMapActions instance)
        {
            foreach (var item in m_Wrapper.m_PlayInputMapActionsCallbackInterfaces)
                UnregisterCallbacks(item);
            m_Wrapper.m_PlayInputMapActionsCallbackInterfaces.Clear();
            AddCallbacks(instance);
        }
    }
    public PlayInputMapActions @PlayInputMap => new PlayInputMapActions(this);
    public interface IPlayInputMapActions
    {
        void OnMovement(InputAction.CallbackContext context);
        void OnDash(InputAction.CallbackContext context);
        void OnInteract(InputAction.CallbackContext context);
        void OnSpell_Slot_1(InputAction.CallbackContext context);
        void OnSpell_Slot_2(InputAction.CallbackContext context);
        void OnSpell_Slot_3(InputAction.CallbackContext context);
        void OnSpell_Slot_4(InputAction.CallbackContext context);
        void OnToggleSpellBook(InputAction.CallbackContext context);
        void OnMousePosition(InputAction.CallbackContext context);
        void OnOpenMenu(InputAction.CallbackContext context);
    }
}
