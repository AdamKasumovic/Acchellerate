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

    private void Start()
    {
        ActivateRandomStartMissions();
        ScheduleNextRandomMission();
        ActivateRandomStartMissions();
        ScheduleNextRandomMission();
        ActivateRandomStartMissions();
        ScheduleNextRandomMission();
        ActivateRandomStartMissions();
        ScheduleNextRandomMission();
        ActivateRandomStartMissions();
        ScheduleNextRandomMission();
        ActivateRandomStartMissions();
        ScheduleNextRandomMission();
        ActivateRandomStartMissions();
        ScheduleNextRandomMission();
        ActivateRandomStartMissions();
        ScheduleNextRandomMission();
        ActivateRandomStartMissions();
        ScheduleNextRandomMission();
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
            tempMission.IsActive = true; // Set the mission as active
            activeMissions.Add(tempMission);
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
        if (randomIntervalMissions.Count == 0 || activeMissions.Count >= maxActiveRandomMissions) return;

        // Get missions that have not been previously activated
        List<SingleMission> availableMissions = randomIntervalMissions.Except(previouslyActivatedRandomMissions).ToList();

        // If all missions have been previously activated, clear the list and use all missions
        if (availableMissions.Count == 0)
        {
            previouslyActivatedRandomMissions.Clear();
            availableMissions = new List<SingleMission>(randomIntervalMissions);
        }

        int randMissionIndex = Random.Range(0, availableMissions.Count);
        SingleMission mission = availableMissions[randMissionIndex];
        mission.IsActive = true; // Set the mission as active
        activeMissions.Add(mission);

        // Add the activated mission to the previouslyActivated list
        previouslyActivatedRandomMissions.Add(mission);

        ScheduleNextRandomMission();
    }

    public void OnTriggerEnter(Collider other)
    {
        foreach (TriggerMissionEntry entry in triggerMissions)
        {
            if (entry.trigger == other)
            {
                SingleMission triggerMission = entry.mission;
                triggerMission.IsActive = true; // Set the mission as active
                activeMissions.Add(triggerMission);
                return;
            }
        }
    }

    // ADD UPDATERS BASED ON MISSION TYPES HERE FOLLOWING THIS EXAMPLE:

    // Call this with something like:
    //Missions missionsComponent = Missions.Instance;
    //missionsComponent.RegisterKill(EnemyType.regular, KillType.frontFlip);
    public void RegisterKill(EnemyType enemyType, KillType killType)
    {
        foreach (SingleMission mission in activeMissions)
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
        }
    }

    // Call this when colliding with a letter object in the environemnt for LettersMission
    public void RegisterLetter(char letter)
    {
        foreach (SingleMission mission in activeMissions)
        {
            if (mission is LettersMission)
            {
                LettersMission letterMission = mission as LettersMission;
                letterMission.LetterCollected(letter);
                letterMission.Execute();
            }
        }
    }

    // Call this when doing a move
    public void RegisterMove(MoveType moveType)
    {
        foreach (SingleMission mission in activeMissions)
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
        foreach (SingleMission mission in activeMissions)
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

