using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZombieAttackTutorial : MonoBehaviour
{
    EnemyStats _stats;
    EnemyNavmesh navmesh;
    Animator animator;
    void Start()
    {
        animator = GetComponent<Animator>();
        _stats = transform.parent.GetComponent<EnemyStats>();
        navmesh = transform.parent.GetComponent<EnemyNavmesh>();
    }

    // Update is called once per frame
    void Update()
    {
        if ((transform.position - CarManager.Instance.gameObject.transform.position).magnitude > navmesh.maximumHitDistance)
        {
            animator.SetTrigger("StopAttack");
        }
    }
    public void Attack()
    {
        if ((transform.position - CarManager.Instance.gameObject.transform.position).magnitude < navmesh.maximumHitDistance && Tutorial.enteredAreas[7])
        {
            CarManager.carHealth -= _stats.damage;
            if (CarManager.Instance.inTutorial)
                CarManager.carHealth = Mathf.Max(CarManager.carHealth, 1);
            StartCoroutine(GameManager.instance.DoVibration(0.75f, 0.75f, 0.25f));
            CameraShaker.Instance.ShakeCamera(5, 0.1f);
            if (navmesh.isSwole)
            {
                SfxManager.instance.PlaySoundAtRandom(SfxManager.SfxCategory.SwoleZombieAttack, navmesh.audioSource);
            }
            else
            {
                SfxManager.instance.PlaySoundAtRandom(SfxManager.SfxCategory.ZombieAttack, navmesh.audioSource);
            }
            CarManager.lastHit = Time.time;
            NewStyleSystem.instance.EndCombo();
        }
    }
}
