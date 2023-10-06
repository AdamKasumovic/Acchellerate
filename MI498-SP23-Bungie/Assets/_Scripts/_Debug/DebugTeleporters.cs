using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebugTeleporters : MonoBehaviour
{
    public GameObject car;
    private Transform[] teleporters;
    private KeyCode[] keyCodes = {
         KeyCode.F1,
         KeyCode.F2,
         KeyCode.F3,
         KeyCode.F4,
         KeyCode.F5,
         KeyCode.F6,
         KeyCode.F7,
         KeyCode.F8,
         KeyCode.F9,
         KeyCode.F10
     };
    // Start is called before the first frame update
    void Start()
    {
        teleporters = GetComponentsInChildren<Transform>();
    }

    // Update is called once per frame
    void Update()
    {
        for (int i = 1; i < Mathf.Min(teleporters.Length, 11); i++)
        {
            if (Input.GetKeyDown(keyCodes[i-1]))
            {
                car.transform.position = teleporters[i].transform.position;
                car.transform.rotation = teleporters[i].transform.rotation;
                var rb = car.GetComponent<Rigidbody>();
                rb.velocity = Vector3.zero;
                rb.angularVelocity = Vector3.zero;
            }
        }
    }
}
