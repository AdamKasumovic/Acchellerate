using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum MoveType  // this has all our move types
{
    flip,
    tilt,
    jump,
    boost,
    burnout,
    any
}

/// <summary>
/// This mission requires the player to simply use a certain number of a certain moves.
/// </summary>
public class UseMovesMission : SingleMission
{

    public int MoveCount { get; private set; }

    [Header("Use Moves Mission Settings")]
    public int RequiredMoves = 3;
    public MoveType missionMoveType = MoveType.tilt;

    protected override void Start()
    {
        base.Start();  // don't get rid of this
        MoveCount = 0;
    }

    protected override void Update()
    {
        base.Update();

        string progress = $"{MoveCount}/{RequiredMoves}";
        string timer = UseTimer ? $" Time left: {Mathf.Max(0, timeRemaining):0.0}s" : "";
        string moveAction = GetMoveAction(missionMoveType);

        MissionName = $"{moveAction} {RequiredMoves} times ({progress}).{timer}";
        Debug.Log(MissionName);
    }

    private string GetMoveAction(MoveType type)
    {
        switch (type)
        {
            case MoveType.flip:
                return "Flip";
            case MoveType.tilt:
                return "Tilt";
            case MoveType.jump:
                return "Jump";
            case MoveType.boost:
                return "Boost";
            case MoveType.burnout:
                return "Burnout";
            default:
                return "Use any move";
        }
    }

    public override void Execute()
    {
        MoveCount++;
        if (MoveCount >= RequiredMoves)
        {
            CompleteMission();
        }
    }

    // These are fine for now, just replace the Debugs
    public override void FailMission()
    {
        base.FailMission();  // MUST CALL THIS!
        Debug.Log("Use Moves Mission Failed!");
    }

    public override void CompleteMission()
    {
        base.CompleteMission();  // MUST CALL THIS!
        Debug.Log("Use Moves Mission Completed!");
    }
}
