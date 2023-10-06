using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClearToOpenGate : MonoBehaviour
{
    public Gate gate;
    public float radius = 5f;
    private bool open = true;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        open = true;
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, radius);
        foreach (var hitCollider in hitColliders)
        {
            if (hitCollider.tag == "Enemy")
            {
                open = false;
                break;
            }
        }
        gate.open = open;
    }

    //Draw the Box Overlap as a gizmo to show where it currently is testing. Click the Gizmos button to see this
    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        //Check that it is being run in Play Mode, so it doesn't try to draw this in Editor mode
        Gizmos.DrawWireSphere(transform.position, radius);
    }
}
