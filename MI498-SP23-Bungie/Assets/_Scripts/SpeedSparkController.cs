using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpeedSparkController : MonoBehaviour
{
    public float speedToTurnOn = 80;
    public float speedToTurnBlue = 160;
    public float speedToTurnRed = 240;
    public float maximumTime = 0.25f;
    public float minimumTime = 0.05f;
    public TrailRenderer tr;
    private Gradient oldGradient;
    private Gradient gradient;
    private GradientColorKey[] colorKey;
    private GradientAlphaKey[] alphaKey;
    // Start is called before the first frame update
    void Start()
    {
        oldGradient = tr.colorGradient;
        
    }

    // Update is called once per frame
    void Update()
    {
        tr.time = Mathf.Lerp(minimumTime, maximumTime, (CarManager.carSpeed - speedToTurnOn) / (speedToTurnRed - speedToTurnOn));
        if (CarManager.carSpeed < speedToTurnOn)
        {
            tr.enabled = false;
        }
        else if (CarManager.carSpeed < speedToTurnBlue)
        {
            tr.enabled = true;
            gradient = new Gradient();




            colorKey = new GradientColorKey[oldGradient.colorKeys.Length];
            for (int i = 0; i < oldGradient.colorKeys.Length; ++i)
            {
                colorKey[i].color = Color.Lerp(oldGradient.colorKeys[i].color, Color.yellow, (CarManager.carSpeed - speedToTurnOn)/(speedToTurnBlue-speedToTurnOn));
                colorKey[i].time = oldGradient.colorKeys[i].time;
            }


            alphaKey = new GradientAlphaKey[oldGradient.alphaKeys.Length];
            for (int i = 0; i < oldGradient.alphaKeys.Length; ++i)
            {
                alphaKey[i].alpha = oldGradient.alphaKeys[i].alpha;
                alphaKey[i].time = oldGradient.alphaKeys[i].time;
            }

            gradient.SetKeys(colorKey, alphaKey);
            tr.colorGradient = gradient;
        }
        else if (CarManager.carSpeed < speedToTurnRed)
        {
            tr.enabled = true;
            gradient = new Gradient();




            colorKey = new GradientColorKey[oldGradient.colorKeys.Length];
            for (int i = 0; i < oldGradient.colorKeys.Length; ++i)
            {
                colorKey[i].color = Color.Lerp(Color.yellow, Color.red, (CarManager.carSpeed - speedToTurnBlue) / (speedToTurnRed - speedToTurnBlue));
                colorKey[i].time = oldGradient.colorKeys[i].time;
            }


            alphaKey = new GradientAlphaKey[oldGradient.alphaKeys.Length];
            for (int i = 0; i < oldGradient.alphaKeys.Length; ++i)
            {
                alphaKey[i].alpha = oldGradient.alphaKeys[i].alpha;
                alphaKey[i].time = oldGradient.alphaKeys[i].time;
            }

            gradient.SetKeys(colorKey, alphaKey);
            tr.colorGradient = gradient;
        }
        else
        {
            tr.enabled = true;
            gradient = new Gradient();




            colorKey = new GradientColorKey[oldGradient.colorKeys.Length];
            for (int i = 0; i < oldGradient.colorKeys.Length; ++i)
            {
                colorKey[i].color = Color.red;
                colorKey[i].time = oldGradient.colorKeys[i].time;
            }


            alphaKey = new GradientAlphaKey[oldGradient.alphaKeys.Length];
            for (int i = 0; i < oldGradient.alphaKeys.Length; ++i)
            {
                alphaKey[i].alpha = oldGradient.alphaKeys[i].alpha;
                alphaKey[i].time = oldGradient.alphaKeys[i].time;
            }

            gradient.SetKeys(colorKey, alphaKey);
            tr.colorGradient = gradient;
        }
    }
}
