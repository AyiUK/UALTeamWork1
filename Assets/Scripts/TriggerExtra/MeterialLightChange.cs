using UnityEngine;

public class MeterialLightChange : MonoBehaviour
{
    [SerializeField] private Renderer targetRenderer;
    [SerializeField, Min(0f)] private float intensity = 2f;

    private static readonly int EmissionColorId =
       Shader.PropertyToID("_EmissionColor");
    private void Awake()
    {
        if (targetRenderer == null)
        {
            targetRenderer = GetComponent<Renderer>();
        }
    }

    // Can be assigned to a Trigger's UnityEvent as a no-argument callback.
    public void EnableEmission()
    {
        if (!TryGetTargetMaterial(out Material material))
        {
            return;
        }

        material.EnableKeyword("_EMISSION");
    }

    public void MeterialLightOn(Color color)
    {
        if (!TryGetTargetMaterial(out Material material))
        {
            return;
        }

        material.EnableKeyword("_EMISSION");
        material.SetColor(EmissionColorId, color * intensity);
    }

    private bool TryGetTargetMaterial(out Material material)
    {
        material = null;
       

        material = targetRenderer.material;
        return true;
    }
}
