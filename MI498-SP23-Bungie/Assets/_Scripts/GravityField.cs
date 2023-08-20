using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GravityField : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        CarManager cm = other.gameObject.GetComponent<CarManager>();
        if (cm != null)
        {
            cm.insideGravityField = true;
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
