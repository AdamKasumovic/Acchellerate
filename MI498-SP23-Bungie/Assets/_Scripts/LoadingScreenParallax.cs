using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoadingScreenParallax : MonoBehaviour
{
    [Tooltip("The distance the background should move before snapping back to its original position.")]
    public float distanceBeforeTeleport = 19.17f;
    [Tooltip("Speed to scroll the background. Negative speeds make the car look like its going right to left.")]
    public float scrollSpeed = 20f;
    private Vector3 originalPosition;
    // Start is called before the first frame update
    void Start()
    {
        originalPosition = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        transform.position -= Vector3.right * scrollSpeed * Time.deltaTime;
        if (Mathf.Abs(transform.position.x - originalPosition.x) > distanceBeforeTeleport)
        {
            transform.position = originalPosition;
        }
    }
}
