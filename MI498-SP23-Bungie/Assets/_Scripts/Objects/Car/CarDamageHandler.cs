using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarDamageHandler : MonoBehaviour
{
    public float colliderSpeed = 60;
    private BoxCollider box;
    private CapsuleCollider[] capsules;

    // Start is called before the first frame update
    void Start()
    {
        box = GetComponent<BoxCollider>();
        capsules = GetComponents<CapsuleCollider>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if(CarManager.carSpeed > colliderSpeed || !CarManager.Instance.carController.isGrounded || CarManager.Instance.isSpinning)
        {
            box.isTrigger = true;
            capsules[0].isTrigger = true;
            capsules[1].isTrigger = true;
        }
        else
        {
            box.isTrigger = false;
            capsules[0].isTrigger = false;
            capsules[1].isTrigger = false;
        }
    }
    /*
    void OnCollisionEnter(Collision other)
    {
        //Debug.Log(CarManager.lastHit);
        var obstacle = other.gameObject;
        //No collision or self collision -- do nothing
        if (obstacle == null || obstacle.layer == 8) // || obstacle.GetComponent<Rigidbody>() == null)
        {
            return;
        }
        var stats = obstacle.GetComponent<EnemyStats>();
        if (stats != null)
        {
            SfxManager.instance.PlaySoundAtRandom(SfxManager.SfxCategory.ZombieAttack);
            CarManager.carHealth -= stats.damage;
            CarManager.lastHit = Time.time;
        }
    }
    */
}
