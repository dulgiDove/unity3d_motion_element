using UnityEngine;

public class PlayerAnimationEventReceiver : MonoBehaviour
{
    [SerializeField] private PlayerCombatController combatController;

    [Header("Hitboxes")]
    [SerializeField] private AttackHitbox rightHandHitbox;
    [SerializeField] private AttackHitbox leftElbowHitbox;
    [SerializeField] private AttackHitbox rightFootHitbox;

    private void Awake()
    {
        if (combatController == null)
            combatController = GetComponentInParent<PlayerCombatController>();
    }

    public void EnableRightHandHitbox()
    {
        rightHandHitbox.EnableHitbox();
    }

    public void DisableRightHandHitbox()
    {
        rightHandHitbox.DisableHitbox();
    }

    public void EnableLeftElbowHitbox()
    {
        leftElbowHitbox.EnableHitbox();
    }

    public void DisableLeftElbowHitbox()
    {
        leftElbowHitbox.DisableHitbox();
    }

    public void EnableRightFootHitbox()
    {
        rightFootHitbox.EnableHitbox();
    }

    public void DisableRightFootHitbox()
    {
        rightFootHitbox.DisableHitbox();
    }

    public void OpenComboWindow()
    {
        combatController.OpenComboWindow();
    }

    public void CloseComboWindow()
    {
        combatController.CloseComboWindow();
    }

    public void EndAttack()
    {
        combatController.EndAttack();
    }

    public void SetComboIndex1()
    {
        combatController.SetCurrentComboIndex(1);
    }

    public void SetComboIndex2()
    {
        combatController.SetCurrentComboIndex(2);
    }

    public void SetComboIndex3()
    {
        combatController.SetCurrentComboIndex(3);
    }
}