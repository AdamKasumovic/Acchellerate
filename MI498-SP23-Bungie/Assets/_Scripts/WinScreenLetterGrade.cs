using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
public class WinScreenLetterGrade : MonoBehaviour
{
    public GameObject letterTextbox;
    public GameObject detailTextbox;
    public GameObject pointsScoredTextbox;
    public GameObject pointsToNextLevelTextbox;
    private TextMeshProUGUI gradeLetter;
    private TextMeshProUGUI gradeDetail;
    private TextMeshProUGUI pointsScored;
    private TextMeshProUGUI pointsToNextLevel;
    public LevelPoints data;
    // Start is called before the first frame update
    void Start()
    {
        gradeLetter = letterTextbox.GetComponent<TextMeshProUGUI>();
        gradeDetail = detailTextbox.GetComponent<TextMeshProUGUI>();
        pointsScored = pointsScoredTextbox.GetComponent<TextMeshProUGUI>();
        pointsToNextLevel = pointsToNextLevelTextbox.GetComponent<TextMeshProUGUI>();
    }

    // Update is called once per frame
    void Update()
    {
        int points = (int)CarManager.numPoints;
        int gradeIdx = data.GetGradeFromPoints(points);
        Grade grade = data.levelPoints[gradeIdx];
        gradeLetter.text = grade.name[0].ToString();
        gradeDetail.text = grade.name[1..];
        pointsScored.text = points.ToString();
        if(gradeIdx != data.levelPoints.Length - 1)
        {
            pointsToNextLevel.text = (data.levelPoints[gradeIdx+1].minPoints - points).ToString();

        }
        else
        {
            pointsToNextLevel.text = "Max Level";
        }
    }
}
