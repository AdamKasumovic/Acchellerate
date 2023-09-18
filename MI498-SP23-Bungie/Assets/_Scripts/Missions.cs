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

    [HideInInspector]
    public List<SingleMission> activeMissions = new List<SingleMission>();

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
            tempMission.IsActive = true; // Set the mission as active
            activeMissions.Add(tempMission);
            tempMissions.RemoveAt(randIndex);
        }
    }

    private void Update()
    {
        // Remove completed or failed missions
        activeMissions.RemoveAll(mission => mission.IsCompleted || mission.IsFailed);
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
        }
    }

    [System.Serializable]
    public class TriggerMissionEntry
    {
        public Collider trigger;
        public SingleMission mission;
    }
}

