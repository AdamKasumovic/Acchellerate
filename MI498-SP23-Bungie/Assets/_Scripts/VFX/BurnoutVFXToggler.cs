using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.VisualScripting;
using UnityEngine.VFX;

public class BurnoutVFXToggler : MonoBehaviour
{
    public VisualEffect burnoutVFX;
    private CarManager cm;
    // Start is called before the first frame update
    void Start()
    {
        cm = CarManager.Instance;
        burnoutVFX.enabled = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (cm.rb.angularVelocity.y > cm.spinSpeed*0.7f)
        {
            burnoutVFX.transform.localEulerAngles = new Vector3(burnoutVFX.transform.localEulerAngles.x, 150, burnoutVFX.transform.localEulerAngles.z);
        }
        else if (cm.rb.angularVelocity.y < -cm.spinSpeed * 0.7f)
        {
            burnoutVFX.transform.localEulerAngles = new Vector3(burnoutVFX.transform.localEulerAngles.x, 210, burnoutVFX.transform.localEulerAngles.z);
        }
        else
        {
            burnoutVFX.transform.localEulerAngles = new Vector3(burnoutVFX.transform.localEulerAngles.x, 180, burnoutVFX.transform.localEulerAngles.z);
        }
        burnoutVFX.SetBool("Loop", cm.isSpinning && cm.carController.isGrounded);
    }
}
