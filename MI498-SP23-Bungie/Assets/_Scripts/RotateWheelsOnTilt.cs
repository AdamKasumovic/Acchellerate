using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateWheelsOnTilt : MonoBehaviour
{
    public float rotationSpeed = 256f;
    public bool left = true;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if ((CarManager.currentState == CarManager.CarState.TiltingLeft && left) || (CarManager.currentState == CarManager.CarState.TiltingRight && !left))
            transform.Rotate(rotationSpeed * Time.deltaTime * CarManager.carSpeed, 0, 0); //rotates 50 degrees per second around z axis
    }
}
