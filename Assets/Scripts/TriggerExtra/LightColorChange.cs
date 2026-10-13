using UnityEngine;

public class LightColorChange : MonoBehaviour
{
    Light StartLight;

    

    void Start()
    {
        transform.SetParent(null, true);
        StartLight = GetComponent<Light>();
        StartLight.color = Color.red;
    }

    public void ChangeColor()
    {
        StartLight.color= Color.green;
    }
}
