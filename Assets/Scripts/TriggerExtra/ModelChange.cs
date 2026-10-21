using UnityEngine;

public class ModelChange : MonoBehaviour
{

    [SerializeField] private GameObject currentModel;
    [SerializeField] private GameObject targetPrefab;
    [SerializeField] private Transform spawnParent;

    public void SwitchModel()
    {
        if (currentModel == null || targetPrefab == null)
        {
            Debug.LogWarning("ModelChange: 请在 Inspector 中指定 Current Model 和 Target Prefab。", this);
            return;
        }

        Transform oldTransform = currentModel.transform;
        Transform spawnTransform = spawnParent != null ? spawnParent : oldTransform;
        Vector3 position = spawnTransform.position;
        Quaternion rotation = spawnTransform.rotation;
        Vector3 scale = oldTransform.lossyScale;

        GameObject newModel = Instantiate(
            targetPrefab,
            position,
            rotation,
            spawnParent
        );

        


        // Hide the GameObject carrying this component, including child renderers.
        foreach (Renderer renderer in GetComponentsInChildren<Renderer>(true))
        {
            renderer.enabled = false;
        }

        currentModel = newModel;
        Debug.Log("modelchange");

    }
}
