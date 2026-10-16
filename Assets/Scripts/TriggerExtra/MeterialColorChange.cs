using UnityEngine;

public class MeterialColorChange : MonoBehaviour
{
    [SerializeField] private Renderer targetRenderer;
    [SerializeField, Min(0f)] private float intensity = 2f;

    private static readonly int EmissionColorId =
        Shader.PropertyToID("_EmissionColor");

    private void Start()
    {
        transform.SetParent(null, true);

        if (targetRenderer == null)
            targetRenderer = GetComponent<Renderer>();

        SetEmissionColor(Color.red);
    }

    public void MeterialChangeColor()
    {
        SetEmissionColor(Color.green);
    }

    private void SetEmissionColor(Color color)
    {
        // material 会创建此 Renderer 独有的材质实例。
        Material material = targetRenderer.material;
        material.EnableKeyword("_EMISSION");
        material.SetColor(EmissionColorId, color * intensity);
    }
}

