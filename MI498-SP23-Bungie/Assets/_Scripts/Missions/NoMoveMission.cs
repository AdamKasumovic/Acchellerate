using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// Mission that fails when player uses too many of a certain move.
/// </summary>
public class NoMoveMission : SingleMission
{
    public int MoveCount { get; private set; }
    
    [Header("No Move Mission Settings")]
    [Tooltip("How many moves are needed to fail the mission.")]
    public int RequiredMoves = 1;
    public MoveType missionMoveType = MoveType.any;

    protected override void Start()
    {
        base.Start();  // don't get rid of this
        MoveCount = 0;
    }

    protected override void Update()
    {
        base.Update();







        string moveAction = GetMoveAction(missionMoveType);
        string tintString = (!IsCompleted && !IsFailed && !IsActive) ? " tint=1" : "";
        string timer = UseTimer ? $"{Mathf.Max(0, timeRemaining):0.0}s" : "";
        string symbol = UseTimer ? $"<sprite index=0{tintString}> " : " ";
        string missionSymbol = $"<sprite index=14>";
        string moveCountString = (RequiredMoves <= 1) ? "at all" : $"{RequiredMoves} times";








        string progress = RequiredMoves > 1 ? $" ({Mathf.Min(MoveCount, RequiredMoves)}/{RequiredMoves})" : "";

        string secondLine = IsActive ? $"\n\n     ({progress}) {timer}" : "";
        MissionName = $"{SpriteInsideBoxMarkdown} {missionSymbol} Do not use {moveAction} {moveCountString}{progress} {symbol}{timer}";


    }

    private string GetMoveAction(MoveType type)
    {
        switch (type)
        {
            case MoveType.flip:
                return "flip";
            case MoveType.tilt:
                return "tilt";
            case MoveType.jump:
                return "jump";
            case MoveType.boost:
                return "boost";
            case MoveType.burnout:
                return "burnout";
            default:
                return "any move";
        }
    }

    public override void Execute()
    {
        base.Execute();
        MoveCount++;
        if (MoveCount >= RequiredMoves)
        {
            FailMission();
        }
    }

    public override void FailMission()
    {
        base.FailMission();  // MUST CALL THIS!
        Debug.Log("No Move Mission Failed!");
    }

    public override void CompleteMission()
    {
        base.CompleteMission();  // MUST CALL THIS!
        Debug.Log("No Move Mission Completed!");
    }

    protected override MissionResult OnTimerExpired()
    {
        // For this specific mission, when the timer expires, we want to complete/fail the mission.
        return MissionResult.Complete;
    }

}
