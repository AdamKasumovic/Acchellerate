using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyDetection : MonoBehaviour
{
    /*[Tooltip("Whether or not the zombies should buddy up")]
    public bool buddyUp = true;*/

    private EnemyNavmesh _enemyNavmesh;
    
    /*
    private bool foundGroup = false;
    */

    
    void Awake()
    {
        _enemyNavmesh = gameObject.GetComponentInParent<EnemyNavmesh>();
    }
    
    void OnTriggerEnter(Collider other)
    {
        /*if (buddyUp && !foundGroup)
        {
            EnemyDetection otherDetection = other.gameObject.GetComponent<EnemyDetection>();
            if (otherDetection != null && otherDetection.buddyUp)
            {
                _enemyNavmesh.SetCarTarget(otherDetection.gameObject, true);
                foundGroup = true;
            }
        }*/
        
        //CarTarget carTarget = other.gameObject.GetComponent<CarTarget>();
        if (other.gameObject.tag == "Player")//carTarget != null)
        {
            _enemyNavmesh.SetCarTarget(other.gameObject.gameObject/*, false*/);
            if (_enemyNavmesh.isSwole)
            {
                SfxManager.instance.PlaySoundAtRandom(SfxManager.SfxCategory.SwoleZombieAggro, _enemyNavmesh.audioSource);
            }
            else
            {
                SfxManager.instance.PlaySoundAtRandom(SfxManager.SfxCategory.ZombieAggro, _enemyNavmesh.audioSource);
            }
        }
    }

    void OnTriggerExit(Collider other)
    {
        // This check needs to be here so another zombie exiting the collider doesn't trigger it.
        //CarTarget carTarget = other.gameObject.GetComponent<CarTarget>();
        if (other.gameObject.tag == "Player")//carTarget != null)
        {
            // We may need to add an edge case where an enemy will find 
            // another enemy when the player leaves.
            _enemyNavmesh.RemoveTarget();
        }
    }
}
