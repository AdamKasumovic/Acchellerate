using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VibrateUI : MonoBehaviour
{
    public float speed = 0f; // Controls the speed of the vibration
    public float amplitude = 1f;
    public float timeBeforeNeedleStarts = 5f;

    private RectTransform rectTransform;
    private Vector3 originalPosition;

    private void Start()
    {
        // Get the RectTransform component and store its original position
        rectTransform = GetComponent<RectTransform>();
        originalPosition = rectTransform.localPosition;
    }

    private void Update()
    {
        float percent = (NewStyleSystem.instance.timer-1) / (NewStyleSystem.instance.multiplierTime - timeBeforeNeedleStarts);
        percent = (NewStyleSystem.instance.timer == 0) ? 1 : percent;
        // Calculate the amount to offset the RectTransform position using a random vector
        Vector3 offset = Mathf.Lerp(speed,0, percent) * Time.deltaTime * Random.insideUnitCircle;

        // Apply the offset to the RectTransform position
        rectTransform.localPosition = originalPosition + offset * Mathf.Lerp(amplitude,0, percent);
    }
}
