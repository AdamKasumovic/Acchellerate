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

    protected float timeRemaining;

    public bool IsActive { get; set; } = false;

    [Header("Rewards")]
    public bool DoPointMultiplication = false;
    [Range(1, 100)]
    public float PointMultiplier = 2.0f;
    [Range(0, 1000000)]
    public float PointMultiplierDuration = 30f;
    public bool AddPoints = false;
    [Range(0, 1000000000000)]
    public float PointsToAdd = 100000f;
    public bool AddTokens = true;
    [Range(0, 100)]
    public int TokensToAdd = 1;
    public bool DoSpeedBuff = false;
    [Range(0, 1000)]
    public float IncreaseToMaxSpeedInMPH = 60f;
    [Range(0, 1000000)]
    public float SpeedIncreaseDuration = 30f;

    private Coroutine pointMultiplierCoroutine;
    private bool pointMultiplierActive = false;

    private Coroutine speedBuffCoroutine;
    private bool speedBuffActive = false;

    protected virtual void Start()
    {
        if (UseTimer)
        {
            timeRemaining = MissionDuration;
        }
    }

    protected virtual void Update()
    {
        if (UseTimer && !IsCompleted && !IsFailed && IsActive)
        {
            timeRemaining -= Time.deltaTime;
            if (timeRemaining <= 0)
            {
                FailMission();
            }
        }
    }

    public abstract void Execute();
    public virtual void FailMission()
    {
        IsActive = false;
        IsFailed = true;
        // Remove completed or failed missions
        Missions.Instance.activeMissions.RemoveAll(mission => mission.IsCompleted || mission.IsFailed);
    }
    public virtual void CompleteMission()
    {
        IsActive = false;
        IsCompleted = true;
        
        HandleRewards();
        
        // Remove completed or failed missions
        Missions.Instance.activeMissions.RemoveAll(mission => mission.IsCompleted || mission.IsFailed);
    }

    void HandleRewards()
    {
        if (AddTokens)
        {
            UpgradeUnlocks.AddCredits(TokensToAdd);
        }

        if (AddPoints)
        {
            CarManager.numPoints += PointsToAdd;
        }

        if (DoPointMultiplication)
        {
            // If a multiplier is currently active, stop the current coroutine
            if (pointMultiplierActive && pointMultiplierCoroutine != null)
            {
                StopCoroutine(pointMultiplierCoroutine);
            }
            CarManager.pointMultiplier = PointMultiplier;
            pointMultiplierActive = true;

            // Start the new coroutine and store its reference
            pointMultiplierCoroutine = StartCoroutine(ResetPointMultiplierAfterDuration());
        }

        if (DoSpeedBuff)
        {
            if (speedBuffActive && speedBuffCoroutine != null)
            {
                StopCoroutine(speedBuffCoroutine);
            }
            CarManager.speedBuff = IncreaseToMaxSpeedInMPH;
            speedBuffActive = true;

            speedBuffCoroutine = StartCoroutine(ResetSpeedBuffAfterDuration());
        }
    }

    private IEnumerator ResetPointMultiplierAfterDuration()
    {
        // Wait for the specified duration
        yield return new WaitForSeconds(PointMultiplierDuration);

        // Reset the multiplier
        CarManager.pointMultiplier = 1f;
        pointMultiplierActive = false;
    }

    private IEnumerator ResetSpeedBuffAfterDuration()
    {
        // Wait for the specified duration
        yield return new WaitForSeconds(SpeedIncreaseDuration);

        // Reset the multiplier
        CarManager.speedBuff = 0f;
        speedBuffActive = false;
    }

}
