using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GradeDisplay : MonoBehaviour
{
    private TextMeshProUGUI gradeLetter;
    public LevelPoints data;
    private Image[] images;
    int lastIdx = -1;
    float timer = 0;
    float maxTime = 1;
    Vector3 maxScale = 3*Vector3.one;
    Vector3 normScale = Vector3.one;

    // Start is called before the first frame update
    void Start()
    {
        images = GetComponentsInChildren<Image>();
        gradeLetter = GetComponent<TextMeshProUGUI>();
        for (int i = 0; i < images.Length; i++)
        {
            images[i].enabled = false;
        }
        
    }

    // Update is called once per frame
    void Update()
    {
        int points = (int)CarManager.numPoints;
        int gradeIdx = data.GetGradeFromPoints(points);
        if(gradeIdx != lastIdx)
        {
            for (int i = 0; i < images.Length; i++)
            {
                images[i].enabled = false;
            }
            images[gradeIdx].enabled = true;
            timer = maxTime;
        }
        if(timer > 0)
        {
            images[gradeIdx].gameObject.transform.localScale = Vector3.Lerp(normScale, maxScale, timer);
        }
        timer -= Time.deltaTime;
        lastIdx = gradeIdx;
        //Grade grade = data.levelPoints[gradeIdx];
        //gradeLetter.text = grade.name[0].ToString();
    }
}
