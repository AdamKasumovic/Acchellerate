using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GibForce : MonoBehaviour
{
    public float ragdollVerticalStrength = 100f;
    public float ragdollForce = 750f;
    public float speedForceMultiplier = 1.5f;
    public bool head = false;
    // Start is called before the first frame update
    void Start()
    {
        if (!head)
            transform.eulerAngles = new Vector3(Random.Range(0f, 360f), Random.Range(0f, 360f), Random.Range(0f, 360f));
        Vector3 direction = (new Vector3(transform.position.x - CarManager.Instance.transform.position.x, 0, transform.position.z - CarManager.Instance.transform.position.z)*.25f+ new Vector3(Random.insideUnitSphere.x, 0, Random.insideUnitSphere.z)).normalized * (head ? 0.25f : 1) * (ragdollForce + CarManager.carSpeed * speedForceMultiplier) + Vector3.up * ragdollVerticalStrength;
        GetComponent<Rigidbody>().AddForce(direction, ForceMode.Impulse);
        GetComponent<Rigidbody>().AddTorque(Random.insideUnitSphere.normalized*360f, ForceMode.Impulse);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
