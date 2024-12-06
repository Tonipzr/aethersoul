using UnityEngine;

public class DreamCityCharacterController : MonoBehaviour
{
    private InputManager controls;
    private Animator animator;
    private SpriteRenderer spriteRenderer;

    void Awake()
    {
        controls = new InputManager();
        controls.Enable();

        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        Vector2 movementVector = controls.FindAction("Movement").ReadValue<Vector2>();
        float movementValue = Mathf.Abs(movementVector.x) > 0 || Mathf.Abs(movementVector.y) > 0 ?
            Mathf.Abs(movementVector.x) + Mathf.Abs(movementVector.y) : -1;
        bool pressingInteract = controls.FindAction("Interact").triggered;

        transform.position += new Vector3(movementVector.x, movementVector.y, 0) * Time.deltaTime * 5;

        if (movementVector.x < 0)
        {
            spriteRenderer.flipX = true;
        }
        else if (movementVector.x > 0)
        {
            spriteRenderer.flipX = false;
        }

        animator.SetFloat("Movement", movementValue);
        animator.SetBool("Action", pressingInteract);

        RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.right, 1.0f);
        if (hit.collider != null)
        {
            DreamCityUIHandler.Instance.ToggleInteractImage(true);

            if (pressingInteract)
            {
                if (hit.collider.name == "Checkpoint") DreamCityUIHandler.Instance.CheckPointInteract();

                if (
                    hit.collider.name == "FireStatue" ||
                    hit.collider.name == "WaterStatue" ||
                    hit.collider.name == "EarthStatue" ||
                    hit.collider.name == "WindStatue"
                ) DreamCityUIHandler.Instance.ToggleBuffUI(true, hit.collider.name);
            }
        }
        else
        {
            DreamCityUIHandler.Instance.ToggleInteractImage(false);
            DreamCityUIHandler.Instance.ToggleBuffUI(false, "");

            if (DreamCityUIHandler.Instance.BoughtBuff)
            {
                DreamCityUIHandler.Instance.BoughtBuff = false;
                DreamCityUIHandler.Instance.PlayAudioUpgradeEffect();
            }
        }
    }

    void OnDestroy()
    {
        controls.Disable();
    }
}
