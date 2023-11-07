using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Abstract mission class representing a single mission in the level.
/// </summary>
public abstract class SingleMission : MonoBehaviour
{
    // Common Function Used to Get All Children of a Transform.
    // Used on start to find all tokens for certain missions in a scene
    public List<Transform> GetAllChilds(Transform _t)
    {
        List<Transform> ts = new List<Transform>();

        int childCount = _t.childCount;
        Transform[] children = new Transform[childCount];

        for (int i = 0; i < childCount; i++)
        {
            ts.Add(_t.GetChild(i));
        }

        return ts;
    }

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

    public string SpriteInsideBoxMarkdown { get; private set; }  // For the mission UI--checkbox, X, exclamation point, or empty depending on mission state

    [Header("Timer Settings")]
    public bool UseTimer = false;
    public float MissionDuration = 0;  // Duration in seconds
    [Tooltip("How long should the mission be queued before it becomes active? This gives players a chance to react to missions that might be easy to fail if they popped up suddenly.")]
    public float BufferTime = 3f;

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
    public bool JumpBoostReward = false;
    [Range(0, 1000)]
    public float JumpBoostStrength = 100, JumpBoostDuration = 10;
    


   


    private Coroutine pointMultiplierCoroutine;
    private bool pointMultiplierActive = false;

    private Coroutine speedBuffCoroutine;
    private bool speedBuffActive = false;
    private float oldBoostDuration;

    private Coroutine jumpBoostCoroutine;
    private bool jumpBoostActive = false;
    private float JumpOriginal;

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
        {
            SpriteInsideBoxMarkdown = "<sprite index=5>";
        }       
        else if (IsCompleted)
        {
            SpriteInsideBoxMarkdown = "<sprite index=4>";
        }
        else if (IsActive)
        {
            SpriteInsideBoxMarkdown = "<sprite index=6>";
        }
        else
        {
            SpriteInsideBoxMarkdown = "";
        }
            


    }

    public void StartFailHighlight(float period, float totalTime, float fadeOutDuration)
    {
        StartCoroutine(HighlightCoroutine(period, totalTime, fadeOutDuration, v => failHighlightValue = v));
    }

    public void StartCompleteHighlight(float period, float totalTime, float fadeOutDuration)
    {
        StartCoroutine(HighlightCoroutine(period, totalTime, fadeOutDuration, v => completeHighlightValue = v));
    }

    private IEnumerator HighlightCoroutine(float period, float totalTime, float fadeOutDuration, System.Action<float> setValue)
    {
        float time = 0;
        while (time < totalTime)
        {
            float value = Mathf.PingPong(time / period, 0.75f) + 0.25f;
            setValue(value);
            time += Time.deltaTime;
            yield return null;
        }

        // Start fade out from current value
        float startValue = Mathf.PingPong(time / period, 0.75f) + 0.25f;
        float fadeStartTime = Time.time;
        while (Time.time < fadeStartTime + fadeOutDuration)
        {
            float value = Mathf.Lerp(startValue, 0f, (Time.time - fadeStartTime) / fadeOutDuration);
            setValue(value);
            yield return null;
        }

        setValue(0f);
    }


    public virtual void Execute()
    {
        progressHighlightValue = 1.36f;  // this should not be changed by a designer
    }

    public virtual void FailMission()
    {
        IsActive = false;
        IsFailed = true;
        SfxManager.instance.PlaySound(SfxManager.SfxCategory.MissionFail);
        // Remove completed or failed missions
        Missions.Instance.activeMissions.RemoveAll(mission => mission.IsCompleted || mission.IsFailed);
        Missions.Instance.failedMissions.Add(this);
        StartFailHighlight(0.5f, 5f, 0.67f);
    }
    public virtual void CompleteMission()
    {
        IsActive = false;
        IsCompleted = true;
        
        HandleRewards();
        SfxManager.instance.PlaySound(SfxManager.SfxCategory.MissionSuccess);
        // Remove completed or failed missions
        Missions.Instance.activeMissions.RemoveAll(mission => mission.IsCompleted || mission.IsFailed);
        Missions.Instance.completedMissions.Add(this);
        Missions.Instance.RegisterMissionCompletion();
        StartCompleteHighlight(0.5f, 5f, 0.67f);
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
        if (JumpBoostReward)
        {
            if(jumpBoostActive && jumpBoostCoroutine != null)
            {
                StopCoroutine(jumpBoostCoroutine);
                CarManager.Instance.vertBoostStrength = JumpOriginal;
            }
            JumpOriginal = CarManager.Instance.vertBoostStrength;
            CarManager.Instance.vertBoostStrength = JumpBoostStrength;
            jumpBoostActive = true;

            jumpBoostCoroutine = StartCoroutine(ResetJumpBoostAfterDuration());
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

    private IEnumerator ResetJumpBoostAfterDuration()
    {
        yield return new WaitForSeconds(JumpBoostDuration);

        CarManager.Instance.vertBoostStrength = JumpOriginal;
        jumpBoostActive = false;
    }

}
