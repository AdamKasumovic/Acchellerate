using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// SingleMission that requires the player to collect all letters to spell out a certain word
/// </summary>
public class LettersMission : SingleMission
{   
    [Header("Letters Mission Settings")]
    // Possible words to spawn
    public List<string> missionWords = new List<string>();
    // Corresponding spawnpoint for words to spawn. should be same length as mission words.
    // Each GameObject should have x children where x is equivalent to the corresponding mission word.
    public List<GameObject> letterSpawnPointsParents;

    private string missionWord;
    private List<char> collectedLetters = new List<char>();

    private int chosenMissionIndex = -1;
    private bool missionActivated = false;

    List<Transform> spawns;

    void Start()
    {
        ChooseMissionWord();
        string progress = $"{collectedLetters.Count}/{missionWord.Length}";
        string timer = UseTimer ? $" Time left: {Mathf.Max(0, timeRemaining):0.0}s" : "";

        MissionName = $"Find all letters of the word: {missionWord} ({progress}).{timer}";
    }

    protected override void Update()
    {
        base.Update();
        if (IsActive && !missionActivated)
        {
            missionActivated = true;
            if(chosenMissionIndex == -1)
            {
                return;
            }
            ToggleWorldLetters();
        }
        // Failsafe. If this is not setup properly, prevent inspector from flooding with errors
        if (chosenMissionIndex != -1)
        {

            string tintString = (!IsCompleted && !IsFailed && !IsActive) ? " tint=1" : "";
            string timer = UseTimer ? $"<sprite index=0{tintString}>{Mathf.Max(0, timeRemaining):0.0}s" : "";
            string missionSymbol = $"<sprite index=8>";


            string progress = $"{collectedLetters.Count}/{missionWord.Length}";



            MissionName = $"{SpriteInsideBoxMarkdown} {missionSymbol} Find Letters of {missionWord} ({progress}) {timer}";

            
        }
    }

    /// <summary>
    /// Function to select mission word and set in scene
    /// </summary>
    private void ChooseMissionWord()
    {
        chosenMissionIndex = Random.Range(0, missionWords.Count - 1);

        if (letterSpawnPointsParents.Count - 1 < chosenMissionIndex)
        {
            Debug.LogWarning("No Mission Spawn Locations Set. Canceling Mission Activation");
            return;
        }

        // clean and grab missionWord
        missionWord = missionWords[chosenMissionIndex];
        missionWord = missionWord.Trim().ToUpper();

        // grab spawn locations for this mission word
        GameObject spawnLocationsParent = letterSpawnPointsParents[chosenMissionIndex];
        spawns = GetAllChilds(spawnLocationsParent.transform);
    }

    /// <summary>
    /// Function to turn the mission on
    /// </summary>
    private void ToggleWorldLetters()
    {
        int i = 0;
        foreach (char letter in missionWord)
        {
            spawns[i].gameObject.SetActive(true);
            spawns[i].gameObject.GetComponent<Letters>().myChar = letter;
            i++;
        }
    }

    public void LetterCollected(char letter)
    {
        collectedLetters.Add(letter);
    }

    private bool CheckCompletion()
    {
        if(collectedLetters.Count == missionWord.Length)
        {
            return true;
        }

        return false;
    }

    public override void Execute()
    {
        base.Execute();
        if (CheckCompletion())
        {
            CompleteMission();
        }
    }

    public override void FailMission()
    {
        base.FailMission();  // MUST CALL THIS!
        // Turn Off Any Remaining Letters
        foreach(Transform spawn in spawns)
        {
            spawn.gameObject.SetActive(false);
        }
        Debug.Log("Letter Mission Failed!");
    }

    public override void CompleteMission()
    {
        base.CompleteMission();  // MUST CALL THIS!
        Debug.Log("Letter Mission Completed!");
    }
}
