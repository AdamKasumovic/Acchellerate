using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FloatAway : MonoBehaviour
{
    private bool floatAway = false;
    public float floatSpeed = 5f;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (floatAway)
        {
            transform.position += Vector3.up * floatSpeed * Time.deltaTime;
        }
    }

    public void Float()
    {
        floatAway = true;
    }
}
