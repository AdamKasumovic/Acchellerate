using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine.PostFX;
using UnityEngine.Rendering;

public class BloodCameraVolumeEffect : MonoBehaviour
{
    [Tooltip("Car health to have maximum blood effect intensity.")]
    public float healthForMaximumIntensity = 5f;

    [Tooltip("Car health when blood effect kicks in.")]
    public float healthForMinimumIntensity = 20f;

    [Tooltip("Maximum intensity of blood effect.")]
    [Range(0f, 1f)]
    public float intensity = 0.5f;

    private CinemachineVolumeSettings cvs;

    //private Vignette vig;
    float yVelocity;

    // Start is called before the first frame update
    void Start()
    {
        //cvs = GetComponent<CinemachineVolumeSettings>();
        //cvs.m_Profile.TryGet<Vignette>(out vig);
        //vig.intensity.value = 0;
    }

    // Update is called once per frame
    void Update()
    {
        // MATH!
        float targetValue = Mathf.Lerp(0, intensity, (-CarManager.carHealth+healthForMinimumIntensity)/(healthForMinimumIntensity-healthForMaximumIntensity));
        //vig.intensity.value = Mathf.SmoothDamp(vig.intensity.value, targetValue, ref yVelocity, 0.3f);
    }
}
