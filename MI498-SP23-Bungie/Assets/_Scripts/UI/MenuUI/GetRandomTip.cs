using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GetRandomTip : MonoBehaviour
{
    public TextAsset tips;
    public string[] tipsArr;
    public TextMeshProUGUI textbox;
    // Start is called before the first frame update
    void Start()
    {
        int tipIndex = PlayerPrefs.GetInt("tipIndex", 0);
        tipsArr = tips.text.Split('\n');
        textbox.text = tipsArr[tipIndex % (tipsArr.Length-1)];
        PlayerPrefs.SetInt("tipIndex", tipIndex + 1);
    }

}
