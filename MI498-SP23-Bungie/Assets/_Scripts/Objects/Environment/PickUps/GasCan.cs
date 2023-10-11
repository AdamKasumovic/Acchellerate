using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GasCan : MonoBehaviour
{
    public float amountTimeToRestore = 10f;
    public PedestalDecals parent;
    bool triggered = false;
    private Missions missionsComponent;
    // Start is called before the first frame update
    void Start()
    {
        missionsComponent = Missions.Instance;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    private void OnTriggerEnter(Collider other)
    {
        if (!triggered)
        {
            triggered = true;
            LevelTimer.numSeconds += amountTimeToRestore;
            if (LevelTimer.numSeconds >= LevelTimer.initTme)
            {
                LevelTimer.numSeconds = LevelTimer.initTme;
            }
            missionsComponent.RegisterGasPickup();
            NewStyleSystem.instance.AddPoints(1000);
            StyleTextbox.instance.AddRewardInfoAndHandleTimeout($"GAS CAN FOUND", 1000, 3, 0, true);
            parent.timeCollected = Time.time;
            Destroy(gameObject);
            parent.collected = true;
        }


    }
}
