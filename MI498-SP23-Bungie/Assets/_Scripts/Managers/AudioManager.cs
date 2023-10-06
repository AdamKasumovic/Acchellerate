using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Assertions.Must;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;

public class AudioManager : MonoBehaviour
{
    // TODO: When more songs get added, have a list of the music events that can happen and rotate through them after upgrading or playing again.
    // On restart, you could pass in the index of the last played song and increment it.

    public static AudioManager instance;

    [Header("Parameters")]
    
    public LevelPoints data;

    [Tooltip("The amount to change the music volume when the announcer is talking")]
    public float amountToChangeMusicVolume = 20f;
    
    [Tooltip("The amount to change the volume of the other SFX when the announcer is talking")]
    public float amountToChangeSFXVolume = 20f;

    private Grade lastGrade;

    private int rankIndex = -1;

    private bool hasChangedOnWin = false;

    public AudioSource announcerSource;

    private AkState track;
    
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }

        track = GetComponentInChildren<AkState>();
        Debug.Log($"Track: {track.data}");
    }

    private void Update()
    {
        if (data != null && SceneManager.GetActiveScene().name != "ShopLevel")
        {
            StyleStateUpdate();
        }
    }

    public void StyleStateUpdate()
    {
        int points = (int)CarManager.numPoints;
        int gradeIdx = data.GetGradeFromPoints(points);
        Grade grade = data.levelPoints[gradeIdx];
        if (grade.name != lastGrade.name /*&& 
            !GameManager.instance.gameState.Equals(GameManager.GameStates.win)*/)
        {
            rankIndex = gradeIdx;
            SetState(grade.styleDisplayName);
            lastGrade = grade;
            if (grade.name != "DESTRUCTIVE" && !(CarManager.Instance.inTutorial && Tutorial.enteredAreas[6] && !Tutorial.enteredAreas[7]))
            {
                StartCoroutine(AnnouncerRankUpCoroutine());
            }
        }

        if (GameManager.instance.gameState.Equals(GameManager.GameStates.win) && !hasChangedOnWin)
        {
            SetState(grade.styleDisplayName + "LevelComplete");
            AkSoundEngine.SetState("TrackState", track.data + "LevelComplete");
            hasChangedOnWin = true;
            StartCoroutine(OnLevelCompleteCoroutine());
        }
    }

    private IEnumerator OnLevelCompleteCoroutine()
    {
        yield return new WaitForSecondsRealtime(3f);
        SfxManager.instance.PlaySound(SfxManager.SfxCategory.AnnouncerLevelComplete, rankIndex, announcerSource);
    }

    public void SetState(string state)
    {
        uint stateInt;
        AkSoundEngine.GetState("MusicState", out stateInt);
        Debug.Log($"Current State: {state}");
        AkSoundEngine.SetState("MusicState", state);
        AkSoundEngine.GetState("MusicState", out stateInt);
        Debug.Log($"New State: {state}");
    }

    public void SetEvent(string newEvent)
    {
        Debug.Log($"GameObject with AkAmbient: {gameObject.GetComponentInChildren<AkEvent>().gameObject.name}");
        AkSoundEngine.PostEvent(newEvent, gameObject.GetComponentInChildren<AkAmbient>().gameObject);
    }

    public IEnumerator AnnouncerRankUpCoroutine()
    {
        //GameManager.instance.volumeSettings.AddToMusicVolume(-1 * amountToChangeMusicVolume);
        //GameManager.instance.volumeSettings.AddToMixerGroupVolume(-1 * amountToChangeSFXVolume, "Non-Announcer");
        AudioClip clip = SfxManager.instance.PlaySound(SfxManager.SfxCategory.AnnouncerRankUp, rankIndex, announcerSource);
        yield return null;
        //yield return new WaitForSecondsRealtime(clip.length);
        //GameManager.instance.volumeSettings.AddToMusicVolume(amountToChangeMusicVolume);
        //GameManager.instance.volumeSettings.AddToMixerGroupVolume(amountToChangeSFXVolume, "Non-Announcer");
    }

    public void PlayComboDroppedVoiceLine(int comboCount)
    {
        Debug.Log("Playing Combo Dropped voice line with count: {comboCount}");
        if (Enumerable.Range(11,25).Contains(comboCount))
        {
            Debug.Log("Playing D Rank Voice Line");
            SfxManager.instance.ChanceToPlaySoundAtRandom(SfxManager.SfxCategory.AnnouncerDropComboDRank, 1, AudioManager.instance.announcerSource);
        }
        else if (Enumerable.Range(26,40).Contains(comboCount))
        {
            Debug.Log("Playing C Rank Voice Line");
            SfxManager.instance.ChanceToPlaySoundAtRandom(SfxManager.SfxCategory.AnnouncerDropComboCRank, 1, AudioManager.instance.announcerSource);
        }
        else if (Enumerable.Range(41, 60).Contains(comboCount))
        {
            Debug.Log("Playing D Rank Voice Line");
            SfxManager.instance.ChanceToPlaySoundAtRandom(SfxManager.SfxCategory.AnnouncerDropComboBRank, 1, AudioManager.instance.announcerSource);
        }
        else if (Enumerable.Range(61, 100).Contains(comboCount))
        {
            Debug.Log("Playing D Rank Voice Line");
            SfxManager.instance.ChanceToPlaySoundAtRandom(SfxManager.SfxCategory.AnnouncerDropComboARank, 1, AudioManager.instance.announcerSource);
        }
        else if (comboCount > 100)
        {
            Debug.Log("Playing D Rank Voice Line");
            SfxManager.instance.ChanceToPlaySoundAtRandom(SfxManager.SfxCategory.AnnouncerDropComboZRank, 1, AudioManager.instance.announcerSource);
        }
    }
}
    