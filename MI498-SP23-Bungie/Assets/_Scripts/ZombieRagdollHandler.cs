using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZombieRagdollHandler : MonoBehaviour
{
    private Rigidbody[] ragdollParts;
    public Animator animator;
    [HideInInspector]
    public bool isRagdolled = false;
    [HideInInspector]
    public bool isLyingDown = false;
    private float onDeathDestroyAfter = 10f;
    public float ragdollVerticalStrength = 2f;
    [Tooltip("Base ragdoll force")]
    public float ragdollForce = 5f;
    [Tooltip("Given the mph speed of the car at impact, add that speed times this to the base ragdoll force above")]
    public float speedForceMultiplier = 2f;
    [Tooltip("The rigidbody on the zombie to apply the force to")]
    public Rigidbody targetForForce;

    public GameObject gibObject;

    public GameObject balloon;

    private EnemyStats enemyStats;

    // Start is called before the first frame update
    void Awake()
    {
        enemyStats = transform.parent.gameObject.GetComponent<EnemyStats>();
        ragdollParts = GetComponentsInChildren<Rigidbody>();
        foreach (Rigidbody rb in ragdollParts)
        {
            rb.isKinematic = true;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (balloon != null && enemyStats != null && enemyStats.health > 0)
        {
            transform.LookAt(CarManager.Instance.transform);
            transform.eulerAngles = new Vector3(0, transform.eulerAngles.y, 0);
        }
    }

    // The death version of this function actually uses a ragdoll, while the other just uses an animation
    public void DisableRagdoll(bool deathVersion = false)
    {
        if (deathVersion)
        {
            foreach (Rigidbody rb in ragdollParts)
            {
                rb.isKinematic = true;
            }
            //animator.enabled = true;
            isRagdolled = false;
        }
        else
        {
            isLyingDown = false;
            animator.SetTrigger("GetUp");
            //animator.enabled = true;
        }
    }

    public void EnableRagdoll(bool deathVersion = false, bool applyForce = false, bool doGib = false)
    {
        if (!doGib || transform.parent.gameObject.name.Contains("Swole"))
        {
            if (deathVersion)
            {
                foreach (Rigidbody rb in ragdollParts)
                {
                    rb.isKinematic = false;
                }
                animator.enabled = false;
                isRagdolled = true;
                isLyingDown = false;

                if (applyForce)
                {
                    Vector3 direction = (transform.parent.position - CarManager.Instance.transform.position).normalized * (ragdollForce + CarManager.carSpeed * speedForceMultiplier) + Vector3.up * ragdollVerticalStrength;
                    targetForForce.AddForce(direction, ForceMode.Impulse);
                }

                StartCoroutine(DestroyAfterTime());
                if (balloon != null)
                {
                    balloon.GetComponent<FloatAway>().Float();
                    balloon.AddComponent<TimedObjectDestroyer>();
                }

            }
            else
            {
                animator.SetTrigger("Down");
                StartCoroutine(LayDown());
            }
        }
        else
        {
            Instantiate(gibObject, transform.position, transform.rotation, null);
            Destroy(transform.parent.gameObject);
        }
    }

    private IEnumerator LayDown()
    {
        isLyingDown = true;
        isRagdolled = false;
        //animator.CrossFadeInFixedTime("Down1", 0.25f);
        yield return new WaitForSeconds(0.3f);
        //animator.enabled = false;
    }
    private IEnumerator DestroyAfterTime()
    {
        yield return new WaitForSeconds(onDeathDestroyAfter);
        Destroy(transform.parent.gameObject);
    }
}
