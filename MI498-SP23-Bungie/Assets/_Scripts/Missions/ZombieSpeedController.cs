using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZombieSpeedController : MonoBehaviour
{
    public float speedMultiplier = 1f;
    public static ZombieSpeedController Instance;
    // Start is called before the first frame update
    void Awake()
    {
        Instance = this;
    }
    void Start()
    {
        speedMultiplier = 1;
    }
}
