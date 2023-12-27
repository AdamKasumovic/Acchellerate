using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MaintainSpeedMission : SingleMission
{
    public int CurrentSpeed { get; private set; }

    [Header("Maintain Speed Mission Settings")]
    public int RequiredMinSpeed = 10;

    protected override void Start()
    {
        base.Start();
        CurrentSpeed = (int)CarManager.carSpeed;

    }

    protected override void Update()
    {
        base.Update();
        CurrentSpeed = (int)CarManager.carSpeed;
        if (IsActive) 
        {
            if(CurrentSpeed < RequiredMinSpeed) 
            {
                FailMission();
            }            
        }
        string tintString = (!IsCompleted && !IsFailed && !IsActive) ? " tint=1" : "";
        string timer = UseTimer ? $"<sprite index=0{tintString}>{Mathf.Max(0, timeRemaining):0.0}s" : "";
        string missionSymbol = $"<sprite index=9>";


        string progress = $"current: {CurrentSpeed}mph";
        MissionName = $"{SpriteInsideBoxMarkdown} {missionSymbol} Stay above {RequiredMinSpeed}mph({progress}) {timer}";

        //Debug.Log(MissionName);
    }

    protected override MissionResult OnTimerExpired()
    {
        return MissionResult.Complete;
    }

    public override void Execute()
    {
        base.Execute();
        CompleteMission();
    }

    public override void FailMission()
    {
        base.FailMission();  // MUST CALL THIS!
        Debug.Log("Maintain Speed Mission Failed!");
    }

    public override void CompleteMission()
    {
        base.CompleteMission();  // MUST CALL THIS!
        Debug.Log("Maintain Speed Mission Completed!");
    }
}
