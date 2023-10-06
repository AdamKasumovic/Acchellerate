# if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
public class EditModeFunctions : EditorWindow
{
    [MenuItem("Window/Edit Mode Functions")]
    public static void ShowWindow()
    {
        GetWindow<EditModeFunctions>("Edit Mode Functions");
    }
 
    private void OnGUI()
    {
        GUILayout.Label("Change Game State");
        if (GUILayout.Button("Win"))
        {
            GameManager.instance.HandleWin();
        }
        else if (GUILayout.Button("Lose"))
        {
            GameManager.instance.HandleLose();
        }

        GUILayout.Label("Change Music State");
        if (GUILayout.Button("Default Instruments")) 
        {
            AudioManager.instance.SetState("Default");
        }

        else if (GUILayout.Button("D Rank")) 
        {
            AudioManager.instance.SetState("DRank");
        }

        else if (GUILayout.Button("C Rank")) 
        {
            AudioManager.instance.SetState("CRank");
        }

        else if (GUILayout.Button("B Rank")) 
        {
            AudioManager.instance.SetState("BRank");
        }

        else if (GUILayout.Button("A Rank")) 
        {
            AudioManager.instance.SetState("ARank");
        }

        else if (GUILayout.Button("Z Rank")) 
        {
            AudioManager.instance.SetState("ZRank");
        }
    }
}
#endif