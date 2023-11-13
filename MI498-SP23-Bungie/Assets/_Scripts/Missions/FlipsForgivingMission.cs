using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum FlipType
{
    front,
    side,
    any
}
public class FlipsForgivingMission : SingleMission
{
    public int Flips { get; private set; }

    [Header("Flips Forgiving Mission Settings")]
    public FlipType flipType = FlipType.any;
    public int RequiredFlips = 3;

    protected override void Start()
    {
        base.Start();
        Flips = 0;
    }

    protected override void Update()
    {
        base.Update();

        string progress = $"{Flips}/{RequiredFlips}";
        string flipTypeName = GetFlipName(flipType);
        string timer = UseTimer ? $" Time left: {Mathf.Max(0, timeRemaining):0.0}s" : "";

        MissionName = $"Do {RequiredFlips}{flipTypeName}flips in a row. ({progress}).{timer}";
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
    public void AddFlip(FlipType typeOfFlip)
    {
        if(flipType == typeOfFlip || flipType == FlipType.any)
        {
            Flips += 1;
            Execute();
        }
    }
    public override void Execute()
    {
        base.Execute();
        if (Flips >= RequiredFlips)
        {
            CompleteMission();
        }
    }

    public override void FailMission()
    {
        base.FailMission();  // MUST CALL THIS!
        Debug.Log("Flips Forgiving Mission Failed!");
    }

    public override void CompleteMission()
    {
        base.CompleteMission();  // MUST CALL THIS!
        Debug.Log("Flips Forgiving Mission Completed!");
    }
}
