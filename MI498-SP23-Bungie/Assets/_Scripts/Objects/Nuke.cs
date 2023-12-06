using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Gaia;

public class Nuke : MonoBehaviour
{
    public GameObject shockwavePrefab;

    private void Start()
    {
        GaiaAPI.StartWeatherRain();
    }

    void OnCollisionEnter(Collision collision)
    {
        // Check if the collision is with the ground
        if (collision.gameObject.tag == "Environment")
        {
            //Debug.Log("Touched the ground!");
            Instantiate(shockwavePrefab, transform.position, Quaternion.identity, null);
            Destroy(gameObject);
        }
    }
}
