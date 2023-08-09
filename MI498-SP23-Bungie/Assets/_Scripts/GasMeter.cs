using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GasMeter : MonoBehaviour
{
    public Image gasMeter;
    [Tooltip("Maximum fill number that signifies maximum speed.")]
    public float maximumFill = 1f;
    public float fuelPercentageBeforeWarning = 0.15f;
    private float flickerInterval = 0.375f;
    public RectTransform gasText;
    public float maxGasTextHeight = 890;
    public float minGasTextHeight = 470;

    private float percentOfMaxSpeed;
    private float startingGasFillY;
    private float endingGasFillY;
    private bool wasLowFuel = false;
    // Start is called before the first frame update
    void Start()
    {
        startingGasFillY = gasMeter.rectTransform.anchoredPosition.y;
        endingGasFillY = startingGasFillY - (maxGasTextHeight - minGasTextHeight) + 8;
    }

    // Update is called once per frame
    void Update()
    {
        percentOfMaxSpeed = LevelTimer.numSeconds / LevelTimer.initTme;
        if (percentOfMaxSpeed <= fuelPercentageBeforeWarning)
        {
            if (!wasLowFuel)
                SfxManager.instance.PlaySoundAtRandom(SfxManager.SfxCategory.LowFuel);
            float t = Mathf.PingPong(Time.time, flickerInterval) / flickerInterval;
            gasMeter.color = Color.Lerp(Color.red, Color.white, t);
        }
        else
        {
            gasMeter.color = Color.white;
        }
        //gasMeter.color = Color.Lerp(new Color(1, 0, 0, gasMeter.color.a), new Color(1, 1, 1, gasMeter.color.a), percentOfMaxSpeed);
        gasMeter.fillAmount = Mathf.Lerp(0, maximumFill, percentOfMaxSpeed);
        gasMeter.rectTransform.anchoredPosition = new Vector2(gasMeter.rectTransform.anchoredPosition.x, Mathf.Lerp(endingGasFillY, startingGasFillY, percentOfMaxSpeed));
        gasText.anchoredPosition = new Vector2(gasText.anchoredPosition.x, Mathf.Lerp(minGasTextHeight, maxGasTextHeight, percentOfMaxSpeed));
        wasLowFuel = percentOfMaxSpeed <= fuelPercentageBeforeWarning;
    }
}
