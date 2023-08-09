using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class CameraShaker : MonoBehaviour
{
    public static CameraShaker Instance { get; private set; }
    private CinemachineFreeLook cfl;
    private CinemachineBasicMultiChannelPerlin cbmcp;
    private float shakeTimer;
    private float shakeTimerTotal;
    private float startingIntensity;
    private bool doLerp = false;

    private void Awake()
    {
        Instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        cfl = GetComponent<CinemachineFreeLook>();
        cbmcp = cfl.GetComponentInChildren<CinemachineBasicMultiChannelPerlin>();
    }

    public void ShakeCamera(float intensity, float time)
    {
        cbmcp.m_AmplitudeGain = intensity;
        shakeTimer = time;
        shakeTimerTotal = time;
        startingIntensity = intensity;
    }

    public void SetLerp(bool lerp)
    {
        doLerp = lerp;
    }

    private void Update()
    {
        if (shakeTimer > 0)
        {
            shakeTimer -= Time.unscaledDeltaTime;
            if (shakeTimer <= 0)
            {
                if (doLerp)
                {
                    cbmcp.m_AmplitudeGain = Mathf.Lerp(startingIntensity, 0, 1 - (shakeTimer / shakeTimerTotal));
                }
                else
                {
                    cbmcp.m_AmplitudeGain = 0;
                }
                
            }
        }
    }
}
