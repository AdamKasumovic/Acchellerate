using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyWhenCarHit : MonoBehaviour
{
    CarManager cm;
    // Start is called before the first frame update
    void Start()
    {
        cm = CarManager.Instance;
    }

    // Update is called once per frame
    void Update()
    {
        if (cm.gotHitRecently)
            Destroy(gameObject);
    }
}
