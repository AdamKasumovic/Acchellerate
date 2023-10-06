using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Letters : MonoBehaviour
{

    public char myChar { get; set; }

    private void OnTriggerEnter(Collider other)
    {
        gameObject.SetActive(false);
        Missions.Instance.RegisterLetter(myChar);
        Debug.Log("Char Collected");
    }
}
