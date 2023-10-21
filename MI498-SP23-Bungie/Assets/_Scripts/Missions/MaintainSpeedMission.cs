using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MaintainSpeedMission : SingleMission
{
    public int CurrentSpeed { get; private set; }

    [Header("Kill Mission Settings")]
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
        string progress = $"Drive above {RequiredMinSpeed} mph for";
        string timer = UseTimer ? $" {Mathf.Max(0, timeRemaining):0.0}s" : "";
        string currentSpeed = $"Your speed is {CurrentSpeed}";

        MissionName = $"{progress}{timer}.{currentSpeed}";
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
        Debug.Log("Kill Mission Failed!");
    }

    public override void CompleteMission()
    {
        base.CompleteMission();  // MUST CALL THIS!
        Debug.Log("Kill Mission Completed!");
    }
}
