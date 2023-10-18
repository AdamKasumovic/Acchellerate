using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlyWithoutStoppingMission : SingleMission
{
    public float Airtime { get; private set; }

    [Header("Fly Without Stopping Mission Settings")]
    public float RequiredTime = 10.0f;

    private CarManager instance;
    private RCC_CarControllerV3 carController;

    protected override void Start()
    {
        base.Start();
        Airtime = 0;
        instance = CarManager.Instance;
        carController = instance.carController;
    }

    protected override void Update()
    {
        base.Update();
        if(!IsCompleted && !IsFailed)
            if (!carController.isGrounded)
                Execute();
            else
                Airtime = 0;

        string progress = $"{Airtime:0.0}s/{RequiredTime:0.0}s";
        string timer = UseTimer ? $" Time left: {Mathf.Max(0, timeRemaining):0.0}s" : "";

        MissionName = $"Fly for {RequiredTime:0.0} total seconds ({progress}).{timer}";
    }

    public override void Execute()
    {
        base.Execute();
        Airtime += Time.deltaTime;
        if (Airtime >= RequiredTime)
        {
            CompleteMission();
        }
    }

    public override void FailMission()
    {
        base.FailMission();  // MUST CALL THIS!
        Debug.Log("Fly Without Stopping Mission Failed!");
    }

    public override void CompleteMission()
    {
        base.CompleteMission();  // MUST CALL THIS!
        Debug.Log("Fly Without Stopping Mission Completed!");
    }
}
