using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyDestructibleObjectsMission : SingleMission
{
    public int ObjectDestroysCount { get; private set; }

    [Header("Destroy Objects Mission Settings")]
    public int RequiredObjectDestroys = 17;

    protected override void Start()
    {
        base.Start();
        ObjectDestroysCount = 0;
    }

    protected override void Update()
    {
        base.Update();

       
        if (IsActive)
        {
            string tintString = (!IsCompleted && !IsFailed && !IsActive) ? " tint=1" : "";
            string timer = UseTimer ? $"<sprite index=2{tintString}>{Mathf.Max(0, timeRemaining):0.0}s" : "";
            string missionSymbol = $"<sprite index=13>";







            string progress = $"Destroy {RequiredObjectDestroys} Objects({ObjectDestroysCount}/{RequiredObjectDestroys}) ";


            MissionName = $"{SpriteInsideBoxMarkdown} {missionSymbol} {progress}{timer}";

        }
        //Debug.Log(MissionName);
    }

    public override void Execute()
    {
        base.Execute();
        ObjectDestroysCount++;
        if (ObjectDestroysCount >= RequiredObjectDestroys)
        {
            CompleteMission();
        }
    }

    public override void FailMission()
    {
        base.FailMission();  // MUST CALL THIS!
    }

    public override void CompleteMission()
    {
        base.CompleteMission();  // MUST CALL THIS!
    }

}
