using System.Collections;
using System.Collections.Generic;
using UnityEngine;



/// <summary>
/// SingleMission that requires killing a certain number of a certain type of enemy in a certain time, using a single move (which we emulate with a 5 second timer)
/// </summary>
public class SingleMoveKillMission : SingleMission
{
    public int KillCount { get; private set; }

    [Header("Kill with a Single Move Mission Settings")]
    public int RequiredKills = 10;
    public EnemyType missionEnemyType = EnemyType.any;
    public KillType missionKillType = KillType.any;
    private bool timedReset = false;
    private float startTime = 0;

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
       
        MissionName = $"Kill {RequiredKills} {enemyName} with a single {killAction} ({progress}).{timer}";
        //Debug.Log(MissionName);
        if (timedReset && Time.time - startTime > 5)
        {
            timedReset = false;
            KillCount = 0;
        }
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
        if (timedReset == false)
        {
            timedReset = true;
            startTime = Time.time;
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
