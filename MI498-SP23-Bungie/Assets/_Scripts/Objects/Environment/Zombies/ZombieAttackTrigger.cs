using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZombieAttackTrigger : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerStay(Collider other)
    {
        Animator animator = other.gameObject.GetComponentInChildren<Animator>();
        if (animator != null)
        {
            if (!animator.GetCurrentAnimatorStateInfo(0).IsName("OnBack"))
            {
                animator.SetTrigger("Attack");
            }
        }
    }
    private void OnTriggerExit(Collider other)
    {
        Animator animator = other.gameObject.GetComponentInChildren<Animator>();
        if (animator != null)
        {
            animator.SetTrigger("StopAttack");
        }
    }
}
