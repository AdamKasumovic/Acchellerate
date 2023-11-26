using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimedObjectDestroyer : MonoBehaviour
{
    public float destroyAfterSeconds = 5f;
    private float timer = 0f;
    public bool useScaledTime = true;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        timer += useScaledTime ? Time.deltaTime : Time.unscaledDeltaTime;
        if (timer >= destroyAfterSeconds)
        {
            Destroy(gameObject);
        }
    }
}
