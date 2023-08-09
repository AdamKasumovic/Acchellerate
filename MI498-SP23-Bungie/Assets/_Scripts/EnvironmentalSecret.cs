using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnvironmentalSecret : MonoBehaviour
{
    public int basePoints = 1000;
    public bool scaleWithMultiplier = false;
    public bool triggered = false;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.tag != "Player")
        {
            return;
        }
        if (!triggered)
        {
            triggered = true;
            int points = basePoints;
            if (scaleWithMultiplier)
            {
                points = NewStyleSystem.instance.AddPointsWithMultiplier(basePoints);
            }
            else
            {
                NewStyleSystem.instance.AddPoints(basePoints);
            }
            StyleTextbox.instance.AddRewardInfoAndHandleTimeout($"ENVIROMENTAL SECRET FOUND", points, 3, 0, true);
        }
    }
}
