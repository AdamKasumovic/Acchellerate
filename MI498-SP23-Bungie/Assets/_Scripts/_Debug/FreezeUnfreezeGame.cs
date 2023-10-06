using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FreezeUnfreezeGame : MonoBehaviour
{
    private float lastFixedTimeScale = 0f;
    private float lastTimeScale = 0f;
    private bool frozen = false;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F11))
        {
            if (!frozen)
            {
                frozen = true;
                lastFixedTimeScale = Time.fixedDeltaTime;
                Time.fixedDeltaTime = 0f;
                lastTimeScale = Time.timeScale;
                Time.timeScale = 0f;
            }
            else
            {
                frozen = false;
                Time.fixedDeltaTime = lastFixedTimeScale;
                Time.timeScale = lastTimeScale;
            }
        }
    }
}
