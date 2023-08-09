using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SetVolumeSliders : MonoBehaviour
{
    public string keyName = "";
    private Slider slider;
    // Start is called before the first frame update
    void Start()
    {
        slider = GetComponent<Slider>();
        slider.value = PlayerPrefs.GetFloat(keyName, 0.25f);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
