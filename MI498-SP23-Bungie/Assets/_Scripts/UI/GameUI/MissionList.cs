using Gaia;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class MissionList : MonoBehaviour
{

    Missions missions;
    public List<GameObject> missionsOnTheScreen = new List<GameObject>();

    [SerializeField]
    TMP_FontAsset fontAsset;
    public void Start()
    {
        missions = Missions.Instance;
    }
    public void newMission()
    {
        GameObject aMission = new GameObject();
        aMission.name = "Mission";
        aMission.AddComponent<CanvasRenderer>();
        aMission.AddComponent<TextMeshProUGUI>();
        aMission.GetComponent<TextMeshProUGUI>().font = fontAsset;
        aMission.GetComponent<TextMeshProUGUI>().fontSize = 17;
        aMission.GetComponent<TextMeshProUGUI>().enableWordWrapping = false;
        aMission.GetComponent<TextMeshProUGUI>().alignment = TextAlignmentOptions.Left;
        aMission.GetComponent<TextMeshProUGUI>().overflowMode = TextOverflowModes.Overflow;
        aMission.transform.parent = this.gameObject.transform;
        aMission.transform.localPosition = new Vector3(-20,-80,0);
        aMission.layer = 5;
        
        missionsOnTheScreen.Add(aMission);
    }

    private void Update()
    {
            if(missions.activeMissions.Count > missionsOnTheScreen.Count)
                newMission();
        for (int i = 0; i < missionsOnTheScreen.Count; i++)
        {
            if(i < missions.activeMissions.Count)
            {
                missionsOnTheScreen[i].GetComponent<TextMeshProUGUI>().text = missions.activeMissions[i].MissionName;
                missionsOnTheScreen[i].transform.localPosition = new Vector3(230, -40 - 80 * i, 0);
            }
            else
            {
                Destroy(missionsOnTheScreen[i]);
            }
        }

    }
}
