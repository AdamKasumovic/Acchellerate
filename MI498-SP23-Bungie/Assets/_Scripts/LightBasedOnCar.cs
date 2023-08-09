using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightBasedOnCar : MonoBehaviour
{
    [Tooltip("Car name this light should be on for when that car is active.")]
    public string carName = "mustang";
    public Light spotLight;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        string carType = PlayerPrefs.GetString("CarName", "mustang");
        spotLight.enabled = (carType == carName.ToLower());
    }
}
