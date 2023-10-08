using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Abstract mission class representing a single mission in the level.
/// </summary>
public abstract class SingleMission : MonoBehaviour
{
    // Whether the mission fails or completes when the time runs out
    public enum MissionResult
    {
        Fail,
        Complete
    }

    // This method is called when the timer expires
    protected virtual MissionResult OnTimerExpired()
    {
        return MissionResult.Fail;
    }

    public string MissionName { get; set; }  // active mission MissionNames should be used for UI
    public bool IsCompleted { get; protected set; }
    public bool IsFailed { get; protected set; }

    [Header("Timer Settings")]
    public bool UseTimer = false;
    public float MissionDuration = 0;  // Duration in seconds

    [HideInInspector]
    public float timeRemaining;

    public bool IsActive { get; set; } = false;
    public bool ShowOnUI = true;

    // Visual Effects Variables
    [HideInInspector]
    // when progress is made, this number (0-1) determines how much the Missions UI text color 
    // is currently between the highlight color (1) and the default color (0)
    public float progressHighlightValue = 0;

    [HideInInspector]
    // This controls the failed missions' opacities
    public float failHighlightValue = 1;

    [HideInInspector]
    // This controls the completed missions' opacities
    public float completeHighlightValue = 1;

    [Header("Rewards")]
    public bool DoPointMultiplication = false;
    [Range(1, 100)]
    public float PointMultiplier = 2.0f;
    [Range(0, 1000000)]
    public float PointMultiplierDuration = 30f;
    public bool AddPoints = false;
    [Range(0, 1000000000)]
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
    private float oldBoostDuration;

    protected virtual void Start()
    {
        progressHighlightValue = 0;
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
                MissionResult result = OnTimerExpired();
                if (result == MissionResult.Fail)
                {
                    FailMission();
                }
                else if (result == MissionResult.Complete)
                {
                    CompleteMission();
                }
            }
        }
        // Fade effect of highlights
        progressHighlightValue = Mathf.Max(progressHighlightValue - Time.deltaTime*10f/11f, 0f);
        if (IsFailed)
            failHighlightValue = Mathf.Max(failHighlightValue - Time.deltaTime*10f/11f, 0f);
        if (IsCompleted)
            completeHighlightValue = Mathf.Max(completeHighlightValue - Time.deltaTime*10f/11f, 0f);
    }

    public virtual void Execute()
    {
        progressHighlightValue = 1.36f;  // this should not be changed by a designer
    }

    public virtual void FailMission()
    {
        IsActive = false;
        IsFailed = true;
        // Remove completed or failed missions
        Missions.Instance.activeMissions.RemoveAll(mission => mission.IsCompleted || mission.IsFailed);
        Missions.Instance.failedMissions.Add(this);
        failHighlightValue = 10.09f;
    }
    public virtual void CompleteMission()
    {
        IsActive = false;
        IsCompleted = true;
        
        HandleRewards();
        
        // Remove completed or failed missions
        Missions.Instance.activeMissions.RemoveAll(mission => mission.IsCompleted || mission.IsFailed);
        Missions.Instance.completedMissions.Add(this);
        completeHighlightValue = 10.09f;
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
            oldBoostDuration = CarManager.Instance.horBoostDuration;
            CarManager.Instance.horBoostDuration = 1000000000f;
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
        CarManager.Instance.horBoostDuration = oldBoostDuration;
    }

}
