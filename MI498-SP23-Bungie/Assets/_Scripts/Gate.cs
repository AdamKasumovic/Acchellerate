using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gate : MonoBehaviour
{
    [Tooltip("Gates are by default in the closed state. This is the direction to move when the gate opens.")]
    public Vector3 openDirection = Vector3.down;
    [Tooltip("A POSITIVE distance to travel when opening.")]
    public float openDistance = 11f;
    [Tooltip("Time it takes to open and close in seconds.")]
    public float openTime = 1f;
    [HideInInspector]
    public bool open = false;
    private float lerper = 0f;
    private Vector3 originalPosition;
    // Start is called before the first frame update
    void Start()
    {
        originalPosition = transform.position;
        openDirection = openDirection.normalized;
    }

    // Update is called once per frame
    void Update()
    {
        if (open)
        {
            lerper += Time.deltaTime;
            lerper = Mathf.Min(lerper, openTime);
        }
        else
        {
            lerper -= Time.deltaTime;
            lerper = Mathf.Max(lerper, 0f);
        }
        transform.position = Vector3.Lerp(originalPosition, originalPosition + openDirection * openDistance, lerper/openTime);
    }
}
