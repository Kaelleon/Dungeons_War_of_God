using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 5f;

    private Rigidbody2D rb;
    private InputAction moveAction;
    private Vector2 moveInput;
    public bool CanMove { get; set; } = true;
    public float SpeedMultiplier { get; set; } = 1f;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();

        // Acción creada por código: no necesitas un archivo de Input Actions
        moveAction = new InputAction("Move", InputActionType.Value);
        moveAction.AddCompositeBinding("2DVector")
            .With("Up", "<Keyboard>/w")
            .With("Down", "<Keyboard>/s")
            .With("Left", "<Keyboard>/a")
            .With("Right", "<Keyboard>/d");
        moveAction.AddCompositeBinding("2DVector")
            .With("Up", "<Keyboard>/upArrow")
            .With("Down", "<Keyboard>/downArrow")
            .With("Left", "<Keyboard>/leftArrow")
            .With("Right", "<Keyboard>/rightArrow");
        moveAction.AddBinding("<Gamepad>/leftStick");
    }

    private void OnEnable() => moveAction.Enable();
    private void OnDisable() => moveAction.Disable();

    private void Update()
    {
        Vector2 raw = moveAction.ReadValue<Vector2>();
        moveInput = Vector2.ClampMagnitude(moveAction.ReadValue<Vector2>(), 1f);//evita que la diagonal sea mas rapida
    }

    private void FixedUpdate()
    {
        rb.linearVelocity = moveInput * (moveSpeed * SpeedMultiplier);
    }
}