using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZombieEnablerHelper : MonoBehaviour
{
    int colliderCount = 0;
    EnemyStats parent;
    // Start is called before the first frame update
    void Start()
    {
        parent = transform.parent.gameObject.GetComponent<EnemyStats>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    private void OnTriggerEnter(Collider other)
    {
        parent.insidePlayer = true;
        colliderCount++;

    }
    /*private void OnTriggerStay(Collider other)
    {
        parent.insidePlayer = true;
    }*/
    private void OnTriggerExit(Collider other)
    {
        colliderCount--;
        if(colliderCount == 0)
        {
            parent.insidePlayer = false;
        }
    }
}
