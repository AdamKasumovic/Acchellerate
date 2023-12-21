using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// Mission that fails when player kills too many of a zombie of a certain type.
/// </summary>
public class NoKillMission : SingleMission
{
    public int KillCount { get; private set; }
    
    [Header("No Kill Mission Settings")]
    [Tooltip("How many zombies are needed to fail the mission.")]
    public int RequiredKills = 1;
    public EnemyType missionEnemyType = EnemyType.any;

    protected override void Start()
    {
        base.Start();  // don't get rid of this
        KillCount = 0;
    }

    protected override void Update()
    {
        base.Update();




        // You are responsible for ensuring that "MissionName" contains the appopriate text that informs players
        // about the mission progress at all times.
        if (IsActive)
        {
            string tintString = (!IsCompleted && !IsFailed && !IsActive) ? " tint=1" : "";
            string timer = UseTimer ? $"<sprite index=0{tintString}>{Mathf.Max(0, timeRemaining):0.0}s" : "";
            string missionSymbol = $"<sprite index=14>";
            string killCountString = (RequiredKills <= 1) ? "any" : $"{RequiredKills}";
            string enemyName = GetEnemyName(missionEnemyType);
            string progress = RequiredKills > 1 ? $" ({Mathf.Min(KillCount, RequiredKills - 1)}/{RequiredKills - 1})." : ".";

            MissionName = $"{SpriteInsideBoxMarkdown} {missionSymbol} Don't kill {killCountString} {enemyName} {progress} {timer}";

        }
    }

    private string GetEnemyName(EnemyType type)
    {
        switch (type)
        {
            case EnemyType.regular:
                return "regular zombies";
            case EnemyType.swole:
                return "swole zombies";
            default:
                return "enemies";
        }
    }

    public override void Execute()
    {
        base.Execute();
        KillCount++;
        if (KillCount >= RequiredKills)
        {
            FailMission();
        }
    }

    public override void FailMission()
    {
        base.FailMission();  // MUST CALL THIS!
        Debug.Log("No Kill Mission Failed!");
    }

    public override void CompleteMission()
    {
        base.CompleteMission();  // MUST CALL THIS!
        Debug.Log("No Kill Mission Completed!");
    }

    protected override MissionResult OnTimerExpired()
    {
        // For this specific mission, when the timer expires, we want to complete/fail the mission.
        return MissionResult.Complete;
    }

}
