using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZombieAnimationController : MonoBehaviour
{
    public Animator animator;
    [HideInInspector]
    public float speed;
    private Vector3 lastPosition;
    public float attackDistanceThreshold = 0.4f;
    // Start is called before the first frame update
    void Start()
    {
        lastPosition = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        float deltaTime = Time.deltaTime;
        if (deltaTime == 0)
        {
            deltaTime = float.MaxValue;
        }
        speed = (transform.position - lastPosition).magnitude / deltaTime;
        animator.SetFloat("Velocity", speed);
        animator.SetFloat("SpeedMultiplier", ZombieSpeedController.Instance.speedMultiplier == 0 ? 0 : (5f/9f)*ZombieSpeedController.Instance.speedMultiplier + (4f/9f));

        lastPosition = transform.position;
    }
}
