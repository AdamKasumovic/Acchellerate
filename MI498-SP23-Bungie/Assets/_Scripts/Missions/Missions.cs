using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

/// <summary>
/// Class containing all SingleMissions for the level this script exists in. This should probably go on the car wherever Trigger collision is handled.
/// </summary>
public class Missions : MonoBehaviour
{

    // Singleton Instance
    public static Missions Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
        }
    }

    [Header("Start Missions")]
    [Tooltip("All possible missions at the start")]
    public List<SingleMission> possibleStartMissions;
    [Tooltip("Number of start missions to randomly activate")]
    public int numberOfStartMissionsToActivate = 3;

    [Header("Random Interval Missions")]
    public List<SingleMission> randomIntervalMissions;
    public float minInterval = 10f;
    public float maxInterval = 60f;
    [Tooltip("Maximum number of random missions that can be active at once. Set to 0 to disable random missions.")]
    public int maxActiveRandomMissions = 3;

    [Header("Trigger Missions")]
    public List<TriggerMissionEntry> triggerMissions;

    public List<SingleMission> activeMissions = new List<SingleMission>();
    public List<SingleMission> completedMissions = new List<SingleMission>();
    public List<SingleMission> failedMissions = new List<SingleMission>();
    public List<SingleMission> queuedMissions = new List<SingleMission>();

    private void Start()
    {
        ActivateRandomStartMissions();


        // Schedule the first random mission
        ScheduleNextRandomMission();
    }

    private void ActivateRandomStartMissions()
    {
        List<SingleMission> tempMissions = new List<SingleMission>(possibleStartMissions);
        for (int i = 0; i < numberOfStartMissionsToActivate && tempMissions.Count > 0; i++)
        {
            int randIndex = Random.Range(0, tempMissions.Count);
            SingleMission tempMission = tempMissions[randIndex];
            queuedMissions.Add(tempMission);
            StartCoroutine(AddToActiveMissions(tempMission, tempMission.BufferTime));
            tempMissions.RemoveAt(randIndex);
        }
    }

    private void Update()
    {

    }

    private void ScheduleNextRandomMission()
    {
        float nextInterval = Random.Range(minInterval, maxInterval);
        Invoke(nameof(ActivateRandomMission), nextInterval);
    }

    private List<SingleMission> previouslyActivatedRandomMissions = new List<SingleMission>();

    private void ActivateRandomMission()
    {
        if (randomIntervalMissions.Count == 0 || (queuedMissions.Count + activeMissions.Count) >= maxActiveRandomMissions)
        {
            ScheduleNextRandomMission();
            return;
        }

        // Get missions that have not been previously activated
        List<SingleMission> availableMissions = randomIntervalMissions.Except(previouslyActivatedRandomMissions).ToList();

        // If all missions have been previously activated, clear the list and use all missions (causes bugs right now)
        if (availableMissions.Count == 0)
        {
            //previouslyActivatedRandomMissions.Clear();
            //availableMissions = new List<SingleMission>(randomIntervalMissions);
        }

        int randMissionIndex = Random.Range(0, availableMissions.Count);
        SingleMission mission;
        if (availableMissions.Count > 0)
            mission = availableMissions[randMissionIndex];
        else
            mission = null;
        if (mission != null && !mission.IsActive)
        {
            queuedMissions.Add(mission);
            StartCoroutine(AddToActiveMissions(mission, mission.BufferTime));

            // Add the activated mission to the previouslyActivated list
            previouslyActivatedRandomMissions.Add(mission);
        }

        ScheduleNextRandomMission();
    }

    public void OnTriggerEnter(Collider other)
    {
        foreach (TriggerMissionEntry entry in triggerMissions)
        {
            if (entry.trigger == other)
            {
                SingleMission triggerMission = entry.mission;
                queuedMissions.Add(triggerMission);
                StartCoroutine(AddToActiveMissions(triggerMission, triggerMission.BufferTime));
                return;
            }
        }
    }

    IEnumerator AddToActiveMissions(SingleMission mission, float delay)
    {
        yield return new WaitForSeconds(delay);
        queuedMissions.Remove(mission);
        activeMissions.Add(mission);
        mission.IsActive = true; // Set the mission as active
    }

    // ADD UPDATERS BASED ON MISSION TYPES HERE FOLLOWING THIS EXAMPLE:

    // Call this with something like:
    //Missions missionsComponent = Missions.Instance;
    //missionsComponent.RegisterKill(EnemyType.regular, KillType.frontFlip);
    public void RegisterKill(EnemyType enemyType, KillType killType)
    {
        // Create a copy of the activeMissions list
        List<SingleMission> missionsCopy = new List<SingleMission>(activeMissions);
        foreach (SingleMission mission in missionsCopy)
        {
            if (mission is KillMission)
            {
                KillMission killMission = mission as KillMission;
                if ((killMission.missionEnemyType == enemyType || killMission.missionEnemyType == EnemyType.any) &&
                    (killMission.missionKillType == killType || killMission.missionKillType == KillType.any))
                {
                    killMission.Execute();
                }
            }
            else if (mission is NoKillMission)
            {
                NoKillMission killMission = mission as NoKillMission;
                if (killMission.missionEnemyType == enemyType || killMission.missionEnemyType == EnemyType.any)
                {
                    killMission.Execute();
                }
            }
            else if (mission is KillsWithEveryMoveMission)
            {
                KillsWithEveryMoveMission killMission = mission as KillsWithEveryMoveMission;
                killMission.AddKill(killType);
            }
            else if (mission is SingleMoveKillMission)
            {
                SingleMoveKillMission killMission = mission as SingleMoveKillMission;
                if ((killMission.missionEnemyType == enemyType || killMission.missionEnemyType == EnemyType.any) &&
                    (killMission.missionKillType == killType || killMission.missionKillType == KillType.any))
                {
                    killMission.Execute();
                }
            }
        }
    }

    //call this when colliding with a secret collectable
    public void RegisterCollect()
    {
        // Create a copy of the activeMissions list
        List<SingleMission> missionsCopy = new List<SingleMission>(activeMissions);
        foreach (SingleMission mission in missionsCopy)
        {
            if (mission is SecretCollectableMission)
            {
                SecretCollectableMission secretCollectableMission = mission as SecretCollectableMission;
                secretCollectableMission.Execute();
            }
        }
    }
    public void RegisterDestruction()
    {
        // Create a copy of the activeMissions list
        List<SingleMission> missionsCopy = new List<SingleMission>(activeMissions);
        foreach (SingleMission mission in missionsCopy)
        {
            if (mission is DestroyDestructibleObjectsMission)
            {
                DestroyDestructibleObjectsMission destroyDestructibleObjectsMission = mission as DestroyDestructibleObjectsMission;
                destroyDestructibleObjectsMission.Execute();
            }
        }
    }
    public void RegisterNoDamage()
    {
        // Create a copy of the activeMissions list
        List<SingleMission> missionsCopy = new List<SingleMission>(activeMissions);
        foreach (SingleMission mission in missionsCopy)
        {
            if (mission is NoDamageMission)
            {
                NoDamageMission noDamageMission = mission as NoDamageMission;
                noDamageMission.Execute();
            }
        }
    }


    public void RegisterColliderExit()
    {
        // Create a copy of the activeMissions list
        List<SingleMission> missionsCopy = new List<SingleMission>(activeMissions);
        foreach (SingleMission mission in missionsCopy)
        {
            if (mission is RankWithinRadiusMission)
            {
                RankWithinRadiusMission rankWithinRadiusMission = mission as RankWithinRadiusMission;
                rankWithinRadiusMission.FailMission();
            }
        }
    }
    public void RegisterGreadLetter(int greadID)
    {
        // Create a copy of the activeMissions list
        List<SingleMission> missionsCopy = new List<SingleMission>(activeMissions);
        foreach (SingleMission mission in missionsCopy)
        {
            if (mission is GradeWithoutGasMission)
            {
                GradeWithoutGasMission gradeWithoutGasMission = mission as GradeWithoutGasMission;
                gradeWithoutGasMission.NewGreadLetter(greadID);
                gradeWithoutGasMission.Execute();
            }
            if (mission is RankWithinRadiusMission)
            {
                RankWithinRadiusMission rankWithinRadiusMission = mission as RankWithinRadiusMission;
                rankWithinRadiusMission.NewGreadLetter(greadID);
                rankWithinRadiusMission.Execute();
            }
        }
    }

    public void RegisterGasPickup()
    {
        // Create a copy of the activeMissions list
        List<SingleMission> missionsCopy = new List<SingleMission>(activeMissions);
        foreach (SingleMission mission in missionsCopy)
        {
            if (mission is GradeWithoutGasMission)
            {
                GradeWithoutGasMission gradeWithoutGasMission = mission as GradeWithoutGasMission;
                gradeWithoutGasMission.FailMission();
            }
        }
    }

    // Call this when colliding with a letter object in the environment for LettersMission
    public void RegisterLetter(char letter)
    {
        // Create a copy of the activeMissions list
        List<SingleMission> missionsCopy = new List<SingleMission>(activeMissions);
        foreach (SingleMission mission in missionsCopy)
        {
            if (mission is LettersMission)
            {
                LettersMission letterMission = mission as LettersMission;
                letterMission.LetterCollected(letter);
                letterMission.Execute();
            }
        }
    }

    public void RegisterArea(string areaName)
    {
        // Create a copy of the activeMissions list
        List<SingleMission> missionsCopy = new List<SingleMission>(activeMissions);
        foreach (SingleMission mission in missionsCopy)
        {
            if (mission is AreaVisitMission)
            {
                AreaVisitMission areaMission = mission as AreaVisitMission;
                areaMission.AreaTokenCollected(areaName);
            }
        }
    }
    public void RegisterCrash()
    {
        // Create a copy of the activeMissions list
        List<SingleMission> missionsCopy = new List<SingleMission>(activeMissions);
        foreach (SingleMission mission in missionsCopy)
        {
            if (mission is NoCrashMission)
            {
                NoCrashMission noCrashMission = mission as NoCrashMission;
                noCrashMission.Execute();
            }
            if (mission is NoCrashForgivingMission)
            {
                NoCrashForgivingMission noCrashForgivingMission = mission as NoCrashForgivingMission;
                noCrashForgivingMission.Execute();
            }
        }
    }
    public void RegisterMissionCompletion()
    {
        // Create a copy of the activeMissions list
        List<SingleMission> missionsCopy = new List<SingleMission>(activeMissions);
        foreach (SingleMission mission in missionsCopy)
        {
            if (mission is CompleteMissionsMission)
            {
                CompleteMissionsMission completeMissionsMission = mission as CompleteMissionsMission;
                completeMissionsMission.Execute();
            }
        }
    }

    // Call this when turning headlights on or off
    public void RegisterHeadlights()
    {
        // Create a copy of the activeMissions list
        List<SingleMission> missionsCopy = new List<SingleMission>(activeMissions);
        foreach (SingleMission mission in missionsCopy)
        {
            if (mission is HeadlightsOffMission)
            {
                HeadlightsOffMission headlightsOffMission = mission as HeadlightsOffMission;
                headlightsOffMission.Execute();
            }
            else if (mission is HeadlightsMashingMission)
            {
                HeadlightsMashingMission headlightsMashingMission = mission as HeadlightsMashingMission;
                headlightsMashingMission.Execute();
            }
        }
    }

    public void RegisterHeight()
    {
        List<SingleMission> missionsCopy = new List<SingleMission>(activeMissions);
        foreach (SingleMission mission in missionsCopy)
        {
            if (mission is HeightMission)
            {
                HeightMission heightMission = mission as HeightMission;
                heightMission.Execute();
            }
        }
    }

    // Call this when doing a move
    public void RegisterMove(MoveType moveType)
    {
        // Create a copy of the activeMissions list
        List<SingleMission> missionsCopy = new List<SingleMission>(activeMissions);
        foreach (SingleMission mission in missionsCopy)
        {
            if (mission is UseMovesMission)
            {
                UseMovesMission useMovesMission = mission as UseMovesMission;
                if (useMovesMission.missionMoveType == moveType || useMovesMission.missionMoveType == MoveType.any)
                {
                    useMovesMission.Execute();
                }
            }
            else if (mission is NoMoveMission)
            {
                NoMoveMission noMoveMission = mission as NoMoveMission;
                if (noMoveMission.missionMoveType == moveType || noMoveMission.missionMoveType == MoveType.any)
                {
                    noMoveMission.Execute();
                }
            }
        }
    }

    // Call this from the appropriate script that knows when the needed event happens like:
    //Missions missionsComponent = Missions.Instance;
    //missionsComponent.SampleUpdater(...);
    public void SampleUpdater(bool param1, int param2)
    {
        // Create a copy of the activeMissions list
        List<SingleMission> missionsCopy = new List<SingleMission>(activeMissions);
        foreach (SingleMission mission in missionsCopy)
        {
            if (mission is InheritedMissionTemplate)  // change with your mission type
            {
                InheritedMissionTemplate imMission = mission as InheritedMissionTemplate;
                // do stuff here like FailMission(), CompleteMission(), or Execute()
            }
        }
    }

    [System.Serializable]
    public class TriggerMissionEntry
    {
        public Collider trigger;
        public SingleMission mission;
    }
}

