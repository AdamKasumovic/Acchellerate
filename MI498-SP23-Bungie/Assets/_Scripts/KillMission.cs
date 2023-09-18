using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Update this with new enemies if needed (or reorganize if enemy types used in multiple spots)
public enum EnemyType
{
    regular,
    swole,
    balloon,
    any
}

public enum KillType
{
    frontFlip,
    tilt,
    groundPound,
    strafe,
    drift,
    driving,
    burnout,
    any
}

/// <summary>
/// SingleMission that requires killing a certain number of a certain type of enemy in a certain time, optionally using a certain kill type.
/// </summary>
public class KillMission : SingleMission
{
    public int KillCount { get; private set; }

    [Header("Kill Mission Settings")]
    public int RequiredKills = 10;
    public EnemyType missionEnemyType = EnemyType.any;
    public KillType missionKillType = KillType.any;

    protected override void Start()
    {
        base.Start();
        KillCount = 0;
    }

    protected override void Update()
    {
        base.Update();

        string progress = $"{KillCount}/{RequiredKills}";
        string timer = UseTimer ? $" Time left: {Mathf.Max(0, timeRemaining):0.0}s" : "";
        string enemyName = GetEnemyName(missionEnemyType);

        MissionName = $"Kill {RequiredKills} {enemyName} ({progress}).{timer}";
    }

    // Be sure to update this if new enemies are added
    private string GetEnemyName(EnemyType type)
    {
        switch (type)
        {
            case EnemyType.regular:
                return "regular zombies";
            case EnemyType.swole:
                return "swole zombies";
            case EnemyType.balloon:
                return "balloon zombies";
            default:
                return "enemies";
        }
    }

    public override void Execute()
    {
        KillCount++;
        if (KillCount >= RequiredKills)
        {
            CompleteMission();
        }
    }

    public override void FailMission()
    {
        IsFailed = true;
        Debug.Log("Kill Mission Failed!");
    }

    public override void CompleteMission()
    {
        IsCompleted = true;
        Debug.Log("Kill Mission Completed!");
    }
}