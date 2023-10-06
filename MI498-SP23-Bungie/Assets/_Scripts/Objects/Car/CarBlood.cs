using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class CarBlood : MonoBehaviour
{
    public DecalProjector[] dps;
    public int killsForMaxBlood = 8;
    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        foreach (DecalProjector dp in dps)
        {
            if (NewStyleSystem.instance != null)
            {
                float fadeFactor = Mathf.Clamp01((float)NewStyleSystem.instance.realMultiplier / (float)killsForMaxBlood);
              float currentVelocity = 0;
               dp.fadeFactor = Mathf.SmoothDamp(dp.fadeFactor, fadeFactor, ref currentVelocity, 0.01f, 1f);
            }
        }
    }
}
