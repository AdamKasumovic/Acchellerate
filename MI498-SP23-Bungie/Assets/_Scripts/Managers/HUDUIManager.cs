using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class HUDUIManager : MonoBehaviour
{
    private TextMeshProUGUI healthDisplay;
    private TextMeshProUGUI speedDisplay;
    private TextMeshProUGUI gasDisplay;
    private TextMeshProUGUI pointsDisplay;

    public bool infiniteGas = false;

    // Start is called before the first frame update
    void Start()
    {
        healthDisplay = transform.Find("HealthMeter/HealthDisplay").gameObject.GetComponent<TextMeshProUGUI>();
        if (healthDisplay == null)
        {
            Debug.Log("No Health Display found in scene");
        }
        speedDisplay = transform.Find("SpeedDisplay").gameObject.GetComponent<TextMeshProUGUI>();
        if (speedDisplay == null)
        {
            Debug.Log("No Speed Display found in scene");
        }
        gasDisplay = transform.Find("GasDisplay").gameObject.GetComponent<TextMeshProUGUI>();
        if (gasDisplay == null)
        {
            Debug.Log("No Gas Display found in scene");
        }
        pointsDisplay = transform.Find("PointsDisplay").gameObject.GetComponent<TextMeshProUGUI>();
        if (pointsDisplay == null)
        {
            Debug.Log("No Points Display found in scene");
        }

    }

    // Update is called once per frame
    void Update()
    {
        healthDisplay.text = $"{CarManager.carHealth:0}";
        speedDisplay.text = $"{CarManager.carSpeed:000}";
        if (!infiniteGas)
            gasDisplay.text = $"{LevelTimer.numSeconds:0}";
        else
            gasDisplay.text = "Åá";
        float yVelocity = 0;
        float currentPoint = float.Parse(pointsDisplay.text.Substring(0),System.Globalization.NumberStyles.AllowThousands);  // If you changed the points display and text and got an error, this index is why
        pointsDisplay.text = $"{Mathf.RoundToInt(Mathf.SmoothDamp(currentPoint, CarManager.numPoints, ref yVelocity, 0.001f, Mathf.Max(8000, Mathf.Abs(currentPoint-CarManager.numPoints)*100))):n0}";
    }
}
