using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class GunModeToggle : MonoBehaviour
{
    public CinemachineFreeLook drivingCam;
    public CinemachineFreeLook shootingCam;
    public int highPriority = 12;
    public int lowPriority = 10;

    public CinemachineVirtualCamera rearView;
    public int rearViewHigh = 15;
    public int rearViewLow = 9;

    private bool shooting = false;
    private bool wasShooting = false;

    public float timeBeforeCarCamera = 5f;
    private float timer = 0f;

    private CarManager cm;
    // Start is called before the first frame update
    void Start()
    {
        cm = CarManager.Instance;
    }

    // Update is called once per frame
    void Update()
    {

        if (Input.GetButtonDown("Fire2"))
        {
            //shooting = !shooting;
        }
        if (shooting)
        {
            if (!wasShooting)
                shootingCam.m_XAxis.Value = 0;
            shootingCam.m_Priority = highPriority;
            drivingCam.m_Priority = lowPriority;
            // enable reticle here
        }
        else
        {
            if (wasShooting)
                drivingCam.m_XAxis.Value = 0;
            drivingCam.m_Priority = highPriority;
            shootingCam.m_Priority = lowPriority;
            // disable reticle here
        }
        wasShooting = shooting;

        //if (!(Mathf.Abs(Input.GetAxis("Mouse X")) > 0.25f || Mathf.Abs(Input.GetAxis("Mouse Y")) > 0.25f))
        //{
        //    timer += Time.deltaTime;
        //}
        //else
        //{
        //    timer = 0f;
        //}
        //if (timer >= timeBeforeCarCamera || Input.GetButton("Fire4"))
        //{
        //    timer = timeBeforeCarCamera;
        //    shooting = false;
        //    if (wasShooting)
        //        drivingCam.m_XAxis.Value = 0;
        //    drivingCam.m_Priority = highPriority;
        //    shootingCam.m_Priority = lowPriority;
        //}
        //else
        //{
        //    shooting = true;
        //    if (!wasShooting)
        //        shootingCam.m_XAxis.Value = 0;
        //    shootingCam.m_Priority = highPriority;
        //    drivingCam.m_Priority = lowPriority;
        //    // enable reticle here
        //}
        //wasShooting = shooting;

        int ungroundedWheels = 0;
        foreach (RCC_WheelCollider wheelCollider in CarManager.Instance.carController.allWheelColliders)
        {
            if (!wheelCollider.isGrounded)
                ++ungroundedWheels;
        }
        rearView.m_Priority = (ungroundedWheels < 2 && cm.rearView && GameManager.instance.gameState == GameManager.GameStates.play) ? rearViewHigh : rearViewLow;
    }
}
