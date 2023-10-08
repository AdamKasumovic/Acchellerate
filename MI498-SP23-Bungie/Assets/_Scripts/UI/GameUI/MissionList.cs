using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using System.Linq;

public class MissionList : MonoBehaviour
{

    Missions missionsInstance;
    public TextMeshProUGUI missionListText;
    public Color defaultColor = Color.white;
    public Color onProgressColor = Color.yellow;
    
    public void Start()
    {
        missionsInstance = Missions.Instance;
    }
    
    private void Update()
    {
        UpdateMissionList();
    }

    private void UpdateMissionList()
    {
        // Reset the text
        missionListText.text = "";

        // Sort the missions based on UseTimer and timeRemaining, then by mission name
        var sortedMissions = missionsInstance.activeMissions
            .OrderBy(mission => mission.UseTimer ? mission.timeRemaining : float.MaxValue)
            .ThenBy(mission => mission.MissionName)
            .ToArray();

        // Loop through each active mission and append its name to the text
        foreach (var mission in sortedMissions)
        {
            if (mission.ShowOnUI)
            {
                Color currentColor = Color.Lerp(defaultColor, onProgressColor, mission.progressHighlightValue);
                string hexColor = ColorUtility.ToHtmlStringRGB(currentColor);
                missionListText.text += $"<color=#{hexColor}>" + mission.MissionName + "\n\n" + "</color>";
            }
        }
    }
}
