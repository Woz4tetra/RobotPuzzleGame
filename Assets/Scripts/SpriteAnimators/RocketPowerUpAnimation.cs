using UnityEngine;

public class RocketPowerUpAnimation : MonoBehaviour
{
    [SerializeField] GameObject[] layers;
    private Light[] lights;
    void Start()
    {
        lights = new Light[layers.Length];
        for (int index = 0; index < layers.Length; index++)
        {
            lights[index] = layers[index].GetComponent<Light>();
        }
    }

    public void SetIntensity(float intensity)
    {
        if (lights == null)
        {
            return;
        }
        foreach (Light light in lights)
        {
            light.intensity = intensity;
        }
    }
}
