using System.Collections;
using UnityEngine;

public class EnemyHitReaction : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Rigidbody rb;

    [Header("Hit Flash")]
    [SerializeField] private Renderer[] meshRenderers;
    [SerializeField] private Color flashColor = Color.white;
    [SerializeField] private float flashDuration = 0.08f;

    private Color[] originalColors;

    private static readonly int BaseColorHash = Shader.PropertyToID("_BaseColor");
    private static readonly int ColorHash = Shader.PropertyToID("_Color");

    private void Awake()
    {
        if (rb == null)
            rb = GetComponent<Rigidbody>();

        if (meshRenderers == null || meshRenderers.Length == 0)
            meshRenderers = GetComponentsInChildren<Renderer>();

        originalColors = new Color[meshRenderers.Length];

        for (int i = 0; i < meshRenderers.Length; i++)
        {
            Material mat = meshRenderers[i].material;

            if (mat.HasProperty(BaseColorHash))
                originalColors[i] = mat.GetColor(BaseColorHash);
            else if (mat.HasProperty(ColorHash))
                originalColors[i] = mat.GetColor(ColorHash);
            else
                originalColors[i] = Color.white;
        }
    }

    public void PlayHitReaction(
        Vector3 hitDirection,
        float knockbackForce,
        float knockbackUpwardForce
    )
    {
        ApplyKnockback(hitDirection, knockbackForce, knockbackUpwardForce);

        StopAllCoroutines();
        StartCoroutine(HitFlashRoutine());
    }

    private void ApplyKnockback(
        Vector3 hitDirection,
        float knockbackForce,
        float knockbackUpwardForce
    )
    {
        if (rb == null)
            return;

        Vector3 force = hitDirection * knockbackForce;
        force.y = knockbackUpwardForce;

        rb.AddForce(force, ForceMode.Impulse);
    }

    private IEnumerator HitFlashRoutine()
    {
        SetFlashColor(flashColor);

        yield return new WaitForSeconds(flashDuration);

        RestoreOriginalColors();
    }

    private void SetFlashColor(Color color)
    {
        foreach (Renderer renderer in meshRenderers)
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
        for (int i = 0; i < meshRenderers.Length; i++)
        {
            Material mat = meshRenderers[i].material;

            if (mat.HasProperty(BaseColorHash))
                mat.SetColor(BaseColorHash, originalColors[i]);
            else if (mat.HasProperty(ColorHash))
                mat.SetColor(ColorHash, originalColors[i]);
        }
    }
}