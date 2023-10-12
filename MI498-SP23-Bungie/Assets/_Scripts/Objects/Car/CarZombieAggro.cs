using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarZombieAggro : MonoBehaviour
{
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
        var navmesh = other.gameObject.GetComponent<EnemyNavmesh>();
        if (navmesh != null)
        {
            other.gameObject.GetComponent<EnemyNavmesh>().SetCarTarget(transform.parent.gameObject);
            
            SfxManager.instance.PlayRandomSoundAtPoint(SfxManager.SfxCategory.ZombieAggro, other.gameObject.transform.position);
            navmesh.inTornado = CarManager.Instance.tornado;
            //navmesh.animator.SetTrigger("Notice");
        }
    }
    private void OnTriggerExit(Collider other)
    {
        var navmesh = other.gameObject.GetComponent<EnemyNavmesh>();
        if(navmesh != null)
        {
            navmesh.RemoveTarget();
            navmesh.inTornado = false;
        }
    }
}
