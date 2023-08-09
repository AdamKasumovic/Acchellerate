using System.Collections.Generic;
using UnityEngine;
using System;
using System.Collections;
using TMPro;
using UnityEngine.SceneManagement;
using Random = UnityEngine.Random;

// Created a struct to map strings to lists of audio files so that
// they can be edited directly from the inspector. You cannot do this using dictionaries.
[Serializable]
public struct SfxMap
{
    public SfxManager.SfxCategory sfxCategory;
    public List<AudioClip> sfxClips;
    public float cooldownTime;
}

public class SfxManager : MonoBehaviour
{
    // Every time a new sound effect is implemented, an enum should be added here.
    // This makes it easier to assign sound effects in the inspector as well as referencing
    // them in code.
    public enum SfxCategory
    {
        ZombieIdle,
        ZombieAggro,
        ZombieAttack,
        ZombieDeath,
        CarHitZombie,
        Splat,
        GroundPound, 
        Boost,
        Jump,
        Flip,
        Burnout,
        AnnouncerGameOpen,
        AnnouncerChooseLevel,
        AnnouncerRankUp,
        AnnouncerReady,
        AnnouncerStart,
        AnnouncerLevelComplete,
        MainMenuRev,
        Tornado,
        AnnouncerDropComboDRank,
        AnnouncerDropComboCRank,
        AnnouncerDropComboBRank,
        AnnouncerDropComboARank,
        AnnouncerDropComboZRank,
        Pause,
        SwoleZombieAggro,
        SwoleZombieAttack,
        SwoleZombieDeath,
        LowFuel
    }

    public static SfxManager instance;

    private static Dictionary<SfxCategory, List<AudioClip>> _sfxDictionary = new Dictionary<SfxCategory, List<AudioClip>>();
    private static Dictionary<SfxCategory, bool> _cooldownDictionary = new Dictionary<SfxCategory, bool>();
    private static Dictionary<SfxCategory, float> _cooldownTimeDictionary = new Dictionary<SfxCategory, float>();
    private AudioSource _audioSource;

    [SerializeField] private bool debugSound;
    
    [SerializeField] private List<SfxMap> maps;

    public float mainMenuAnnouncerVolume = .6f;
    public float gameAnnouncerVolume = 1f;
    
    public static bool countingDown = false;
    
    //[SerializeField] private TextMeshProUGUI readyText;
    //[SerializeField] private TMP_FontAsset grungeFontAsset;
    //[SerializeField] private GameObject gameStartImage;
    [SerializeField] private GameObject threeImage;
    [SerializeField] private GameObject twoImage;
    [SerializeField] private GameObject oneImage;
    [SerializeField] private GameObject goImage;

    void Awake()
    {
        // singleton code
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != this)
        {
            Destroy(gameObject);
            Debug.LogError("Multiple SFXManagers in scene");
        }
        
        foreach (SfxMap map in maps)
        {
            if (!_sfxDictionary.ContainsKey(map.sfxCategory))
            {
                _sfxDictionary.Add(map.sfxCategory, map.sfxClips);
                _cooldownDictionary.Add(map.sfxCategory, false);
                _cooldownTimeDictionary.Add(map.sfxCategory, map.cooldownTime);
            }
        }
        _audioSource = GetComponent<AudioSource>();

