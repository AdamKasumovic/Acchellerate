using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class LevelTimer : MonoBehaviour
{
    public float changableTime = 180;
    public static float initTme = 180;
    public static float numSeconds = 180;
    private TextMeshProUGUI timeDisplay;
    public float timeWarning = 90f;
    public float timeVeryLow = 15f;
    bool warningTriggered = false;
    bool warningFade = false;
    bool veryLowTriggered = false;
    bool veryLowFade = false;
    bool comboTriggered = false;
    bool comboFade = false;
    float timer = -1;
    float prevComboTimer = 0f;
    // Start is called before the first frame update
    void Start()
    {

        initTme = changableTime;
        numSeconds = initTme;
        timeDisplay = GetComponent<TextMeshProUGUI>();
    }

    // Update is called once per frame
    void Update()
    { 
        numSeconds -= Time.deltaTime;
        if (CarManager.Instance.inTutorial && numSeconds < 1)
        {
            numSeconds = 1;
        }
        //timeDisplay.text = $"{((int)numSeconds)/60}:{((int)numSeconds)%60:00}";
        timeDisplay.text = "";
        if (numSeconds < 0)
        {
            GameManager.instance.HandleWin();

        }
        if(Mathf.Abs(numSeconds - timeWarning) < .1 && !warningTriggered)
        {
            warningTriggered = true;
            StartCoroutine(Flasher(0));
            timer = 1;
        }
        if (warningFade)
        {
            Array.ForEach(transform.GetChild(0).gameObject.GetComponentsInChildren<Image>(), x => x.color = new Color(x.color.r, x.color.g, x.color.b, timer));
            timer -= Time.deltaTime;
            if (timer < 0)
            {
                warningTriggered = false;
                transform.GetChild(0).gameObject.SetActive(false);
            }
        }
        if(Mathf.Abs(numSeconds - timeVeryLow) < .1 && !veryLowTriggered)
        {
            veryLowTriggered = true;
            StartCoroutine(Flasher(1));
            timer = 1;
        }
        if (veryLowFade)
        {
            Array.ForEach(transform.GetChild(1).gameObject.GetComponentsInChildren<Image>(), x => x.color = new Color(x.color.r, x.color.g, x.color.b, timer));
            timer -= Time.deltaTime;
            if (timer < 0)
            {
                veryLowTriggered = false;
                transform.GetChild(1).gameObject.SetActive(false);
            }
        }

        if (!veryLowTriggered && !warningTriggered && !comboTriggered && NewStyleSystem.instance.timer <= 0 && prevComboTimer > 0 && !CarManager.Instance.inTutorial)
        {
            comboTriggered = true;
            comboFade = false;
            Array.ForEach(transform.GetChild(2).gameObject.GetComponentsInChildren<RawImage>(), x => x.color = new Color(x.color.r, x.color.g, x.color.b, 1));
            StartCoroutine(Flasher(2,0));
            timer = 1;
        }

        if (comboFade)
        {
            Array.ForEach(transform.GetChild(2).gameObject.GetComponentsInChildren<RawImage>(), x => x.color = new Color(x.color.r, x.color.g, x.color.b, timer));
            timer -= Time.deltaTime;
            if (timer < 0)
            {
                comboTriggered = false;
                transform.GetChild(2).gameObject.SetActive(false);
            }
        }

        prevComboTimer = NewStyleSystem.instance.timer;
    }
    IEnumerator Flasher(int child, int numFlashes=2)
    {
        for (int i = 0; i < numFlashes; i++)
        {
            transform.GetChild(child).gameObject.SetActive(true);
            yield return new WaitForSeconds(0.5f);
            transform.GetChild(child).gameObject.SetActive(false);
            yield return new WaitForSeconds(0.5f);
        }
        transform.GetChild(child).gameObject.SetActive(true);
        if (child == 0)
        {
            warningFade = true;
        }
        else if (child == 1)
        {
            veryLowFade = true;
        }
        else if (child == 2)
        {
            comboFade = true;
        }
    }
}

