using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Write a summary here
/// </summary>

public class HeightMission : SingleMission
{
    // "<Replace>" should become the name of your mission type (e.g. "Kill")

    // Consult SingleMission to ensure that you do not do anything redundant here!

    // Place any mission-specific variables needed here

    // Place any mission-specific settings here. Try to make as much as possible a setting to maximize customizability.
    [Header("Height Mission Settings")]
    //True if we want the player to not go above a height, false if we don't want them to go below
    public bool stayBelow = true;
    //Y value of plane
    public int height;
    public GameObject planePrefab;
    private GameObject heightPlane;

    [Tooltip("Whether or not this mission is awesome.")]
    public bool isAwesome = true;

    protected override void Start()
    {
        base.Start();

        heightPlane = Instantiate(planePrefab, new Vector3(CarManager.Instance.transform.position.x,height, CarManager.Instance.transform.position.z), Quaternion.identity);
    }

    protected override void Update()
    {
        base.Update();

        if (IsActive)
        {
            string tintString = (!IsCompleted && !IsFailed && !IsActive) ? " tint=1" : "";
            string timer = UseTimer ? $"<sprite index=0{tintString}>{Mathf.Max(0, timeRemaining):0.0}s" : "";
            string missionSymbol = $"<sprite index=6>";
            string progress = ".";
            if (stayBelow)
            {
                MissionName = $"{SpriteInsideBoxMarkdown} {missionSymbol} Stay below {height} height. {timer}";
            }
            else
            {
                MissionName = $"{SpriteInsideBoxMarkdown} {missionSymbol} Stay above {height} height. {timer}";
            }

            
        }
    }


    public override void Execute()
    {
        base.Execute();
        FailMission();
    }


    public override void FailMission()
    {
        base.FailMission();
        Destroy(heightPlane.gameObject);
        Debug.Log("Height Mission Failed!");
    }

    public override void CompleteMission()
    {
        base.CompleteMission();
        Destroy(heightPlane.gameObject);
        Debug.Log("Height Mission Completed!");
    }


    protected override MissionResult OnTimerExpired()
    {

        return MissionResult.Complete;
    }


}

