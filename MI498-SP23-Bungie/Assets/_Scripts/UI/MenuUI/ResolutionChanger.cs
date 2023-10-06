using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ResolutionChanger : MonoBehaviour
{
    public Resolution[] resolutions;
    Resolution res;
    FullScreenMode mode = FullScreenMode.ExclusiveFullScreen;
    TMP_Dropdown dd;
    List<Resolution> native_resolutions;
    // Start is called before the first frame update
    void Start()
    {
        native_resolutions = new List<Resolution>();
        resolutions = Screen.resolutions;
        dd = GetComponent<TMP_Dropdown>();
        var standard = resolutions[resolutions.Length - 1];
        foreach (Resolution resolution in resolutions)
        {
            if(resolution.refreshRate == standard.refreshRate && Mathf.Abs((float)standard.width/ (float)standard.height - (float)resolution.width/ (float)resolution.height) < .01f) 
            {
                native_resolutions.Add(resolution);
                dd.options.Add(new TMP_Dropdown.OptionData(resolution.ToString()));
            }
        }
        dd.value = resolutions.Length-1;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void SetResolution()
    {
        int index = dd.value;
        var test = resolutions[index];
        Screen.SetResolution(native_resolutions[index].width, native_resolutions[index].height, mode, native_resolutions[index].refreshRate);
    }
}
