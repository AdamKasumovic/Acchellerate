using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using LootLocker.Requests;
using TMPro;

public class LeaderboardLoad : MonoBehaviour
{
    public TextMeshProUGUI text; 
    void Start()
    {
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


    public void GetScores(int amount)
    {
        LootLockerSDKManager.GetScoreList("13782", amount, 0, (response) =>
        {
            if (response.statusCode == 200)
            {
                LootLockerLeaderboardMember[] scores = response.items;
                string[] scoresArrays = new string[scores.Length];
                for(int i = 0; i < scores.Length; i++)
                {
                    scoresArrays[i] = "<mspace=.6em> <align=\"left\">" + (scores[i].rank + "").PadRight(2) + " " + scores[i].member_id + "</align><line-height=0></mspace>\n<mspace=.6em> <align=right>" + scores[i].score + "pts<line-height=1em></mspace>";
                }
                text.text = string.Join("\n", scoresArrays);
            }
            else
            {
                Debug.Log("failed: " + response.Error);
            }
        });
    }
}
