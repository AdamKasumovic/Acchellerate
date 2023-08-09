using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParallaxUI : MonoBehaviour
{
    public float distanceBeforeReset = 10f;
    public float speed = 10f;
    private Vector2 originalPosition;
    private RectTransform rt;
    public bool scaleWithRank = false;
    private float speedMultiplier = 1;
    public LevelPoints data;
    // Start is called before the first frame update
    void Start()
    {
        rt = GetComponent<RectTransform>();
        originalPosition = rt.anchoredPosition;
    }

    // Update is called once per frame
    void Update()
    {
        if (scaleWithRank && data!=null)
        {
            float gradeIdx = data.GetGradeFromPoints((int)CarManager.numPoints);
            if (gradeIdx < 3)
            {
                speedMultiplier = (gradeIdx + 1) / 5;
            }
            else
            {
                speedMultiplier = 1;
            }
        }
        rt.anchoredPosition -= Vector2.right * speed * speedMultiplier * Time.deltaTime;
        if (Mathf.Abs(rt.anchoredPosition.x - originalPosition.x) > distanceBeforeReset)
        {
            rt.anchoredPosition = new Vector2(originalPosition.x, rt.anchoredPosition.y);
        }
    }
}
