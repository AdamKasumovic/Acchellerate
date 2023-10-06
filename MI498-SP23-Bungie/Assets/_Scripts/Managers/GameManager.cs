using System.Collections;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;


public class GameManager : MonoBehaviour
{
    public int[] pointsFromRankIndex = { 1, 2, 3, 5, 8 };
    public enum GameStates { play, pause, menu, win, lose }
    public GameStates gameState = GameStates.play;
    public InputAction pause;
    public LevelPoints data;
    public bool gameIsPaused;
    public UIManager uiManager;
    
    // An instance of the GameManager that can be called from any script.
    public static GameManager instance;

    public string upgradeMenuName = "GaragePage";
    public int upgradeMenuPage = 4;
    public GasStationManager gasStationManager;
    public VolumeSettings volumeSettings;

    private void Awake()
    {
        PreventDoubleSceneLoading.sceneLoading = false;
        if (instance == null)
        {
            instance = this.GetComponent<GameManager>();
        }
        instance.gameState = GameStates.play;
        gameIsPaused = false;
        Time.timeScale = 1;
        Time.fixedDeltaTime = 0.02F;
        if (Gamepad.current != null)
            Gamepad.current.ResetHaptics();

        pause.Enable();
        volumeSettings.ChangeMusicVolume(PlayerPrefs.GetFloat("MusicVolume", 0.25f));
        volumeSettings.ChangeSFXVolume(PlayerPrefs.GetFloat("SFXVolume", 0.25f));
        volumeSettings.ChangeAnnouncerVolume(PlayerPrefs.GetFloat("AnnouncerVolume", 0.25f));
    }

    private void Start()
    {
        
    }


    // Update is called once per frame
    private void Update()
    {
        LookForPauseInput();

        Cursor.lockState = (gameState == GameStates.play) ? CursorLockMode.Locked : CursorLockMode.None;
        
        // TEMPORARY DEBUG FUNCTIONALITY
        if (Input.GetKeyDown(KeyCode.F5))
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
    }

    private void LookForPauseInput()
    {
        if(uiManager.currentPage == upgradeMenuPage)
        {
            return;
        }
        if (pause.triggered && gameState == GameStates.menu)
        {
            HandleLeavingMenu();
        }
        else if (!SfxManager.countingDown && pause.triggered && (gameState == GameStates.play ||
                                     gameState == GameStates.pause))
        {
            HandlePause();
        }
    }
    
    public void HandlePause()
    {
        // Toggle game state
        if (gameState == GameStates.play)
        {
            gameState = GameStates.pause;
            volumeSettings.SetAudioChannelVolume("Car", -80);
        }
        else if (gameState == GameStates.pause)
        {
            gameState = GameStates.play;
            volumeSettings.SetAudioChannelVolume("Car", 0);
        }
        
        // Toggle time scale
        Time.timeScale = gameIsPaused ? 1 : 0;
        gameIsPaused = !gameIsPaused;
        //Debug.Log($"Time.timeScale: {Time.timeScale}");
        if (gameIsPaused)
            SfxManager.instance.PlaySound(SfxManager.SfxCategory.Pause);
        // Update the UI
        uiManager.HandlePause();
    }
    public void HandleMenu(string menuName)
    {
        gameState = GameStates.menu;

        // Toggle time scale
        if (menuName != upgradeMenuName)
            Time.timeScale = 0;
        //Debug.Log(menuName);

        // Update the UI
        uiManager.GoToPageByName(menuName);
    }
    public void HandleLeavingMenu()
    {
        gameState = GameStates.play;

        // Toggle time scale
        Time.timeScale = 1;
        Time.fixedDeltaTime = 0.02F;
        Debug.Log($"Time.timeScale: {Time.timeScale}");

        if (uiManager.currentPage == upgradeMenuPage)
        {
            gasStationManager.ResetCar();
        }

        // Update the UI
        uiManager.GoToPageByName("InGamePage");
    }

    public void HandleWin()
    {
        //StyleSystem.instance.EndCombo();
        gameState = GameStates.win;
        ImageSelector.instance.SetImageOnWin();
        UpgradeUnlocks.AddCredits(pointsFromRankIndex[data.GetGradeFromPoints((int)CarManager.numPoints)]);
        volumeSettings.SetAudioChannelVolume("Car", -80);
        Time.timeScale = 0;
        gameIsPaused = true;
        uiManager.GoToPageByName("WinPage");
        AudioManager.instance.SetState("");
    }

    public void HandleLose()
    {
        //Debug.Log("LOSE");
        gameState = GameStates.lose;
        volumeSettings.SetAudioChannelVolume("Car", -80);
        Time.timeScale = 0f;
        uiManager.GoToPageByName("LosePage");
    }

    public IEnumerator DoVibration(float lSpeed, float rSpeed, float duration)
    {
        if (Gamepad.current != null)
        {
            Gamepad.current.SetMotorSpeeds(lSpeed, rSpeed);
            yield return new WaitForSecondsRealtime(duration);
            Gamepad.current.ResetHaptics();
        }
        yield return null;
    }

    private void OnApplicationQuit()
    {
        if (Gamepad.current != null)
            Gamepad.current.ResetHaptics();
    }
    private void OnApplicationPause()
    {
        if (Gamepad.current != null)
            Gamepad.current.ResetHaptics();
    }
}