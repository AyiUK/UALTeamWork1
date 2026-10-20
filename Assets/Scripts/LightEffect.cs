using UnityEngine;

public class LightEffect : MonoBehaviour
{
    Light EffectLight;
    float Intensity = 1f;
    float timer = 0f;
    public float Waves = 0f;
    public float Speed = 1f;
    void Start()
    {
        EffectLight = GetComponent<Light>();
       

    }

    // Update is called once per frame
    void Update()
    {
        timer += Time.deltaTime*Speed;

        if (timer >= 1f)
        {
            timer -= 1f;

            float variation = Random.Range(-Mathf.Abs(Waves), Mathf.Abs(Waves));
            Intensity = Mathf.Max(0f, Random.Range(0.8f, 2f) + variation);
            EffectLight.intensity = Intensity;
        }
        
    }
}
