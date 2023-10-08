using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// SingleMission that requires the player to find a hidden collectable
/// You have to provide a list of possible spawn points in editor for it to spawn from
/// </summary>
public class SecretCollectableMission : SingleMission
{ 
    [Header("No Kill Mission Settings")]
    [Tooltip("add all spawnpoints")]
    public List<Transform> spawns;
    public GameObject collectablePrefab;
    public GameObject collectable;



    protected override void Start()
    {
        base.Start();  // don't get rid of this
    
        int rand = Random.Range(0, spawns.Count);
        collectable = Instantiate(collectablePrefab, spawns[rand]);
    }

    protected override void Update()
        {   
        base.Update();

        string progress = collectable.GetComponent<SecretCollectable>().trigger ? $"." : "Not Found Yet.";
        string timer = UseTimer ? $" Time left: {Mathf.Max(0, timeRemaining):0.0}s" : "";  // this works already, probably leave it alone. Note that it gives the empty string if there's no timer for the mission.
    

    


        // You are responsible for ensuring that "MissionName" contains the appopriate text that informs players
        // about the mission progress at all times.
        MissionName = $"Find the secret totem. {timer}";
        Debug.Log(MissionName);
        }





    public override void Execute()
    {
        base.Execute();
        CompleteMission();

    }

    public override void FailMission()
    {
    base.FailMission();  // MUST CALL THIS!
        Destroy(collectable.gameObject);
        collectable = null;
    Debug.Log("Collectable Mission Failed!");
    }

    public override void CompleteMission()
    {
    base.CompleteMission();  // MUST CALL THIS!
        Destroy(collectable.gameObject);
        collectable = null;
        Debug.Log("Collectable Mission Completed!");
    }

    protected override MissionResult OnTimerExpired()
    {
    // For this specific mission, when the timer expires, we want to complete/fail the mission.
    return MissionResult.Fail;
    }

}