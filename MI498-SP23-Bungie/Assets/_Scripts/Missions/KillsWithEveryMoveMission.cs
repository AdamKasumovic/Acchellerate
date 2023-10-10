using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KillsWithEveryMoveMission : SingleMission
{
    public int frontFlipKillCount { get; private set; }
    public int tiltKillCount { get; private set; }
    public int groundPoundKillCount { get; private set; }
    public int strafeKillCount { get; private set; }
    public int burnoutKillCount { get; private set; }


    [Header("Kills With Every Move Mission Settings")]
    public int RequiredKills = 10;

    protected override void Start()
    {
        base.Start();
        frontFlipKillCount = 0;
        tiltKillCount = 0;
        groundPoundKillCount = 0;
        strafeKillCount = 0;
        burnoutKillCount = 0;
    }

    protected override void Update()
    {
        base.Update();

        string progress = $"Kill Count: (Front Flip {frontFlipKillCount}, Ground Pound {groundPoundKillCount}, Tilt {tiltKillCount}," +
            $" Strafe {strafeKillCount}, Burnout {burnoutKillCount}) /{RequiredKills}";
        string timer = UseTimer ? $" Time left: {Mathf.Max(0, timeRemaining):0.0}s" : "";

        MissionName = $"({progress}).{timer}";
            Debug.Log(MissionName);
    }



    public void AddKill(KillType killType)
    {
        if (killType == KillType.frontFlip)
            frontFlipKillCount++;
        else if (killType == KillType.tilt)
            tiltKillCount++;
        else if (killType == KillType.groundPound)
            groundPoundKillCount++;
        else if (killType == KillType.strafe)
            strafeKillCount++;
        else if (killType == KillType.burnout)
            burnoutKillCount++;
        Execute();
    }

    public override void Execute()
    {
        base.Execute();

        if (frontFlipKillCount >= RequiredKills && tiltKillCount >= RequiredKills && groundPoundKillCount >= RequiredKills &&
            strafeKillCount >= RequiredKills && burnoutKillCount >= RequiredKills)
        {
            CompleteMission();
        }
    }

    public override void FailMission()
    {
        base.FailMission();  // MUST CALL THIS!
        Debug.Log("Kill Mission Failed!");
    }

    public override void CompleteMission()
    {
        base.CompleteMission();  // MUST CALL THIS!
        Debug.Log("Kill Mission Completed!");
    }
}
