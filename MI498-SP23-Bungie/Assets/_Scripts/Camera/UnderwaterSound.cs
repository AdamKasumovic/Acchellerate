using UnityEngine;
using Gaia;

[RequireComponent(typeof(AudioLowPassFilter))]
public class UnderwaterSound : MonoBehaviour
{
    private AudioLowPassFilter lowPassFilter;
    public float cutoffFrequency = 500;

    void Start()
    {
        lowPassFilter = GetComponent<AudioLowPassFilter>();
    }

    void Update()
    {
        if (GaiaUnderwaterEffects.Instance != null && GaiaUnderwaterEffects.Instance.IsUnderwater)
        {
            lowPassFilter.enabled = true;
            lowPassFilter.cutoffFrequency = cutoffFrequency;
        }
        else
        {
            lowPassFilter.enabled = false;
        }
    }
}
