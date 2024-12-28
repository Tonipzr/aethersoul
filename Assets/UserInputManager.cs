using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;
using UnityEngine.InputSystem;

public class UserInputManager : MonoBehaviour
{
    public static UserInputManager Instance { get; private set; }

    public Vector2 MovementInput { get; private set; }
    public Vector2 MousePosition { get; private set; }
    public bool DashInput { get; private set; }
    public bool InteractInput { get; private set; }
    public bool ToggleSpellBookInput { get; private set; }
    public bool OpenMenuInput { get; private set; }
    public bool SpellSlot1Input { get; private set; }
    public bool SpellSlot2Input { get; private set; }
    public bool SpellSlot3Input { get; private set; }
    public bool SpellSlot4Input { get; private set; }

    private PlayerInput _playerInput;

    private InputAction _movement;
    private InputAction _mousePosition;
    private InputAction _dash;
    private InputAction _interact;
    private InputAction _toggleSpellBook;
    private InputAction _openMenu;

    private InputAction _spellSlot1;
    private InputAction _spellSlot2;
    private InputAction _spellSlot3;
    private InputAction _spellSlot4;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        _playerInput = GetComponent<PlayerInput>();

        SetupInputActions();

        DontDestroyOnLoad(gameObject);
    }

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        UpdateInput();
    }

    private void SetupInputActions()
    {
        _movement = _playerInput.actions.FindAction("Movement");
        _mousePosition = _playerInput.actions.FindAction("MousePosition");
        _dash = _playerInput.actions.FindAction("Dash");
        _interact = _playerInput.actions.FindAction("Interact");
        _toggleSpellBook = _playerInput.actions.FindAction("ToggleSpellBook");
        _openMenu = _playerInput.actions.FindAction("OpenMenu");

        _spellSlot1 = _playerInput.actions.FindAction("Spell_Slot_1");
        _spellSlot2 = _playerInput.actions.FindAction("Spell_Slot_2");
        _spellSlot3 = _playerInput.actions.FindAction("Spell_Slot_3");
        _spellSlot4 = _playerInput.actions.FindAction("Spell_Slot_4");
    }

    private void UpdateInput()
    {
        MovementInput = _movement.ReadValue<Vector2>();
        MousePosition = _mousePosition.ReadValue<Vector2>();
        DashInput = _dash.triggered;
        InteractInput = _interact.triggered;
        ToggleSpellBookInput = _toggleSpellBook.triggered;
        OpenMenuInput = _openMenu.triggered;

        SpellSlot1Input = _spellSlot1.triggered;
        SpellSlot2Input = _spellSlot2.triggered;
        SpellSlot3Input = _spellSlot3.triggered;
        SpellSlot4Input = _spellSlot4.triggered;
    }

    public string GetKeyMap(string actionName)
    {
        return _playerInput.actions.FindAction(actionName).bindings[0].effectivePath.Replace("<", "").Replace(">", "").Replace("Keyboard", "").Replace("/", "").ToUpper();
    }
}
