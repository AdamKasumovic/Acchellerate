using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NoCrashForgivingMission : SingleMission
{
    public int CrashCount { get; private set; }
    public float ForgivingTimer { get; private set; }

    [Header("No Crash Forgiving Mission Settings")]
    public float RequiredTime = 10.0f;
    [Tooltip("How many Crashs are needed to fail the mission.")]
    public int RequiredCrashsToFail = 1;

    protected override void Start()
    {
        base.Start();  // don't get rid of this
        CrashCount = 0;
        ForgivingTimer = 0;
    }

    protected override void Update()
    {
        base.Update();

        if (IsActive)
        {
            ForgivingTimer += Time.deltaTime;
            if (ForgivingTimer >= RequiredTime)
                CompleteMission();
            if (CrashCount >= RequiredCrashsToFail)
            {
                CrashCount = 0;
                ForgivingTimer = 0;
            }
        }


        if (IsActive)
        {
            string tintString = (!IsCompleted && !IsFailed && !IsActive) ? " tint=1" : "";
            string timer = UseTimer ? $"<sprite index=0{tintString}>{Mathf.Max(0, timeRemaining):0.0}s" : "";
            string missionSymbol = $"<sprite index=13>";


            string progress = $"{Mathf.Round(RequiredTime - ForgivingTimer)}s";



            MissionName = $"{SpriteInsideBoxMarkdown} {missionSymbol} Don't crash for ({progress}) {timer}";

        }
    }

    public override void Execute()
    {
        base.Execute();
        CrashCount++;
    }
    protected override MissionResult OnTimerExpired()
    {
        return MissionResult.Complete;
    }
    public override void FailMission()
    {
        base.FailMission();  // MUST CALL THIS!
        Debug.Log("No Crash Forgiving Mission Failed!");
    }

    public override void CompleteMission()
    {
        base.CompleteMission();  // MUST CALL THIS!
        Debug.Log("No Crash Forgiving Mission Completed!");
    }
}