        if (_audioSource == null)
        {
            Debug.LogError("No AudioSource attached to GameManager.");
        }
    }

    private IEnumerator Start()
    {
        string sceneName = SceneManager.GetActiveScene().name;
        HashSet<string> countdownScenes = new HashSet<string> { "TutorialLevel", "Canyon_Main", "Canyon_CircleTrack", "Canyon_Arches", "Farm_Main" };  // scenes that should have countdowns
        if (sceneName == "MainMenu")
        {
            Debug.Log("Main Menu SFXManager");
            VolumeSettings mainMenuVolumeSettings = GetComponentInParent<VolumeSettings>();
            mainMenuVolumeSettings.ChangeMusicVolume(PlayerPrefs.GetFloat("MusicVolume", 0.25f));
            mainMenuVolumeSettings.ChangeSFXVolume(PlayerPrefs.GetFloat("SFXVolume", 0.25f));
            mainMenuVolumeSettings.ChangeAnnouncerVolume(PlayerPrefs.GetFloat("AnnouncerVolume", 0.25f));
            yield return new WaitForSecondsRealtime(0.1f);
            AudioClip mainMenuClip = PlaySoundAtRandom(SfxCategory.AnnouncerGameOpen);
            yield return new WaitForSecondsRealtime(mainMenuClip.length - 1f);
            //AkSoundEngine.PostEvent("MusicEvent", AudioManager.instance.gameObject.GetComponentInChildren<AkAmbient>().gameObject);
            //AudioManager.instance.SetState("MainMenu");
        }
        else if (countdownScenes.Contains(sceneName))
        { 
            Debug.Log("Main Level SFXManager");
            AudioManager.instance.SetState("Default");
            // Calculate/Retrieve the track number and then set the state using the audio manager.
            int trackNumber = Random.Range(2, 3);
            Debug.Log(trackNumber);
            countingDown = true;
            Time.timeScale = 0;
            // Play "3" SFX here
            //AudioClip clip = PlaySoundAtRandom(SfxCategory.AnnouncerReady, AudioManager.instance.announcerSource);
            yield return new WaitForSecondsRealtime(1);
            threeImage.SetActive(false);
            twoImage.SetActive(true);
            // Play "2" SFX here
            yield return new WaitForSecondsRealtime(1);
            twoImage.SetActive(false);
            oneImage.SetActive(true);
            // Play "1" SFX here
            yield return new WaitForSecondsRealtime(1);
            oneImage.SetActive(false);
            goImage.SetActive(true);
            // Play "GO" SFX here
			
            //readyText.text = "GO!";
            //clip = PlaySoundAtRandom(SfxCategory.AnnouncerStart, AudioManager.instance.announcerSource);
            //yield return new WaitForSecondsRealtime(clip.length);
            yield return new WaitForSecondsRealtime(0.5f);
            Time.timeScale = 1;
            yield return new WaitForSecondsRealtime(1);
            GameManager.instance.uiManager.GoToPage(0);
            goImage.SetActive(false);
            threeImage.SetActive(true);  // TODO: Make script that causes image to fade-in from transparent to opaque and large to small
            countingDown = false;
        }
        else if (sceneName == "ShopLevel")
        {
            Debug.Log("Shop");
            Debug.Log($"AK Ambient: {AudioManager.instance.gameObject.GetComponentInChildren<AkAmbient>().gameObject}");
            AkSoundEngine.PostEvent("MusicEvent", AudioManager.instance.gameObject.GetComponentInChildren<AkAmbient>().gameObject);
            AudioManager.instance.SetState("Shop");
        }
        // Reset the car volume so that it goes back to playing after the level is over. This should happen for any scene.
        if (GameManager.instance != null)
            GameManager.instance.volumeSettings.SetAudioChannelVolume("Car", 0);
    }

    // Have a certain percentage to play a specified sound effect
    // (Has a chance to call PlaySound)
    public void ChanceToPlaySound(SfxCategory category, float percentChanceToPlay, int index = 0, AudioSource source = null)
    {
        AudioSource sourceToUse = source != null ? source : _audioSource;
        float randomNumber = Random.Range(0.0f, 1.0f);

        if (randomNumber >= percentChanceToPlay)
        {
            PlaySound(category, index, sourceToUse);
        }
    }
    
    // Have a certain percentage to play a random sound effect from the list of the specified category
    // (Has a chance to call PlaySoundAtRandom)
    public void ChanceToPlaySoundAtRandom(SfxCategory category, float percentChanceToPlay, AudioSource source = null)
    {
        AudioSource sourceToUse = source != null ? source : _audioSource;
        float randomNumber = Random.Range(0.0f, 1.0f);

        if (randomNumber >= 1f - percentChanceToPlay)
        {
            PlaySoundAtRandom(category, sourceToUse);
        }
    }

    public void ChanceToPlaySoundAtPoint(SfxCategory category, float percentChanceToPlay, Vector3 position, int index = 0, AudioSource source = null)
    {
        AudioSource sourceToUse = source != null ? source : _audioSource;
        float randomNumber = Random.Range(0.0f, 1.0f);

        if (randomNumber >= percentChanceToPlay)
        {
            PlaySoundAtPoint(category, position, index, sourceToUse);
        }
    }
    
    // Has a chance to call PlayRandomSoundAtPoint()
    public void ChanceToPlayRandomSoundAtPoint(SfxCategory category, float percentChanceToPlay, Vector3 position, AudioSource source = null)
    {
        AudioSource sourceToUse = source != null ? source : _audioSource;

        float randomNumber = Random.Range(0.0f, 1.0f);

        if (randomNumber >= percentChanceToPlay)
        {
            PlayRandomSoundAtPoint(category, position, sourceToUse);
        }
    }

    // Play a random sound effect from the list of the specified category
    public AudioClip PlaySoundAtRandom(SfxCategory category, AudioSource source = null, bool randomPitch = false)
    {
        AudioSource sourceToUse = source != null ? source : _audioSource;
        int upperBound = _sfxDictionary[category].Count - 1;
        return PlaySound(category, Random.Range(0, upperBound), sourceToUse, randomPitch);
    }

    // Call PlaySoundAtRandom to play a sound at the specified coordinates.
    public void PlayRandomSoundAtPoint( SfxCategory category, Vector3 position, AudioSource source = null)
    {
        AudioSource sourceToUse = source != null ? source : _audioSource;
        //Debug.Log($"SFX Dictionary: {_sfxDictionary}");
        int upperBound = _sfxDictionary[category].Count - 1;
        PlaySoundAtPoint(category, position, Random.Range(0, upperBound), sourceToUse);
    }

    // Play a sound of specified category at specified index of the list of that category's potential sfx
    public AudioClip PlaySound(SfxCategory category, int index = 0, AudioSource source = null, bool randomPitch = false)
    {
        AudioSource sourceToUse = source != null ? source : _audioSource;
        //Debug.Log($"Source game object: {sourceToUse.gameObject}");
        //Debug.Log($"Source mixer: {sourceToUse.outputAudioMixerGroup}");
        // Get rid of this later if it screws something up. 
        if (_cooldownDictionary[category] == false)
        {
            if (_sfxDictionary.ContainsKey(category) && _sfxDictionary[category].Count > index
                                                     && (!sourceToUse.isPlaying || !_sfxDictionary[category].Contains(sourceToUse.clip)))
            {
                if (randomPitch)
                {
                    sourceToUse.pitch = Random.Range(1, 3);
                    //Debug.Log("Pitch: " + sourceToUse.pitch);
                }
                else
                {
                    // Reset the pitch if it is set to something else
                    sourceToUse.pitch = 1;
                }
                sourceToUse.PlayOneShot(_sfxDictionary[category][index]);
                sourceToUse.pitch = 1;
                if (debugSound)
                {
                    Debug.Log("Played sound effect: " + _sfxDictionary[category][index]);
                }
                StartCoroutine(CooldownCoroutine(category));

                return _sfxDictionary[category][index];
            }
        }

        return null;
    }

    // Call PlaySound to play a sound at the specified coordinates
    public void PlaySoundAtPoint(SfxCategory category, Vector3 position, int index = 0, AudioSource source = null)
    {
        AudioSource sourceToUse = source != null ? source : _audioSource;
        
        if (_cooldownDictionary[category] == false)
        {
            // Get rid of this later if it screws something up. 
            if (_sfxDictionary.ContainsKey(category) && _sfxDictionary[category].Count > index
                                                     && (!sourceToUse.isPlaying || !_sfxDictionary[category].Contains(sourceToUse.clip)))
            {
                sourceToUse.PlayOneShot(_sfxDictionary[category][index]);
                if (debugSound)
                {
                    Debug.Log("Played sound effect: " + _sfxDictionary[category][index]);
                }

                StartCoroutine(CooldownCoroutine(category));
            }
        }
    }
    
    // Stop sound of specified category at specified index
    public void StopSound(SfxCategory category, int index = 0, AudioSource source = null)
    {
        AudioSource sourceToUse = source != null ? source : _audioSource;
        sourceToUse.Stop();
        if (debugSound)
        {
            Debug.Log("Stopped sound effect: " + _sfxDictionary[category][index]);
        }
    }

    // Is the sound of the specified category at the specified index currently playing
    public bool IsPlaying(SfxCategory category, int index = 0, AudioSource source = null)
    {
        AudioSource sourceToUse = source != null ? source : _audioSource;
        bool isPlaying = _sfxDictionary.ContainsKey(category) && _sfxDictionary[category].Count > index
                                                              && sourceToUse.isPlaying &&
                                                              _sfxDictionary[category].Contains(sourceToUse.clip);
        Debug.Log($"{category} is playing: {isPlaying}");
        return isPlaying;
    }

    IEnumerator CooldownCoroutine(SfxCategory category)
    {
        _cooldownDictionary[category] = true;
        yield return new WaitForSecondsRealtime(_cooldownTimeDictionary[category]);
        _cooldownDictionary[category] = false;
    }
}