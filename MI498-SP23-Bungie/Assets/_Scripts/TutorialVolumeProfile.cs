using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class TutorialVolumeProfile : MonoBehaviour
{
    public VolumeProfile volumeProfile;
    private Volume volume;
    // Start is called before the first frame update
    void Start()
    {
        volume = GetComponent<Volume>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Tutorial.enteredAreas[6])
        {
            volume.profile = volumeProfile;
        }
    }
}
