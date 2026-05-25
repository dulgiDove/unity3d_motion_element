using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerMotor : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float walkSpeed = 4.5f;
    [SerializeField] private float sprintSpeed = 7.0f;

    [Header("Rotation")]
    [SerializeField] private float rotationSpeed = 15f;

    [Header("Jump")]
    [SerializeField] private float jumpHeight = 2.2f;
    [SerializeField] private float coyoteTime = 0.12f;
    [SerializeField] private float jumpBufferTime = 0.12f;
    [SerializeField] private float jumpCutVelocityMultiplier = 0.35f;

    [Header("Gravity")]
    [SerializeField] private float gravity = -24f;
    [SerializeField] private float groundedGravity = -2f;
    [SerializeField] private float fallGravityMultiplier = 1.5f;

    [Header("Dodge Speed")]
    [SerializeField] private float forwardDodgeSpeed = 7f;
    [SerializeField] private float sideDodgeSpeed = 6f;
    [SerializeField] private float backwardDodgeSpeed = 4.5f;

    [Header("Dodge Duration")]
    [SerializeField] private float forwardDodgeDuration = 0.6f;
    [SerializeField] private float sideDodgeDuration = 1.45f;
    [SerializeField] private float backwardDodgeDuration = 2.7f;
    [SerializeField] private float dodgeCooldown = 0.2f;

    [Header("Attack Movement")]
    [SerializeField] private float attackMoveMultiplier = 0.3f;

    public float CurrentHorizontalSpeed { get; private set; }
    public float VerticalVelocity => verticalVelocity;
    public bool IsGrounded => characterController.isGrounded;
    public bool IsDodging => isDodging;
    public Vector2 DodgeInput { get; private set; }

    private CharacterController characterController;

    private float verticalVelocity;
    private float coyoteTimer;
    private float jumpBufferTimer;

    private bool hasJumpCut;

    private bool isDodging;
    private float dodgeTimer;
    private float dodgeCooldownTimer;
    private float currentDodgeSpeed;
    private Vector3 dodgeDirection;

    private void Awake()
    {
        characterController = GetComponent<CharacterController>();
    }

    public void Move(
        Vector2 moveInput,
        bool isSprinting,
        bool jumpPressed,
        bool jumpHeld,
        bool dodgePressed,
        Vector2 dodgeInput,
        bool isAttacking,
        bool canMoveDuringAttack,
        bool isLockedOn,
        Transform lockOnTarget,
        Transform cameraTransform
    )
    {
        Vector3 moveDirection = GetCameraRelativeDirection(moveInput, cameraTransform);

        // 공격 중에는 이동, 점프, 회피 금지
        if (isAttacking && !isDodging)
        {
            jumpPressed = false;
            jumpHeld = false;
            dodgePressed = false;

            if (!canMoveDuringAttack)
                moveDirection = Vector3.zero;
        }

        // 점프 중에는 회피 금지
        if (!characterController.isGrounded)
        {
            dodgePressed = false;
        }

        UpdateTimers(jumpPressed);
        UpdateDodgeCooldown();

        if (dodgePressed)
        {
            Vector3 dodgeMoveDirection = GetCameraRelativeDirection(dodgeInput, cameraTransform);
            TryStartDodge(dodgeMoveDirection, dodgeInput);
        }

        ApplyGravity();

        if (isDodging)
        {
            UpdateDodge();
        }
        else
        {
            HandleJump(jumpHeld);
            float movementMultiplier = isAttacking && canMoveDuringAttack
                ? attackMoveMultiplier
                : 1f;

            MoveNormally(moveDirection, isSprinting, movementMultiplier);
        }

        if (isLockedOn && lockOnTarget != null)
            RotateToTarget(lockOnTarget);
        else
            RotateToCameraForward(cameraTransform);
    }

    private void MoveNormally(Vector3 moveDirection, bool isSprinting, float movementMultiplier = 1f)
    {
        float speed = isSprinting ? sprintSpeed : walkSpeed;
        speed *= movementMultiplier;

        CurrentHorizontalSpeed = moveDirection.magnitude * speed;

        Vector3 velocity = moveDirection * speed;
        velocity.y = verticalVelocity;

        characterController.Move(velocity * Time.deltaTime);
    }

    private void TryStartDodge(Vector3 moveDirection, Vector2 rawDodgeInput)
    {
        if (isDodging)
            return;

        if (dodgeCooldownTimer > 0f)
            return;

        if (!characterController.isGrounded)
            return;

        if (moveDirection.sqrMagnitude < 0.01f)
            return;

        DodgeInput = rawDodgeInput.normalized;
        dodgeDirection = moveDirection.normalized;

        currentDodgeSpeed = GetDodgeSpeed(DodgeInput);
        dodgeTimer = GetDodgeDuration(DodgeInput);

        isDodging = true;
        dodgeCooldownTimer = dodgeCooldown;
    }

    private void UpdateDodge()
    {
        dodgeTimer -= Time.deltaTime;

        Vector3 velocity = dodgeDirection * currentDodgeSpeed;
        velocity.y = verticalVelocity;

        CurrentHorizontalSpeed = currentDodgeSpeed;

        characterController.Move(velocity * Time.deltaTime);

        if (dodgeTimer <= 0f)
            isDodging = false;
    }

    private float GetDodgeDuration(Vector2 dodgeInput)
    {
        if (dodgeInput.y < -0.5f)
            return backwardDodgeDuration;

        if (Mathf.Abs(dodgeInput.x) > 0.5f)
            return sideDodgeDuration;

        return forwardDodgeDuration;
    }

    private float GetDodgeSpeed(Vector2 dodgeInput)
    {
        if (dodgeInput.y < -0.5f)
            return backwardDodgeSpeed;

        if (Mathf.Abs(dodgeInput.x) > 0.5f)
            return sideDodgeSpeed;

        return forwardDodgeSpeed;
    }

    private void UpdateDodgeCooldown()
    {
        if (dodgeCooldownTimer > 0f)
            dodgeCooldownTimer -= Time.deltaTime;
    }

    private void UpdateTimers(bool jumpPressed)
    {
        if (characterController.isGrounded)
        {
            coyoteTimer = coyoteTime;
            hasJumpCut = false;
        }
        else
        {
            coyoteTimer -= Time.deltaTime;
        }

        if (jumpPressed)
            jumpBufferTimer = jumpBufferTime;
        else
            jumpBufferTimer -= Time.deltaTime;
    }

    private void HandleJump(bool jumpHeld)
    {
        bool canJump = coyoteTimer > 0f;
        bool hasBufferedJump = jumpBufferTimer > 0f;

        if (canJump && hasBufferedJump)
        {
            verticalVelocity = Mathf.Sqrt(jumpHeight * -2f * gravity);

            coyoteTimer = 0f;
            jumpBufferTimer = 0f;
            hasJumpCut = false;
        }

        if (!jumpHeld && verticalVelocity > 0f && !hasJumpCut)
        {
            verticalVelocity *= jumpCutVelocityMultiplier;
            hasJumpCut = true;
        }
    }

    private void ApplyGravity()
    {
        if (characterController.isGrounded && verticalVelocity < 0f)
        {
            verticalVelocity = groundedGravity;
            return;
        }

        float gravityMultiplier = verticalVelocity < 0f ? fallGravityMultiplier : 1f;
        verticalVelocity += gravity * gravityMultiplier * Time.deltaTime;
    }

    private Vector3 GetCameraRelativeDirection(Vector2 moveInput, Transform cameraTransform)
    {
        Vector3 forward = cameraTransform.forward;
        Vector3 right = cameraTransform.right;

        forward.y = 0f;
        right.y = 0f;

        forward.Normalize();
        right.Normalize();

        Vector3 direction = forward * moveInput.y + right * moveInput.x;

        if (direction.sqrMagnitude > 1f)
            direction.Normalize();

        return direction;
    }

    private void RotateToCameraForward(Transform cameraTransform)
    {
        Vector3 forward = cameraTransform.forward;
        forward.y = 0f;

        if (forward.sqrMagnitude < 0.01f)
            return;

        Quaternion targetRotation = Quaternion.LookRotation(forward);

        transform.rotation = Quaternion.Slerp(
            transform.rotation,
            targetRotation,
            rotationSpeed * Time.deltaTime
        );
    }

    private void RotateToTarget(Transform target)
    {
        Vector3 direction = target.position - transform.position;
        direction.y = 0f;

        if (direction.sqrMagnitude < 0.01f)
            return;

        Quaternion targetRotation = Quaternion.LookRotation(direction);

        transform.rotation = Quaternion.Slerp(
            transform.rotation,
            targetRotation,
            rotationSpeed * Time.deltaTime
        );
    }
}