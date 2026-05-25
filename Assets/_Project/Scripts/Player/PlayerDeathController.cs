using System.Collections;
using UnityEngine;

public class PlayerDeathController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Health health;
    [SerializeField] private Animator animator;
    [SerializeField] private GameObject deathPanel;

    [Header("Death")]
    [SerializeField] private float deathPanelDelay = 1.5f;

    private static readonly int DieTriggerHash = Animator.StringToHash("DieTrigger");

    public bool IsDead { get; private set; }

    private void Awake()
    {
        if (health == null)
            health = GetComponent<Health>();

        if (animator == null)
            animator = GetComponentInChildren<Animator>();

        if (deathPanel != null)
            deathPanel.SetActive(false);
    }

    private void OnEnable()
    {
        if (health != null)
            health.OnDied += HandleDeath;
    }

    private void OnDisable()
    {
        if (health != null)
            health.OnDied -= HandleDeath;
    }

    private void HandleDeath()
    {
        if (IsDead)
            return;

        IsDead = true;

        if (animator != null)
            animator.SetTrigger(DieTriggerHash);

        StartCoroutine(ShowDeathPanelRoutine());
    }

    private IEnumerator ShowDeathPanelRoutine()
    {
        yield return new WaitForSeconds(deathPanelDelay);

        if (deathPanel != null)
            deathPanel.SetActive(true);
    }
}