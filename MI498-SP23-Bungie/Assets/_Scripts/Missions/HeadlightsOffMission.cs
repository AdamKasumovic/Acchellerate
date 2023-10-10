using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeadlightsOffMission : SingleMission
{
    public bool wereHeadlightsTurnedOff { get; private set; }

    protected override void Start()
    {
        base.Start();
        wereHeadlightsTurnedOff = false;
    }

    protected override void Update()
    {
        base.Update();

        string progress = $"Figure out how to turn headlights off";
        string timer = UseTimer ? $" Time left: {Mathf.Max(0, timeRemaining):0.0}s" : "";

        MissionName = $"{progress}.{timer}";
    }

    public override void Execute()
    {
        base.Execute();
        wereHeadlightsTurnedOff = true;
        if(wereHeadlightsTurnedOff)
        {
            CompleteMission();
        }
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
