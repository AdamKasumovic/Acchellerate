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
        if (IsActive)
        {
            string tintString = (!IsCompleted && !IsFailed && !IsActive) ? " tint=1" : "";
            string timer = UseTimer ? $"<sprite index=0{tintString}>{Mathf.Max(0, timeRemaining):0.0}s" : "";
            string missionSymbol = $"<sprite index=4>";


            string progress = "";
            if (groundPoundKillCount < RequiredKills)
            {
                progress += "<sprite index=6>";
            }
            if (frontFlipKillCount< RequiredKills)
            {
                progress += "<sprite index=7>";
            }
            if (tiltKillCount< RequiredKills)
            {
                progress += "<sprite index=10>";
            }
            if (burnoutKillCount < RequiredKills)
            {
                progress += "<sprite index=11>";
            }
            if (strafeKillCount < RequiredKills)
            {
                progress += "<sprite index=12>";
            }



            MissionName = $"{SpriteInsideBoxMarkdown} {missionSymbol} Kill {RequiredKills} enemies with {progress}. {timer}";

        }
    }

    public void AddKill(KillType killType)
    {
        if (killType == KillType.frontFlip && frontFlipKillCount < RequiredKills)
        {
            frontFlipKillCount++;
            Execute();
        }
        else if (killType == KillType.tilt && tiltKillCount < RequiredKills)
        {
            tiltKillCount++;
            Execute();
        }
        else if (killType == KillType.groundPound && groundPoundKillCount < RequiredKills)
        {
            groundPoundKillCount++;
            Execute();
        }
        else if (killType == KillType.strafe && strafeKillCount < RequiredKills)
        {
            strafeKillCount++;
            Execute();
        }
        else if (killType == KillType.burnout && burnoutKillCount < RequiredKills)
        {
            burnoutKillCount++;
            Execute();
        }
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
