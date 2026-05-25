using UnityEngine;

public class PlayerAnimationController : MonoBehaviour
{
    [SerializeField] private Animator animator;
    [SerializeField] private PlayerMotor motor;

    private static readonly int MoveSpeedHash = Animator.StringToHash("MoveSpeed");
    private static readonly int IsGroundedHash = Animator.StringToHash("IsGrounded");
    private static readonly int VerticalVelocityHash = Animator.StringToHash("VerticalVelocity");

    private static readonly int IsDodgingHash = Animator.StringToHash("IsDodging");
    private static readonly int DodgeXHash = Animator.StringToHash("DodgeX");
    private static readonly int DodgeYHash = Animator.StringToHash("DodgeY");

    private void Reset()
    {
        motor = GetComponent<PlayerMotor>();
        animator = GetComponentInChildren<Animator>();
    }

    private void Awake()
    {
        if (motor == null)
            motor = GetComponent<PlayerMotor>();

        if (animator == null)
            animator = GetComponentInChildren<Animator>();
    }

    private void Update()
    {
        float normalizedMoveSpeed = Mathf.InverseLerp(
            0f,
            7.0f,
            motor.CurrentHorizontalSpeed
        );

        animator.SetFloat(MoveSpeedHash, normalizedMoveSpeed);
        animator.SetBool(IsGroundedHash, motor.IsGrounded);
        animator.SetFloat(VerticalVelocityHash, motor.VerticalVelocity);

        animator.SetBool(IsDodgingHash, motor.IsDodging);
        animator.SetFloat(DodgeXHash, motor.DodgeInput.x);
        animator.SetFloat(DodgeYHash, motor.DodgeInput.y);
    }
}