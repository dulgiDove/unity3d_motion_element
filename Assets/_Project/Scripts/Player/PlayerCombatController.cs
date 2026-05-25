using UnityEngine;

public class PlayerCombatController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Animator animator;

    [Header("Combo")]
    [SerializeField] private int maxComboCount = 3;

    private static readonly int AttackTriggerHash = Animator.StringToHash("AttackTrigger");
    private static readonly int AttackIndexHash = Animator.StringToHash("AttackIndex");

    private int currentComboIndex;
    private bool isAttacking;
    private bool canQueueNextAttack;
    public bool IsAttacking => isAttacking;
    public bool CanMoveDuringAttack => isAttacking && canQueueNextAttack;

    private void Awake()
    {
        if (animator == null)
            animator = GetComponentInChildren<Animator>();
    }

    public void TryAttack()
    {
        if (!isAttacking)
        {
            StartFirstAttack();
            return;
        }

        if (!canQueueNextAttack)
            return;

        if (currentComboIndex >= maxComboCount)
            return;

        int nextComboIndex = currentComboIndex + 1;
        animator.SetInteger(AttackIndexHash, nextComboIndex);
    }

    private void StartFirstAttack()
    {
        isAttacking = true;
        canQueueNextAttack = false;
        currentComboIndex = 1;

        animator.SetInteger(AttackIndexHash, currentComboIndex);
        animator.SetTrigger(AttackTriggerHash);
    }

    public void SetCurrentComboIndex(int comboIndex)
    {
        currentComboIndex = comboIndex;
    }

    public void OpenComboWindow()
    {
        canQueueNextAttack = true;
    }

    public void CloseComboWindow()
    {
        canQueueNextAttack = false;
    }

    public void EndAttack()
    {
        ResetCombo();
    }

    private void ResetCombo()
    {
        currentComboIndex = 0;
        isAttacking = false;
        canQueueNextAttack = false;

        animator.SetInteger(AttackIndexHash, 0);
    }
}