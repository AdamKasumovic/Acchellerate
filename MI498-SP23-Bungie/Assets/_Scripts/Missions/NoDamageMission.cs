using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NoDamageMission : SingleMission
{
    


    [Header("No Damage Mission Settings")]
    [Tooltip("Whether or not this mission is awesome.")]
    public bool isAwesome = false;
    public float initHealth;

    protected override void Start()
    {
        base.Start();  // don't get rid of this
       initHealth = CarManager.Instance.checkHealth();
        // Initialize anything here as needed
    }

    protected override void Update()
    {
        base.Update();

        string progress = $"";  // update this with the progress, or get rid of it from MissionName if this mission doesn't have progress.
        string timer = UseTimer ? $" Time left: {Mathf.Max(0, timeRemaining):0.0}s" : "";  // this works already, probably leave it alone. Note that it gives the empty string if there's no timer for the mission.


        // You are responsible for ensuring that "MissionName" contains the appopriate text that informs players
        // about the mission progress at all times.
        MissionName = $"Do not take damage! ({progress}).{timer}";
        Debug.Log(MissionName);
    }

    // Place any helper functions here.

    // This function indicates progress in the mission. It might just call "CompleteMission" if there's no progress in the mission (just insta win or lose).
    public override void Execute()
    {
        base.Execute();
        float Health = CarManager.Instance.checkHealth();

        if (initHealth != Health)
        {
            if (initHealth < Health)
            {
                initHealth = Health;
            }
            else
            {
                FailMission();
            }
        }


    }

    // These are fine for now, just replace the Debugs
    public override void FailMission()
    {
        base.FailMission();  // MUST CALL THIS!
        Debug.Log("No Damage Mission Failed!");
    }

    public override void CompleteMission()
    {
        base.CompleteMission();  // MUST CALL THIS!
        Debug.Log("No Damage Mission Completed!");
    }

    // Change this depending on your mission (you can delete it if your mission should result in a fail when time runs out since that's the default)
    protected override MissionResult OnTimerExpired()
    {
        // For this specific mission, when the timer expires, we want to complete/fail the mission.
        return MissionResult.Complete;
    }

    // Great! Now head over to Missions.cs and add an Updater for your mission type and then trigger it from the appropriate script.
}

