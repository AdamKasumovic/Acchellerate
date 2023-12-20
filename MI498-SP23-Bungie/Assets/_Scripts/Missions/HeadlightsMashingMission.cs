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

        if (IsActive)
        {
            string tintString = (!IsCompleted && !IsFailed && !IsActive) ? " tint=1" : "";
            string timer = UseTimer ? $"<sprite index=0{tintString}>{Mathf.Max(0, timeRemaining):0.0}s" : "";
            string missionSymbol = $"<sprite index=15>";


            string progress = $"{headlightsSitchCounter / 2}/{timesToMashHeadlights}";



            MissionName = $"{SpriteInsideBoxMarkdown} {missionSymbol} Flicker Headlights ({progress}).{timer}";

        }
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