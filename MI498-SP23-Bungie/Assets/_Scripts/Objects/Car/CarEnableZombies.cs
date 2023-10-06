using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarEnableZombies : MonoBehaviour
{
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
    private void OnTriggerEnter(Collider other)
    {
        var stats = other.GetComponent<EnemyStats>();
        var navmesh = other.GetComponent<EnemyNavmesh>();
        if (stats != null)
        {
            stats.insidePlayer = true;
        }
       /* if(navmesh != null && CarManager.carSpeed > 20f)
        {
            navmesh.animator.SetTrigger("Scared");
        }*/
    }
    private void OnTriggerExit(Collider other)
    {
        var stats = other.GetComponent<EnemyStats>();
        if(stats != null)
        {
            stats.insidePlayer = false;
        }
    }
}
