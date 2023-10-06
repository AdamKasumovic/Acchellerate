using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SpeedometerOverlay : MonoBehaviour
{
    public Image speedOverlay;
    [Tooltip("Minimum fill number that signifies maximum speed.")]
    public float minimumFill = 0.22f;
    [Tooltip("Maximum fill number that signifies minimum speed.")]
    public float maximumFill = 0.22f;
    [Tooltip("This is useful for when you crash. It determines the maximum speed the overlay can move to adjust to the actual speed.")]
    public float maximumSpeed = 1f;
    private float timeBeforeMovement = 0.01f;
    private Color initialColor;
    private Image image;
    [Tooltip("How long the overlay flashes when you kill a zombie in seconds.")]
    public float flashDuration = 0.25f;
    [Tooltip("Color to flash")]
    public Color colorToFlash = new Color(1, 1, 1, 0.5f);

    private float percentOfMaxSpeed;

    private float percentOfMaxSpeed2;

    public LevelPoints data;

    private RectTransform rt;

    public float topImageLocation;
    public float bottomImageLocation;
    // Start is called before the first frame update
    void Start()
    { 
        image = GetComponent<Image>();
        initialColor = image.color;
        rt = GetComponent<RectTransform>();
    }

    // Update is called once per frame
    void Update()
    {
        if (data != null)
        {
            int gradeIdx = data.GetGradeFromPoints((int)CarManager.numPoints);
            if (gradeIdx != data.levelPoints.Length - 1)
            {
                float points = CarManager.numPoints - data.levelPoints[gradeIdx].minPoints;
                percentOfMaxSpeed = points / (data.levelPoints[gradeIdx + 1].minPoints - data.levelPoints[gradeIdx].minPoints);

            }
            else
            {
                percentOfMaxSpeed = 0;
            }
            float fillAmount = Mathf.Lerp(maximumFill, minimumFill, percentOfMaxSpeed);
            float currentVelocity = 0;
            speedOverlay.fillAmount = Mathf.SmoothDamp(speedOverlay.fillAmount, fillAmount, ref currentVelocity, timeBeforeMovement, maximumSpeed);
            rt.anchoredPosition = new Vector2(rt.anchoredPosition.x, Mathf.Lerp(bottomImageLocation,topImageLocation,Mathf.SmoothDamp(speedOverlay.fillAmount, fillAmount, ref currentVelocity, timeBeforeMovement, maximumSpeed)));
        }
        percentOfMaxSpeed2 = NewStyleSystem.instance.timer / NewStyleSystem.instance.multiplierTime;
        percentOfMaxSpeed2 = percentOfMaxSpeed2 == 1 ? 0 : percentOfMaxSpeed2;  // fix flashing
        image.color = percentOfMaxSpeed2 > (NewStyleSystem.instance.multiplierTime-flashDuration) / NewStyleSystem.instance.multiplierTime ? colorToFlash : initialColor;
    }


}
