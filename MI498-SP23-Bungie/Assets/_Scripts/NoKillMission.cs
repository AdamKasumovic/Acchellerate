using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Place any enums you need here (see below for example)

//public enum KillType  // this has all our kill types
//{
//    frontFlip,
//    tilt,
//    groundPound,
//    strafe,
//    drift,
//    driving,
//    burnout,
//    any
//}


/// <summary>
/// Write a summary here
/// </summary>
public class NoKillMission : SingleMission
{
    public int KillCount { get; private set; }
    // "<Replace>" should become the name of your mission type (e.g. "Kill")

    // Consult SingleMission to ensure that you do not do anything redundant here!

    // Place any mission-specific variables needed here

    // Place any mission-specific settings here. Try to make as much as possible a setting to maximize customizability.
    [Header("No Kill Mission Settings")]
    public int RequiredKills = 1;
    public EnemyType missionEnemyType = EnemyType.any;
    [Tooltip("Whether or not this mission is awesome.")]
    public bool isAwesome = false;

    protected override void Start()
    {
        base.Start();  // don't get rid of this
        KillCount = 0;
        // Initialize anything here as needed
    }

    protected override void Update()
    {
        base.Update();

        string progress = $"{KillCount}/{RequiredKills}";  // update this with the progress, or get rid of it from MissionName if this mission doesn't have progress.
        //string progress = $"{KillCount}/{RequiredKills}";
        string timer = UseTimer ? $" Time left: {Mathf.Max(0, timeRemaining):0.0}s" : "";  // this works already, probably leave it alone. Note that it gives the empty string if there's no timer for the mission.
        string enemyName = GetEnemyName(missionEnemyType);


        // You are responsible for ensuring that "MissionName" contains the appopriate text that informs players
        // about the mission progress at all times.
        MissionName = $"{RequiredKills} {enemyName} ({progress}).{timer}";
        Debug.Log(MissionName);
    }

    // Place any helper functions here.

    private string GetEnemyName(EnemyType type)
    {
        switch (type)
        {
            case EnemyType.regular:
                return "regular zombies";
            case EnemyType.swole:
                return "swole zombies";
            default:
                return "enemies";
        }
    }

    // This function indicates progress in the mission. It might just call "CompleteMission" if there's no progress in the mission (just insta win or lose).
    public override void Execute()
    {
        KillCount++;
        if (KillCount >= RequiredKills)
        {
            FailMission();
        }
    }

    // These are fine for now, just replace the Debugs
    public override void FailMission()
    {
        base.FailMission();  // MUST CALL THIS!
        Debug.Log("No Kill Mission Failed!");
    }

    public override void CompleteMission()
    {
        base.CompleteMission();  // MUST CALL THIS!
        Debug.Log("No Kill Mission Completed!");
    }

    // Change this depending on your mission (you can delete it if your mission should result in a fail when time runs out since that's the default)
    protected override MissionResult OnTimerExpired()
    {
        // For this specific mission, when the timer expires, we want to complete/fail the mission.
        return MissionResult.Complete;
    }

    // Great! Now head over to Missions.cs and add an Updater for your mission type and then trigger it from the appropriate script.
}
