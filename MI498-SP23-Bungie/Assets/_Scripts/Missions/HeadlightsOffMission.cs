using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeadlightsOffMission : SingleMission
{
    protected override void Update()
    {
        base.Update();

        string progress = $"Figure out how to turn headlights on/off";
        string timer = UseTimer ? $" Time left: {Mathf.Max(0, timeRemaining):0.0}s" : "";

        MissionName = $"{progress}.{timer}";
    }

    public override void Execute()
    {
        base.Execute();
        CompleteMission();
    }

    public override void FailMission()
    {
        base.FailMission();  // MUST CALL THIS!
        Debug.Log("Turn off Headlights Mission Failed!");
    }

    public override void CompleteMission()
    {
        base.CompleteMission();  // MUST CALL THIS!
        Debug.Log("Turn off Headlights Mission Completed!");
    }
}
