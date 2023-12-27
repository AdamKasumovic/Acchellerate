using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class MaintainSpeedForgivingMission : SingleMission
{
    public int CurrentSpeed { get; private set; }
    public float ForgivingTimer { get; private set; }

    [Header("Maintain Speed Forgiving Mission Settings")]
    public float RequiredTime = 10.0f;
    public int RequiredMinSpeed = 10;

    protected override void Start()
    {
        base.Start();
        CurrentSpeed = (int)CarManager.carSpeed;
        ForgivingTimer = 0;
    }

    protected override void Update()
    {
        base.Update();
        CurrentSpeed = (int)CarManager.carSpeed;
        if(IsActive)
        {
            ForgivingTimer += Time.deltaTime;
            if (CurrentSpeed < RequiredMinSpeed)
            {
                ForgivingTimer = 0;
            }
            if (ForgivingTimer >= RequiredTime)
            {
                CompleteMission();
            }
        }


        string tintString = (!IsCompleted && !IsFailed && !IsActive) ? " tint=1" : "";
        string timer = UseTimer ? $"<sprite index=0{tintString}>{Mathf.Max(0, timeRemaining):0.0}s" : "";
        string missionSymbol = $"<sprite index=9>";


        string progress = $"current: {CurrentSpeed} mph";



        MissionName = $"{SpriteInsideBoxMarkdown} {missionSymbol} Drive above {RequiredMinSpeed} mph for {Mathf.Round(RequiredTime - ForgivingTimer)}s ({progress})";

        
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
        Debug.Log("Maintain Speed Forgiving Mission Failed!");
    }

    public override void CompleteMission()
    {
        base.CompleteMission();  // MUST CALL THIS!
        Debug.Log("Maintain Speed Forgiving Mission Completed!");
    }
}