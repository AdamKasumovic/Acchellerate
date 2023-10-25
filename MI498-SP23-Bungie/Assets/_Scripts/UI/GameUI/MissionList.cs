using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using System.Linq;
using UnityEngine.UI;

public class MissionList : MonoBehaviour
{

    Missions missionsInstance;
    public TextMeshProUGUI missionListText;
    public Color defaultColor = Color.white;
    public Color onProgressColor = Color.yellow;
    public Color onFailColor = Color.red;
    public Color onCompleteColor = Color.green;
    public Color onQueueColor = Color.white;
    public RectTransform backgroundRect;
    public float padding = 10f; // Adjust this value for padding
    public float verticalOffset = 5f; // Adjust this value to move the box up/down
    public float heightAdjustment = 5f;


    private void Start()
    {
        missionsInstance = Missions.Instance;
    }
    
    private void Update()
    {
        UpdateMissionList();
        UpdateBackground();
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

        // Loop through each failed mission and append its name to the text
        foreach (var mission in missionsInstance.failedMissions)
        {
            if (mission.ShowOnUI && mission.failHighlightValue > 0)
            {
                string hexColor = ColorUtility.ToHtmlStringRGB(onFailColor);
                missionListText.text += $"<color=#{hexColor}{GetPartialOpacity(mission.failHighlightValue)}>" + mission.MissionName + "</color>";
                missionListText.text += "\n\n";
            }
        }

        // Loop through each completed mission and append its name to the text
        foreach (var mission in missionsInstance.completedMissions)
        {
            if (mission.ShowOnUI && mission.completeHighlightValue > 0)
            {
                string hexColor = ColorUtility.ToHtmlStringRGB(onCompleteColor);
                missionListText.text += $"<color=#{hexColor}{GetPartialOpacity(mission.completeHighlightValue)}>" + mission.MissionName + "</color>";
                missionListText.text += "\n\n";
            }
        }

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

        if (missionsInstance.queuedMissions.Count > 0)
        {
            string hexColor = ColorUtility.ToHtmlStringRGBA(onQueueColor);
            missionListText.text += $"<color=#{hexColor}>Up Next:</color>\n";
        }

        // Loop through each queued mission and append its name to the text
        foreach (var mission in missionsInstance.queuedMissions)
        {
            if (mission.ShowOnUI)
            {
                string hexColor = ColorUtility.ToHtmlStringRGBA(onQueueColor);
                missionListText.text += $"<color=#{hexColor}>" + mission.MissionName + "\n\n" + "</color>";
            }
        }
    }

    public string GetPartialOpacity(float opacity)
    {
        return Mathf.Clamp(Mathf.RoundToInt(opacity * 255), 0, 255).ToString("X2");
    }

    private void UpdateBackground()
    {
        if (missionListText != null && backgroundRect != null)
        {
            // Get the height of the text
            float textHeight = missionListText.preferredHeight;

            // Adjust for the scale of the RectTransform
            textHeight *= backgroundRect.localScale.y;

            // Set the height of the background rect
            backgroundRect.sizeDelta = new Vector2(backgroundRect.sizeDelta.x, textHeight / backgroundRect.localScale.y + padding + heightAdjustment);

            // Adjust the position to align with the top left
            backgroundRect.anchoredPosition = new Vector2(backgroundRect.anchoredPosition.x, -textHeight / 2 - padding / 2 + verticalOffset);
        }
    }
}
