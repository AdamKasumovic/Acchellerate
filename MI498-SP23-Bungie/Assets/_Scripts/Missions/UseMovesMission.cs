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

        if (IsActive)
        {
           
            string moveAction = GetMoveAction(missionMoveType);
            string tintString = (!IsCompleted && !IsFailed && !IsActive) ? " tint=1" : "";
            string timer = UseTimer ? $"{Mathf.Max(0, timeRemaining):0.0}s" : "";
            string symbol = UseTimer ? $"<sprite index=2{tintString}> " : " ";
            







            string progress = $"{MoveCount}/{RequiredMoves}";

            string secondLine = IsActive ? $"\n\n     ({progress}) {timer}" : "";
            MissionName = $"<sprite index=3{tintString}> {SpriteInsideBoxMarkdown}{symbol} {moveAction} {RequiredMoves} times ({progress}). {timer}";

        }
        //Debug.Log(MissionName);
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
        base.Execute();
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
