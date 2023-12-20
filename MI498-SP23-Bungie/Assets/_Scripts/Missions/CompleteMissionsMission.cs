using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CompleteMissionsMission : SingleMission
{
    public int MissionCount { get; private set; }

    [Header("Complete Missions Mission Settings")]
    public int RequiredMissions = 1;

    protected override void Start()
    {
        base.Start();
        MissionCount = 0;
    }

    protected override void Update()
    {
        base.Update();

        if (IsActive)
        {
            string tintString = (!IsCompleted && !IsFailed && !IsActive) ? " tint=1" : "";
            string timer = UseTimer ? $"<sprite index=2{tintString}> Time left: {Mathf.Max(0, timeRemaining):0.0}s" : "";
            string missionSymbol = $"<sprite index=3>";







            string progress = $"Complete Missions ({MissionCount}/{RequiredMissions})";


            MissionName = $"{SpriteInsideBoxMarkdown} {missionSymbol} {progress}{timer}";

        }
    }

    public override void Execute()
    {
        base.Execute();
        MissionCount++;
        if (MissionCount >= RequiredMissions && IsActive)
        {
            CompleteMission();
        }
    }

    public override void FailMission()
    {
        base.FailMission();  // MUST CALL THIS!
        Debug.Log("Complete Missions Mission Failed!");
    }

    public override void CompleteMission()
    {
        base.CompleteMission();  // MUST CALL THIS!
        Debug.Log("Complete Missions Mission Completed!");
    }
}
