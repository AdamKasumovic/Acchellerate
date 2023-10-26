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
        string progress = $"Drive above {RequiredMinSpeed} mph for {Mathf.Round(RequiredTime - ForgivingTimer)}";
        string timer = UseTimer ? $" Time left: {Mathf.Max(0, timeRemaining):0.0}s" : "";
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
        Debug.Log("Maintain Speed Forgiving Mission Failed!");
    }

    public override void CompleteMission()
    {
        base.CompleteMission();  // MUST CALL THIS!
        Debug.Log("Maintain Speed Forgiving Mission Completed!");
    }
}