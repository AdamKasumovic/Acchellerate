using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TriggerSplash : MonoBehaviour
{
    ParticleSystem ps;
    int lastIdx = -1;
    public LevelPoints data;
    // Start is called before the first frame update
    void Start()
    {
        ps = GetComponent<ParticleSystem>();
    }

    // Update is called once per frame
    void Update()
    {
        int points = (int)CarManager.numPoints;
        int gradeIdx = data.GetGradeFromPoints(points);
        if (gradeIdx != lastIdx && !(CarManager.Instance.inTutorial && !Tutorial.enteredAreas[5]))
        {
            ps.Stop();
            ps.Play();
            //Debug.Log("PLAY!");
        }
        lastIdx = gradeIdx;
    }
}
