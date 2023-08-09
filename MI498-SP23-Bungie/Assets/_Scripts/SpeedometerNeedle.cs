using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpeedometerNeedle : MonoBehaviour
{
    public RectTransform rectTransform;
    [Tooltip("Rotation of needle when at 0 speed in signed degrees.")]
    public float lowRotation = 142f;
    [Tooltip("Rotation of needle when at max speed in signed degrees.")]
    public float highRotation = -90f;
    [Tooltip("This is useful for when you crash. It determines the maximum speed the needle can move to adjust to the actual speed.")]
    public float maximumSpeed = 1f;
    private float timeBeforeMovement = 0.01f;

    private float percentOfMaxSpeed;

    public float timeBeforeNeedleStarts = 1f;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        percentOfMaxSpeed = NewStyleSystem.instance.timer / (NewStyleSystem.instance.multiplierTime - timeBeforeNeedleStarts);
        //percentOfMaxSpeed = percentOfMaxSpeed == 1 ? 0 : percentOfMaxSpeed;  // fix flashing

        rectTransform.eulerAngles = new Vector3(0,0,Mathf.Lerp(lowRotation, highRotation, percentOfMaxSpeed));  // unclamping should naturally produce the wobbly needle at max speed effect
        //float currentVelocity = 0;
        //float currentZAngle = rectTransform.eulerAngles.z <= 180 ? rectTransform.eulerAngles.z : (rectTransform.eulerAngles.z - 360); // Angles are periodic by 360
        //rectTransform.eulerAngles = new Vector3(0, 0, Mathf.SmoothDamp(currentZAngle, eulerAngles, ref currentVelocity, timeBeforeMovement, maximumSpeed));
    }
}
