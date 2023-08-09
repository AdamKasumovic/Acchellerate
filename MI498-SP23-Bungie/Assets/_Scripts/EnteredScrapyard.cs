using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnteredScrapyard : MonoBehaviour
{
    public int indexToSet = 0;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            Tutorial.enteredAreas[indexToSet] = true;
            Debug.Log("ENTERED: " + indexToSet );
        }
    }
}
