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
            string progress = ".";
            string timer = UseTimer ? $" Time left: {Mathf.Max(0, timeRemaining):0.0}s" : "";  // this works already, probably leave it alone. Note that it gives the empty string if there's no timer for the mission.
            if (stayBelow)
            {
                MissionName = $"Do not go above {height} height.";
            }
            else
            {
                MissionName = $"Do not go below {height} height.";
            }
            Debug.Log(MissionName);
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

