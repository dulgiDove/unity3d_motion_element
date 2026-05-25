using System.Collections;
using UnityEngine;

public class PlayerHitReaction : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Health health;
    [SerializeField] private Renderer[] renderers;

    [Header("Flash")]
    [SerializeField] private Color flashColor = Color.white;
    [SerializeField] private float flashDuration = 0.08f;

    private Color[] originalColors;
    private Coroutine flashCoroutine;

    private static readonly int BaseColorHash = Shader.PropertyToID("_BaseColor");
    private static readonly int ColorHash = Shader.PropertyToID("_Color");

    private void Awake()
    {
        if (health == null)
            health = GetComponent<Health>();

        if (renderers == null || renderers.Length == 0)
            renderers = GetComponentsInChildren<Renderer>();

        CacheOriginalColors();
    }

    private void OnEnable()
    {
        if (health != null)
            health.OnHealthChanged += HandleHealthChanged;
    }

    private void OnDisable()
    {
        if (health != null)
            health.OnHealthChanged -= HandleHealthChanged;
    }

    private void HandleHealthChanged(float currentHealth, float maxHealth)
    {
        PlayFlash();
    }

    private void PlayFlash()
    {
        if (flashCoroutine != null)
            StopCoroutine(flashCoroutine);

        flashCoroutine = StartCoroutine(FlashRoutine());
    }

    private IEnumerator FlashRoutine()
    {
        SetColor(flashColor);

        yield return new WaitForSeconds(flashDuration);

        RestoreOriginalColors();

        flashCoroutine = null;
    }

    private void CacheOriginalColors()
    {
        originalColors = new Color[renderers.Length];

        for (int i = 0; i < renderers.Length; i++)
        {
            Material mat = renderers[i].material;

            if (mat.HasProperty(BaseColorHash))
                originalColors[i] = mat.GetColor(BaseColorHash);
            else if (mat.HasProperty(ColorHash))
                originalColors[i] = mat.GetColor(ColorHash);
            else
                originalColors[i] = Color.white;
        }
    }

    private void SetColor(Color color)
    {
        foreach (Renderer renderer in renderers)
        {
            Material mat = renderer.material;

            if (mat.HasProperty(BaseColorHash))
                mat.SetColor(BaseColorHash, color);
            else if (mat.HasProperty(ColorHash))
                mat.SetColor(ColorHash, color);
        }
    }

    private void RestoreOriginalColors()
    {
        for (int i = 0; i < renderers.Length; i++)
        {
            Material mat = renderers[i].material;

            if (mat.HasProperty(BaseColorHash))
                mat.SetColor(BaseColorHash, originalColors[i]);
            else if (mat.HasProperty(ColorHash))
                mat.SetColor(ColorHash, originalColors[i]);
        }
    }
}