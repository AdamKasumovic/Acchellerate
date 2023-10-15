using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AreaVisitMission : SingleMission
{
    [Header("Area Visit Mission Settings")]
    public GameObject areaTokensParent;

    private List<AreaToken> areaTokens = new List<AreaToken>();

    private int chosenMissionIndex = -1;
    bool missionActivated = false;
    bool areaChosen = false;

    protected override void Start()
    {
        base.Start();
        List<Transform> areaTokenParents = GetAllChilds(areaTokensParent.transform);
        foreach(Transform token in areaTokenParents)
        {
            areaTokens.Add(token.gameObject.GetComponent<AreaToken>());
        }
    }

    protected override void Update()
    {
        base.Update();
        if (IsActive && !missionActivated)
        {
            missionActivated = true;
            TurnOnAreaToken();
        }

        if (!areaChosen)
        {
            ChooseArea();
            areaChosen = true;
        }

        string timer = UseTimer ? $" Time left: {Mathf.Max(0, timeRemaining):0.0}s" : "";

        if (chosenMissionIndex >= 0 && chosenMissionIndex < areaTokens.Count)
            MissionName = $"Visit Area: {areaTokens[chosenMissionIndex].areaName}.{timer}";
        //Debug.Log(MissionName);
    }

    /// <summary>
    /// There is nothing to prevent the same word from being selected if this mission is called more than once at this moment.
    /// </summary>
    private void ChooseArea()
    {
        if(areaTokens.Count == 0)
        {
            Debug.LogWarning("No areas to visit. Add more area tokens or reduce number of areas to visit in inspector for AreaVisitMission.");
            return;
        }

        chosenMissionIndex = Random.Range(0, areaTokens.Count - 1);
    }

    private void TurnOnAreaToken()
    {
        areaTokens[chosenMissionIndex].gameObject.SetActive(true);
    }

    public void AreaTokenCollected(string areaName)
    {
        // If multiple area missions are active at once, this will prevent any conflict
        if(areaName == areaTokens[chosenMissionIndex].areaName)
        {
            chosenMissionIndex = -1;
            CompleteMission();
        }
    }

    public override void FailMission()
    {
        base.FailMission();  // MUST CALL THIS!
        // Turn Off The Area
        areaTokens[chosenMissionIndex].gameObject.SetActive(false);
        Debug.Log("Letter Mission Failed!");
    }

    public override void CompleteMission()
    {
        base.CompleteMission();  // MUST CALL THIS!
        Debug.Log("Letter Mission Completed!");
    }
}
