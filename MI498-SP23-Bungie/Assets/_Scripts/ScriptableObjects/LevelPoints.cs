using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct Grade
{
    public int minPoints;
    public string name;
    public string styleDisplayName;
}
[CreateAssetMenu(fileName = "LevelWinScreenData", menuName = "ScriptableObjects/LevelPoints", order = 1)]
public class LevelPoints : ScriptableObject
{
    [SerializeField]
    public Grade[] levelPoints;
    public int GetGradeFromPoints(int points)
    {
        for(int i = 0; i < levelPoints.Length-1; i++)
        {
            if (levelPoints[i].minPoints <= points && levelPoints[i+1].minPoints > points)
            {
                return i;
            }
        }
        return levelPoints.Length - 1;
    }
}
