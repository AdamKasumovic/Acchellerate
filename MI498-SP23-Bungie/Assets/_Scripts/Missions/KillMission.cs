using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Update this with new enemies if needed (or reorganize if enemy types used in multiple spots)
public enum EnemyType
{
    regular,
    swole,
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
        string killAction = GetKillAction(missionKillType);

        MissionName = $"{killAction} {RequiredKills} {enemyName} ({progress}).{timer}";
        //Debug.Log(MissionName);
    }

    // Update this if new enemies added
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

    // Update this if new kill types added
    private string GetKillAction(KillType type)
    {
        switch (type)
        {
            case KillType.frontFlip:
                return "Flip kill";
            case KillType.tilt:
                return "Tilt kill";
            case KillType.groundPound:
                return "Ground pound";
            case KillType.strafe:
                return "Strafe kill";
            case KillType.drift:
                return "Drift kill";
            case KillType.driving:
                return "Drive kill";
            case KillType.burnout:
                return "Burnout kill";
            default:
                return "Kill";
        }
    }

    public override void Execute()
    {
        base.Execute();
        KillCount++;
        if (KillCount >= RequiredKills)
        {
            CompleteMission();
        }
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
