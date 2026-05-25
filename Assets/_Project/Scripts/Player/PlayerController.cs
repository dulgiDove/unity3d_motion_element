using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private PlayerInputReader inputReader;
    [SerializeField] private PlayerMotor motor;
    [SerializeField] private Transform cameraTransform;
    [SerializeField] private PlayerCombatController combatController;
    [SerializeField] private PlayerLockOnController lockOnController;
    [SerializeField] private PlayerInteractionController interactionController;
    [SerializeField] private PlayerDeathController deathController;

    private void Reset()
    {
        inputReader = GetComponent<PlayerInputReader>();
        motor = GetComponent<PlayerMotor>();
        combatController = GetComponent<PlayerCombatController>();

        if (Camera.main != null)
            cameraTransform = Camera.main.transform;
    }

    private void Awake()
    {
        if (inputReader == null)
            inputReader = GetComponent<PlayerInputReader>();

        if (motor == null)
            motor = GetComponent<PlayerMotor>();

        if (cameraTransform == null && Camera.main != null)
            cameraTransform = Camera.main.transform;

        if(combatController == null)
            combatController = GetComponent<PlayerCombatController>();

        if (lockOnController == null)
            lockOnController = GetComponent<PlayerLockOnController>();

        if (interactionController == null)
            interactionController = GetComponent<PlayerInteractionController>();

        if (deathController == null)
            deathController = GetComponent<PlayerDeathController>();
    }

    private void Update()
    {
        bool isAttacking = combatController != null && combatController.IsAttacking;
        bool canMoveDuringAttack = combatController != null && combatController.CanMoveDuringAttack;
        bool isDodging = motor.IsDodging;

        if (deathController != null && deathController.IsDead)
            return;

        if (inputReader.LockOnPressedThisFrame)
            lockOnController.ToggleLockOn();

        if (inputReader.InteractPressedThisFrame)
        {
            interactionController.HandleInteract();
        }

        motor.Move(
            inputReader.MoveInput,
            inputReader.IsSprintPressed,
            inputReader.JumpPressedThisFrame,
            inputReader.JumpHeld,
            inputReader.DodgePressedThisFrame,
            inputReader.DodgeInput,
            isAttacking,
            canMoveDuringAttack,
            lockOnController.IsLockedOn,
            lockOnController.CurrentTarget,
            cameraTransform
        );

        if (inputReader.AttackPressedThisFrame && !isDodging)
            combatController.TryAttack();

        inputReader.ConsumeJumpPressed();
        inputReader.ConsumeDodgePressed();
        inputReader.ConsumeAttackPressed();
        inputReader.ConsumeLockOnPressed();
        inputReader.ConsumeInteractPressed();
    }
}