using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.IO;

public class LoadLevelByInput : MonoBehaviour
{
    public InputField inputField;
    public Text sceneNames;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void LoadLevel()
    {
        int sceneCount = SceneManager.sceneCountInBuildSettings;
        string[] scenes = new string[sceneCount];

        for (int i = 0; i < sceneCount; i++)
        {
            scenes[i] = Path.GetFileNameWithoutExtension(SceneUtility.GetScenePathByBuildIndex(i));
        }

        string requestedScene = inputField.text;

        for (int i = 0; i < sceneCount; i++)
        {
            if (scenes[i].ToLower().Contains(requestedScene.ToLower()) && scenes[i].ToLower()[0] == requestedScene.ToLower()[0])
            {
                SceneManager.LoadScene(scenes[i]);
                return;
            }
        }

        DisplaySceneNames(requestedScene, scenes);
    }

    void DisplaySceneNames(string requestedScene, string[] scenes)
    {
        sceneNames.enabled = true;
        string resultingText = requestedScene + " not found.\nAvailable scenes:\n";
        string newline = "\n";
        for (int i = 0; i < scenes.Length; ++i)
        {
            newline = i % 4 == 3 ? "\n" : ""; 
            resultingText += i != scenes.Length - 1 ? scenes[i] + ", " + newline : scenes[i];
        }
        sceneNames.text = resultingText;
    }
}
