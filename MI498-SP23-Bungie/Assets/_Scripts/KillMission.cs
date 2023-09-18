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
        MissionName = $"Kill {RequiredKills} enemies";
        KillCount = 0;
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
        // Logic or events you want to trigger when the kill mission fails
        // E.g., display a message to the player
        Debug.Log("Kill Mission Failed!");
    }

    public override void CompleteMission()
    {
        IsCompleted = true;
        // Logic or events you want to trigger when the kill mission is completed
        // E.g., display a message to the player
        Debug.Log("Kill Mission Completed!");
    }
}
