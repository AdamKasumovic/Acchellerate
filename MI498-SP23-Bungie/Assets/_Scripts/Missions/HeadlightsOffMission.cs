using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeadlightsOffMission : SingleMission
{
    protected override void Update()
    {
        base.Update();



        string tintString = (!IsCompleted && !IsFailed && !IsActive) ? " tint=1" : "";
        string timer = UseTimer ? $"<sprite index=0{tintString}>{Mathf.Max(0, timeRemaining):0.0}s" : "";
        string missionSymbol = $"<sprite index=15>";




        MissionName = $"{SpriteInsideBoxMarkdown} {missionSymbol} Turn off Headlights {timer}";

        
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
