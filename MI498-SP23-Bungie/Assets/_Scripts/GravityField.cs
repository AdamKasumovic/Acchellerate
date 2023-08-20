using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GravityField : MonoBehaviour
{
    [Tooltip("The car must be moving at least this fast in MPH to be affected by the gravity field.")]
    public float minSpeedForGravityField = 0;
    private void OnTriggerEnter(Collider other)
    {
        CarManager cm = other.gameObject.GetComponent<CarManager>();
        if (cm != null)
        {
            cm.insideGravityField = true;
            cm.minSpeedForGravityField = minSpeedForGravityField;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        CarManager cm = other.gameObject.GetComponent<CarManager>();
        if (cm != null)
        {
            cm.insideGravityField = false;
        }
    }
}
