using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelLoadButton : MonoBehaviour
{
    public void Start()
    {
        PreventDoubleSceneLoading.sceneLoading = false;
    }
    // Start is called before the first frame update
    public void LoadLevel(string name)
    {
        Time.timeScale = 1;
        Time.fixedDeltaTime = 0.02F;
        AkSoundEngine.StopAll();
        if (SceneManager.GetActiveScene().name == "MainMenu" && name=="LoadingScreen" && !PreventDoubleSceneLoading.sceneLoading)
        {
            PreventDoubleSceneLoading.sceneLoading = true;
            StartCoroutine(LoadFromMainMenuCoroutine(name));
        }
        else if (!PreventDoubleSceneLoading.sceneLoading)
        {
            SceneManager.LoadScene(name);
        }
    }

    private IEnumerator LoadFromMainMenuCoroutine(string name)
    {
        AudioClip clip = SfxManager.instance.PlaySound(SfxManager.SfxCategory.AnnouncerChooseLevel);
        yield return new WaitForSecondsRealtime(clip.length + 0.1f);
        SceneManager.LoadScene(name, LoadSceneMode.Additive);
        // TODO: Put an AKSoundEngine in the scene so the music can actually play.
        //AkSoundEngine.SetState("MusicState", "None");
        yield return null;
    }
    private IEnumerator LoadFromShopCoroutine(string name)
    {
        
        SceneManager.LoadScene(name, LoadSceneMode.Additive);
        yield return null;
    }
    public void RestartLevel()
    {
        LoadLevel(SceneManager.GetActiveScene().name);
    }
}