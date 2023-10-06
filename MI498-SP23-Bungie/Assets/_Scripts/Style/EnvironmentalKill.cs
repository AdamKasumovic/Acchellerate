using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnvironmentalKill : MonoBehaviour
{
    public float damageMultiplier = 5f;
    public int basePoints = 500;
    private Rigidbody rb;
    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    private void OnCollisionEnter(Collision collision)
    {
        float damage = damageMultiplier * rb.velocity.magnitude;
        if(collision.gameObject.tag == "Enemy")
        {
            if (collision.gameObject.GetComponent<EnemyStats>().Damage(damage) > 0)
            {
                int realPoints =  NewStyleSystem.instance.AddPointsWithMultiplier(basePoints);
                StyleTextbox.instance.AddRewardInfoAndHandleTimeout($"Environment Kill", realPoints, 2.5f);
            }

        }
    }
}
