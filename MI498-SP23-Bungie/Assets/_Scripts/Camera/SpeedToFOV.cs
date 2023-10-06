using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
public class SpeedToFOV : MonoBehaviour
{
    private CinemachineFreeLook cfl;
    public float minFov = 35f;
    public float maxFov = 50f;
    public float slope = 15f;
    // Start is called before the first frame update
    void Start()
    {
        cfl = GetComponent<CinemachineFreeLook>();
        cfl.m_CommonLens = true;
    }

    // Update is called once per frame
    void Update()
    {
        float smoothTime = 0.3f;
        float fovVelocity = 0.0f;
        cfl.m_Lens.FieldOfView = Mathf.SmoothDamp(cfl.m_Lens.FieldOfView, minFov + CarManager.carSpeed * slope / CarManager.carMaxSpeed, ref fovVelocity, smoothTime);
    }
}
