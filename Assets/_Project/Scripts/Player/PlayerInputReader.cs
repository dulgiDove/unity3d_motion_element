using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInputReader : MonoBehaviour
{
    [SerializeField] private PlayerInput playerInput;

    [Header("Double Tap Dodge")]
    [SerializeField] private float doubleTapThreshold = 0.25f;
    [SerializeField] private float inputPressThreshold = 0.5f;

    public Vector2 MoveInput { get; private set; }
    public bool IsSprintPressed { get; private set; }

    public bool JumpPressedThisFrame { get; private set; }
    public bool JumpHeld { get; private set; }

    public bool DodgePressedThisFrame { get; private set; }
    public Vector2 DodgeInput { get; private set; }

    public bool AttackPressedThisFrame { get; private set; }

    public bool LockOnPressedThisFrame { get; private set; }

    public bool InteractPressedThisFrame { get; private set; }

    private InputAction moveAction;
    private InputAction sprintAction;
    private InputAction jumpAction;
    private InputAction attackAction;
    private InputAction lockOnAction;
    private InputAction interactAction;

    private Vector2 previousMoveInput;

    private float lastForwardTapTime = -999f;
    private float lastBackwardTapTime = -999f;
    private float lastLeftTapTime = -999f;
    private float lastRightTapTime = -999f;

    private void Awake()
    {
        if (playerInput == null)
            playerInput = GetComponent<PlayerInput>();

        moveAction = playerInput.actions["Move"];
        sprintAction = playerInput.actions["Sprint"];
        jumpAction = playerInput.actions["Jump"];
        attackAction = playerInput.actions["Attack"];
        lockOnAction = playerInput.actions["LockOn"];
        interactAction = playerInput.actions["Interact"];
    }

    private void OnEnable()
    {
        moveAction.performed += OnMove;
        moveAction.canceled += OnMove;

        sprintAction.performed += OnSprint;
        sprintAction.canceled += OnSprint;

        jumpAction.started += OnJumpStarted;
        jumpAction.canceled += OnJumpCanceled;

        attackAction.started += OnAttackStarted;

        lockOnAction.started += OnLockOnStarted;

        interactAction.started += OnInteractStarted;
    }

    private void OnDisable()
    {
        moveAction.performed -= OnMove;
        moveAction.canceled -= OnMove;

        sprintAction.performed -= OnSprint;
        sprintAction.canceled -= OnSprint;

        jumpAction.started -= OnJumpStarted;
        jumpAction.canceled -= OnJumpCanceled;

        attackAction.started -= OnAttackStarted;

        lockOnAction.started -= OnLockOnStarted;

        interactAction.started -= OnInteractStarted;
    }

    private void OnMove(InputAction.CallbackContext context)
    {
        Vector2 newMoveInput = context.ReadValue<Vector2>();

        DetectDoubleTap(newMoveInput);

        previousMoveInput = newMoveInput;
        MoveInput = newMoveInput;
    }

    private void DetectDoubleTap(Vector2 newInput)
    {
        if (WasPressedThisFrame(previousMoveInput.y, newInput.y))
            CheckDoubleTap(ref lastForwardTapTime, Vector2.up);

        if (WasPressedThisFrame(-previousMoveInput.y, -newInput.y))
            CheckDoubleTap(ref lastBackwardTapTime, Vector2.down);

        if (WasPressedThisFrame(-previousMoveInput.x, -newInput.x))
            CheckDoubleTap(ref lastLeftTapTime, Vector2.left);

        if (WasPressedThisFrame(previousMoveInput.x, newInput.x))
            CheckDoubleTap(ref lastRightTapTime, Vector2.right);
    }

    private bool WasPressedThisFrame(float previousValue, float currentValue)
    {
        return previousValue < inputPressThreshold &&
               currentValue >= inputPressThreshold;
    }

    private void CheckDoubleTap(ref float lastTapTime, Vector2 dodgeInput)
    {
        if (Time.time - lastTapTime <= doubleTapThreshold)
        {
            DodgePressedThisFrame = true;
            DodgeInput = dodgeInput;
            lastTapTime = -999f;
            return;
        }

        lastTapTime = Time.time;
    }

    private void OnSprint(InputAction.CallbackContext context)
    {
        IsSprintPressed = context.ReadValueAsButton();
    }

    private void OnJumpStarted(InputAction.CallbackContext context)
    {
        JumpPressedThisFrame = true;
        JumpHeld = true;
    }

    private void OnJumpCanceled(InputAction.CallbackContext context)
    {
        JumpHeld = false;
    }

    private void OnAttackStarted(InputAction.CallbackContext context)
    {
        AttackPressedThisFrame = true;
    }

    private void OnLockOnStarted(InputAction.CallbackContext context)
    {
        LockOnPressedThisFrame = true;
    }

    private void OnInteractStarted(InputAction.CallbackContext context)
    {
        InteractPressedThisFrame = true;
    }

    public void ConsumeJumpPressed()
    {
        JumpPressedThisFrame = false;
    }

    public void ConsumeDodgePressed()
    {
        DodgePressedThisFrame = false;
    }

    public void ConsumeAttackPressed()
    {
        AttackPressedThisFrame = false;
    }

    public void ConsumeLockOnPressed()
    {
        LockOnPressedThisFrame = false;
    }

    public void ConsumeInteractPressed()
    {
        InteractPressedThisFrame = false;
    }
}