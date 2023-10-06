using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthMeter : MonoBehaviour
{
    public Image healthMeter;
    [Tooltip("Maximum fill number that signifies maximum health.")]
    public float maximumFill = 0.22f;

    [Tooltip("This is useful for when you crash. It determines the maximum speed the health bar can move to adjust to the actual health remaining.")]
    public float maximumSpeed = 1f;
    private float timeBeforeMovement = 0.01f;

    private float percentOfMaxSpeed;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        percentOfMaxSpeed = CarManager.carHealth / CarManager.carMaxHealth;

        //gasMeter.color = Color.Lerp(new Color(1, 0, 0, gasMeter.color.a), new Color(1, 1, 1, gasMeter.color.a), percentOfMaxSpeed);
        float fillAmount = Mathf.Lerp(0, maximumFill, percentOfMaxSpeed);
        float currentVelocity = 0;
        healthMeter.fillAmount = Mathf.SmoothDamp(healthMeter.fillAmount, fillAmount, ref currentVelocity, timeBeforeMovement, maximumSpeed);
    }
}
