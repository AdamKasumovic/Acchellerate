using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NukeManager : MonoBehaviour
{
    private static NukeManager _instance;
    public static NukeManager Instance { get { return _instance; } }

    public bool nukeActive = false;
    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            _instance = this;
        }
    }

    public void LaunchNuke()
    {
        //Debug.Log("Now I am become death, the destroyer of worlds!");
        //nukeActive = true;
    }
}
