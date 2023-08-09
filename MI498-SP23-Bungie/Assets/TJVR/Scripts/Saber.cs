using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Saber : MonoBehaviour
{
    public AudioClip beamAudio;
    public AudioClip beamOffAudio;

    private AudioSource audioSource;
    private Animator animator;

    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();
    }

    public void TriggerBeam()
    {
        bool isOn = animator.GetBool("LightSaberOn");
        if (!isOn)
        {
            audioSource.Stop();
            audioSource.PlayOneShot(beamAudio);
        }
        else
        {
            audioSource.Stop();
            audioSource.PlayOneShot(beamOffAudio);
        }
        animator.SetBool("LightSaberOn", !isOn);
    }
}