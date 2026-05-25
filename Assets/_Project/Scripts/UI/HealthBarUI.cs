using UnityEngine;
using UnityEngine.UI;

public class HealthBarUI : MonoBehaviour
{
    [SerializeField] private Health targetHealth;
    [SerializeField] private Slider healthSlider;

    private void Awake()
    {
        if (healthSlider == null)
            healthSlider = GetComponent<Slider>();
    }

    private void OnEnable()
    {
        if (targetHealth == null)
            return;

        targetHealth.OnHealthChanged += UpdateHealthBar;
        UpdateHealthBar(targetHealth.CurrentHealth, targetHealth.MaxHealth);
    }

    private void OnDisable()
    {
        if (targetHealth == null)
            return;

        targetHealth.OnHealthChanged -= UpdateHealthBar;
    }

    private void UpdateHealthBar(float currentHealth, float maxHealth)
    {
        healthSlider.value = currentHealth / maxHealth;
    }
}