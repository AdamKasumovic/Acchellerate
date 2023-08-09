using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadFromLoadingScreen : MonoBehaviour
{
    public string gameSceneName = "ChrisArchesBlockout";
    public float minimumLoadTime = 3f;
    private float timer = 0f;
    public Vector3 carStartPosition;
    public Vector3 carEndPosition;
    public Transform car;
    AsyncOperation LoadOperation;
    float loadtime = 0f;

    private void Awake()
    {
        StartCoroutine(LoadScene(gameSceneName));
        loadtime = 0f;
        if (gameSceneName == "MainMenu")
            Cursor.lockState = CursorLockMode.None;
        else
            Cursor.lockState = CursorLockMode.Locked;
    }

    private void Update()
    {
        timer += Time.deltaTime;
        if (LoadOperation != null)
        {
            loadtime = Mathf.Clamp01(LoadOperation.progress / 0.9f);
        }
        if (car != null)
            car.position = Vector3.Lerp(carStartPosition, carEndPosition, (timer+loadtime) / (minimumLoadTime+1));
        Time.timeScale = 1f;
        Time.fixedDeltaTime = 0.02f;
    }

    private IEnumerator LoadScene(string sceneName)
    {
        yield return new WaitForSeconds(minimumLoadTime);

        //Load the new scene
        yield return StartCoroutine(LoadNew(sceneName));

        yield return null;
    }

    private IEnumerator LoadNew(string sceneName)
    {
        LoadOperation = SceneManager.LoadSceneAsync(sceneName);

        while (!LoadOperation.isDone)
        {
            yield return null;
        }
    }
}
