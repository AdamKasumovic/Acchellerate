using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlickerLights : MonoBehaviour
{
    public Light[] lightsToFlicker;
    public float flickerDurationMin = 0.05f;
    public float flickerDurationMax = 0.5f;
    [Tooltip("The odds every frame that this light will flicker. Should be a very small number between 0 and 1.")]
    [Range(0,1)]
    public float flickerLikelihood = 1 / 120f;
    private float flickerTimer = 0f;
    private float flickerRand;

    public AudioSource flickerSoundOn;
    public AudioSource flickerSoundOff;
    private bool wasEnabled = true;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        foreach(Light l in lightsToFlicker)
        {
            wasEnabled = l.enabled;
            l.enabled = flickerTimer <= 0;
            if (l.enabled != wasEnabled)
            {
                if (l.enabled)
                {
                    if (flickerSoundOn != null)
                        flickerSoundOn.Play();
                }
                else
                {
                    if (flickerSoundOff != null)
                        flickerSoundOff.Play();
                }
            }
        }
        flickerTimer -= Time.deltaTime;

        flickerRand = Random.Range(0f, 1f);

        if (flickerRand <= flickerLikelihood)
        {
            flickerTimer = Random.Range(flickerDurationMin, flickerDurationMax);
        }
    }
}
