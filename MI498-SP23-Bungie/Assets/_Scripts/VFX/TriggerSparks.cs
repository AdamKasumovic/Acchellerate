using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class TriggerSparks : MonoBehaviour
{
    public GameObject particleSystemPrefab;
    Camera cam;

    // Update is called once per frame
    void Update()
    {
        SpawnAtMousePos();
        cam = Camera.main;
    }

    void SpawnAtMousePos()
    {
        if (Mouse.current.leftButton.wasPressedThisFrame)
        {
            Ray ray = cam.ScreenPointToRay(Mouse.current.position.ReadValue());
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit))
            {
                GameObject particleSystem = Instantiate(particleSystemPrefab, hit.point, Quaternion.identity);
                Destroy(particleSystem, 1f);
            }
        }
    }
}
