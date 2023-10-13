using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AreaToken : MonoBehaviour
{
    public string areaName;

    private void OnTriggerEnter(Collider other)
    {
        gameObject.SetActive(false);
        Missions.Instance.RegisterArea(areaName);
        Debug.Log($"Area {areaName} visited");
    }
}
