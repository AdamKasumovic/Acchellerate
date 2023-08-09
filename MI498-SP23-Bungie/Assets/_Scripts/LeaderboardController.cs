using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using LootLocker.Requests;
using TMPro;

public class LeaderboardController : MonoBehaviour
{
    public TMP_InputField playerName;
    private bool submitted = false;
    public TextAsset[] profanitylist;
    public HashSet<string> profanity = new HashSet<string>();

    // Start is called before the first frame update
    void Start()
    {
        profanity = new HashSet<string>();
        for(int i = 0; i < profanitylist.Length; i++)
        {
            string[] lines = profanitylist[i].text.Split('\n');
            for (int j = 0; j < lines.Length; j++)
            {
                profanity.Add(lines[j]);
            }
        }
        LootLockerSDKManager.StartGuestSession((response) =>
        {
            if (response.statusCode == 200)
            {
                Debug.Log("Successful");
            }
            else
            {
                Debug.Log("failed: " + response.Error);
            }
        });
    }

    public void SubmitScore()
    {
        if (submitted)
        {
            return;
        }
        if (profanity.Contains(playerName.text))
        {
            submitted = true;
            return;
        }
        LootLockerSDKManager.SubmitScore(playerName.text, (int)CarManager.numPoints, "13782", (response) =>
        {
            if (response.statusCode == 200)
            {
                submitted = true;
                Debug.Log("Successful");
            }
            else
            {
                Debug.Log("failed: " + response.Error);
            }
        });
    }
}
