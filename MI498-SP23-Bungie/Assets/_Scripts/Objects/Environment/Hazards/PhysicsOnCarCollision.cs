using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PhysicsOnCarCollision : MonoBehaviour
{
    public string vulnerableTag = "CarDestructible";
    public float optionalForce = 2f;
    public ClusterFall cf;
    public bool stopColliding = true;
    Missions mission;
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
        if (other.tag == vulnerableTag)
        {   
            if (cf != null)
            {
                cf.TriggerCollapse();
            }
            else 
            {
                if(mission == null)
                {
                    mission = Missions.Instance;
                    Missions.Instance.RegisterDestruction();
                }
                MakeKinematic();
            }
            //other.GetComponent<Rigidbody>().AddForce((other.transform.position-transform.parent.parent.position).normalized * 2f, ForceMode.Impulse);
            // Play a sound here!
        }
    }

    public void MakeKinematic()
    {
        Rigidbody rb = GetComponent<Rigidbody>();
        if (rb != null)
        {
            GetComponent<Rigidbody>().isKinematic = false;
            if(stopColliding)
                StartCoroutine(MakeNotCollideWithCar());
        }
    }

    IEnumerator MakeNotCollideWithCar()
    {
        yield return new WaitForSeconds(0.15f);
        int LayerIgnoreRaycast = LayerMask.NameToLayer("ZombieRagdoll");
        gameObject.layer = LayerIgnoreRaycast;
        yield return new WaitForSeconds(10f);
        Destroy(gameObject);
    }
}
