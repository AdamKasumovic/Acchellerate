using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IsPlayerInside : MonoBehaviour
{
    private Missions missionsComponent;
    void Start()
    {
        missionsComponent = Missions.Instance;
    }
    void OnTriggerExit(Collider collider)
    {
        if(collider.tag == "Player")
        {
            missionsComponent.RegisterColliderExit();
        }
    }
    
}
