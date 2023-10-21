using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NoCrashMission : SingleMission
{
    public int CrashCount { get; private set; }

    [Header("No Crash Mission Settings")]
    [Tooltip("How many Crashs are needed to fail the mission.")]
    public int RequiredCrashsToFail = 1;

    protected override void Start()
    {
        base.Start();  // don't get rid of this
        CrashCount = 0;
    }

    protected override void Update()
    {
        base.Update();

        string progress =$"Don't crash!";
        string timer = UseTimer ? $" Time left: {Mathf.Max(0, timeRemaining):0.0}s" : ""; 

        MissionName = $"{progress}{timer}";
    }

    public override void Execute()
    {
        base.Execute();
        CrashCount++;
        if (CrashCount >= RequiredCrashsToFail)
        {
            FailMission();
        }
    }
    protected override MissionResult OnTimerExpired()
    {
        return MissionResult.Complete;
    }
    public override void FailMission()
    {
        base.FailMission();  // MUST CALL THIS!
        Debug.Log("No Crash Mission Failed!");
    }

    public override void CompleteMission()
    {
        base.CompleteMission();  // MUST CALL THIS!
        Debug.Log("No Crash Mission Completed!");
    }
}
