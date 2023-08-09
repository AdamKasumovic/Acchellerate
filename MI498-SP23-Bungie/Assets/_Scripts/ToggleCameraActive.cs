using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class ToggleCameraActive : MonoBehaviour
{
    public int highPriority = 11;
    public int lowPriority = 9;
    private CinemachineVirtualCamera vcam;

    // Start is called before the first frame update
    void Start()
    {
        vcam = GetComponent<CinemachineVirtualCamera>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void TogglePriority()
    {
        vcam.m_Priority = vcam.m_Priority == highPriority ? lowPriority : highPriority;
    }
}
