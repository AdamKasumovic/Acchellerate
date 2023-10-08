using Gaia;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class MissionList : MonoBehaviour
{

    Missions missionsInstance;
    public TextMeshProUGUI missionListText;
    
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

        // Loop through each active mission and append its name to the text
        foreach (var mission in missionsInstance.activeMissions)
        {
            missionListText.text += mission.MissionName + "\n\n";
        }
    }
}
