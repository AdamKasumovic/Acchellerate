using System;
using System.Collections;
using System.Collections.Generic;
using AK.Wwise;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class VolumeSettings : MonoBehaviour
{
    public AudioMixer mainMixer;
    
    public float preVoiceNonAnnouncerVolume;

    private float preVoiceMusicVolume;

    public void AddToMusicVolume(float volume)
    {
        int refID = 1;
        float setMusicVolume;
        AkSoundEngine.GetRTPCValue("Volume", gameObject.GetComponentInChildren<AkGameObj>().gameObject,
            (uint)0, out setMusicVolume, ref refID);
        Debug.Log($"Current Music Volume: {setMusicVolume}");
        volume += setMusicVolume;
        Debug.Log($"Music Volume: {volume}");
        ChangeMusicVolume(volume);
    }

    public void AddToSFXVolume(float volume)
    {
        float currentVolume;
        mainMixer.GetFloat("SFX", out currentVolume);
        volume += currentVolume;
        Debug.Log($"Volume: {volume}");
        ChangeSFXVolume(volume);
    }

    public void AddToMixerGroupVolume(float volume, string groupName)
    {
        float currentVolume;
        mainMixer.GetFloat(groupName, out currentVolume);
        Debug.Log($"Current SFX Volume: {currentVolume}");
        volume += currentVolume;
        Debug.Log($"Volume: {volume}");
        mainMixer.SetFloat(groupName, volume);
    }

    public void ChangeMusicVolume(float volume)
    {
        AkSoundEngine.SetRTPCValue("Volume", volume);
        PlayerPrefs.SetFloat("MusicVolume", volume);  // Make it persist
        // int refID = 1;
        // float setMusicVolume;
        // AkSoundEngine.GetRTPCValue("Volume", gameObject.GetComponentInChildren<AkGameObj>().gameObject,
        //     (uint)0, out setMusicVolume, ref refID);
        // Debug.Log($"RTPC Value after change {setMusicVolume}");
        // Debug.Log($"Prefs value {PlayerPrefs.GetFloat("MusicVolume")}");
    }

    // THIS FUNCTION ONLY WORKS WITH THE SLIDER
    public void ChangeSFXVolume(float volume)
    {
        float newVolume = Mathf.Log10(volume) * 20;
        //Debug.Log("New Volume: " + newVolume);
        mainMixer.SetFloat("SFX", newVolume);
        //float result;
        //mainMixer.GetFloat("SFX", out result);
        //Debug.Log($"Current mixer volume: {result}");
        PlayerPrefs.SetFloat("SFXVolume", volume);  // Make it persist
    }
    
    // THIS FUNCTION ONLY WORKS WITH THE SLIDER
    public void ChangeAnnouncerVolume(float volume)
    {
        float newVolume = Mathf.Log10(volume) * 20;
        //Debug.Log("New Volume: " + newVolume);
        //TODO: Change this for EVERY time player prefs are set. Really look for it!
        mainMixer.SetFloat("Announcer", newVolume);
        //float result;
        //mainMixer.GetFloat("SFX", out result);
        //Debug.Log($"Current mixer volume: {result}");
        PlayerPrefs.SetFloat("AnnouncerVolume", volume);  // Make it persist
    }

    public void SetAudioChannelVolume(string audioChannel, float volume)
    {
        mainMixer.SetFloat(audioChannel, volume);
    }

    public bool LerpAudio(float amountToIncreaseVolume = 20f,float duration = .5f, bool usePreviousVolume = false)
    {
        // Use a negative number for amountToIncrease if you'd like to decrease it.
        float percentComplete = 0;
        float elapsedTime = 0;
        
        int refID = 1;

        float startingMusicVolume;
        AkSoundEngine.GetRTPCValue("Volume", gameObject.GetComponentInChildren<AkGameObj>().gameObject,
            (uint)0, out startingMusicVolume, ref refID);
        Debug.Log($"Starting Music Volume: {startingMusicVolume}");

        float startingNonAnnouncerVolume;
        mainMixer.GetFloat("Non-Announcer", out startingNonAnnouncerVolume);
        Debug.Log($"Starting Non Announcer Volume: {startingNonAnnouncerVolume}");

        /*if (!usePreviousVolume)
        {
            // Set the current volume so it can be returned to.
            Debug.Log($"Prefs: {PlayerPrefs.GetFloat("MusicVolume")}");
            preVoiceNonAnnouncerVolume = startingNonAnnouncerVolume;
            preVoiceMusicVolume = PlayerPrefs.GetFloat("MusicVolume");
        }*/

        while (percentComplete < 1)
        {
            elapsedTime += Time.unscaledDeltaTime;
            percentComplete = elapsedTime / duration;
            float musicVolume = Mathf.Lerp(startingMusicVolume, startingMusicVolume + amountToIncreaseVolume, percentComplete);
            float sfxVolume = Mathf.Lerp(startingNonAnnouncerVolume, startingNonAnnouncerVolume + amountToIncreaseVolume, percentComplete);
            Debug.Log($"Music Volume: {musicVolume}");
            Debug.Log($"SFX Volume: {sfxVolume}");
            AkSoundEngine.SetRTPCValue("Volume", musicVolume);
            mainMixer.SetFloat("Non-Announcer", sfxVolume);
            float setMusicVolume, setSFXVolume = 0;
            AkSoundEngine.GetRTPCValue("Volume", gameObject.GetComponentInChildren<AkGameObj>().gameObject,
                (uint)0, out setMusicVolume, ref refID);
            mainMixer.GetFloat("Non-Announcer", out setSFXVolume);
            Debug.Log($"Set Music Volume: {setMusicVolume}");
            Debug.Log($"Set SFX Volume: {setSFXVolume}");
        }

        // Signal a flag that the lerp is done
        return true;
    }
}
