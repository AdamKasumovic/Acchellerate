using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SecretCollectable : MonoBehaviour
{
    public bool trigger = false;
    private void OnTriggerEnter(Collider other)
    {
        Debug.Log(other.gameObject);
        if (other.CompareTag("Player"))
        {
            trigger = true;
            Missions.Instance.RegisterCollect();
            Missions.Instance.RegisterHeight();
        }
    }
}
