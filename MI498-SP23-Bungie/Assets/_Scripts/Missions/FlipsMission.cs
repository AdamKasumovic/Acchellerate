using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.Vehicles.Car;

public class FlipsMission : SingleMission
{
    public int Flips { get; private set; }
    [Header("Flip Mission Settings")]
    public FlipType flipType = FlipType.any;
    public int RequiredFlips = 3;
    private bool wasAirborne = false;
    private CarManager instance;
    private RCC_CarControllerV3 carController;
    protected override void Start()
    {
        base.Start();
        Flips = 0;
        instance = CarManager.Instance;
        carController = instance.carController;
    }

    protected override void Update()
    {
        base.Update();

        if (IsActive && !IsCompleted && !IsFailed)
        {
            if(!carController.isGrounded && !wasAirborne)
            {
                wasAirborne = true;
            }
            if (wasAirborne && carController.isGrounded && Flips > 0)
            {
                wasAirborne= false;
                if (Flips >= RequiredFlips)
                    Execute();
                else
                    FailMission();
            }
        }

        string flipTypeName = GetFlipName(flipType);
        string tintString = (!IsCompleted && !IsFailed && !IsActive) ? " tint=1" : "";
        string timer = UseTimer ? $"<sprite index=2{tintString}>{Mathf.Max(0, timeRemaining):0.0}s" : "";
        string missionSymbol = $"<sprite index=7>";



        string progress = $"{Flips}/{RequiredFlips}";


        MissionName = $"{SpriteInsideBoxMarkdown} {missionSymbol} Do {RequiredFlips}{flipTypeName}flips in a row ({progress}) {timer}";


    }
    private string GetFlipName(FlipType type)
    {
        switch (type)
        {
            case FlipType.front:
                return " front ";
            case FlipType.side:
                return " side ";
            default:
                return " ";
        }
    }
    public void AddFlips(FlipType typeOfFlip, int numflips)
    {
        if (flipType != FlipType.any)
        {
            if (flipType == typeOfFlip)
                Flips += numflips;
        }
        else
            Flips += numflips;
    }
    public override void Execute()
    {
        base.Execute();
        if (Flips >= RequiredFlips)
            CompleteMission();
        else
            FailMission();
    }

    public override void FailMission()
    {
        base.FailMission();  // MUST CALL THIS!
        Debug.Log("Flip Mission Failed!");
    }

    public override void CompleteMission()
    {
        base.CompleteMission();  // MUST CALL THIS!
        Debug.Log("Flip Mission Completed!");
    }
}
