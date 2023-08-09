using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class ToggleFirstPerson : MonoBehaviour
{
    public CinemachineVirtualCamera cvcam;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F1))
        {
            cvcam.m_Priority = (cvcam.m_Priority == 13 ? -10 : 13);
        }
    }
}
