using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Abstract mission class representing a single mission in the level.
/// </summary>
public abstract class SingleMission : MonoBehaviour
{
    public string MissionName { get; set; }  // active mission MissionNames should be used for UI
    public bool IsCompleted { get; protected set; }
    public bool IsFailed { get; protected set; }

    [Header("Timer Settings")]
    public bool UseTimer = false;
    public float MissionDuration = 0;  // Duration in seconds

    private float timeRemaining;

    protected virtual void Start()
    {
        if (UseTimer)
        {
            timeRemaining = MissionDuration;
        }
    }

    protected virtual void Update()
    {
        if (UseTimer && !IsCompleted && !IsFailed)
        {
            timeRemaining -= Time.deltaTime;
            if (timeRemaining <= 0)
            {
                FailMission();
            }
        }
    }

    public abstract void Execute();
    public abstract void FailMission();
    public abstract void CompleteMission();
}
