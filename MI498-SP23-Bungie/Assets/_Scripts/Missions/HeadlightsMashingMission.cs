using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeadlightsMashingMission : SingleMission
{
    public int headlightsSitchCounter { get; private set; }

    [Header("Headlights Mashing Mission Settings")]
    public int timesToMashHeadlights = 100;

    protected override void Start()
    {
        base.Start();
        headlightsSitchCounter = 0;
    }

    protected override void Update()
    {
        base.Update();

        string progress = $"Turn headlights on and off {headlightsSitchCounter/2}/{timesToMashHeadlights} times";
        string timer = UseTimer ? $" Time left: {Mathf.Max(0, timeRemaining):0.0}s" : "";

        MissionName = $"{progress}.{timer}";
    }
    public override void Execute()
    {
        base.Execute();

        if (headlightsSitchCounter < timesToMashHeadlights * 2)
            headlightsSitchCounter++;

        if (headlightsSitchCounter >= timesToMashHeadlights*2)
            CompleteMission();
    }

    public override void FailMission()
    {
        base.FailMission();  // MUST CALL THIS!
        Debug.Log("Mash Headlights Mission Failed!");
    }

    public override void CompleteMission()
    {
        base.CompleteMission();  // MUST CALL THIS!
        Debug.Log("Mash Headlights Mission Completed!");
    }
}