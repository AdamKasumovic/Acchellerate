using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PedestalDecals : MonoBehaviour
{
    public float centerDistance = 3f;
    public float amplitude = .25f;
    public float spinRate = 1.0f;
    public GameObject powerUp;
    public GameObject currentSpawn;
    public bool collected = false;
    public bool respawn = false;
    public float timeCollected = 0f;
    public float minTimeToRespawn = 20f;
    // Start is called before the first frame update
    void Start()
    {
        currentSpawn = Instantiate(powerUp, Vector3.up * centerDistance, Quaternion.identity, transform);
        currentSpawn.GetComponent<GasCan>().parent = this;
    }

    // Update is called once per frame
    void Update()
    {
        if (!collected)
        {
            currentSpawn.transform.localPosition = Vector3.up * (centerDistance + amplitude * Mathf.Sin(Time.time));
            currentSpawn.transform.Rotate(spinRate * 360 * Vector3.up * Time.deltaTime / (2 * Mathf.PI));
        }
        else if(collected && respawn && Time.time > timeCollected+minTimeToRespawn)
        {
            collected = false;
            currentSpawn = Instantiate(powerUp, Vector3.up * centerDistance, Quaternion.identity, transform);
            currentSpawn.GetComponent<GasCan>().parent = this;
        }
    }
}
