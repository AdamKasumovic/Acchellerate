using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.VFX;

public class ZombieKillbox : MonoBehaviour
{
    public bool scaleHitbox = false;
    public enum faces {Front, Left, Right, Back, Top, Bottom, Any};
    public faces face;
    [Range(0.0f, 1.0f)]
    public float timeMultiplier = 0.25f;
    public float hitStopDuration = 0.025f;
    public float hitSlowDuration = 0.10f;
    public bool enableHitStopCooldown = true;
    public float hitStopCooldownDuration = 3f;
    public Vector3 mainDirection;
    [DoNotSerialize] public MultipleCarTriggerHandler mHandler;
    private bool currentlyStopped;
    public int highValueKillPoints = 3000;
    public GameObject deathVFX;


    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {


        if (GameManager.instance != null && GameManager.instance.gameState == GameManager.GameStates.pause)
        {
            //Debug.Log("IN MENU");
            Time.timeScale = 0f;
            //StopAllCoroutines();
        }

    }
    void OnTriggerEnter(Collider other)
    {
        var obstacle = other.gameObject;
        var stats = obstacle.GetComponent<EnemyStats>();
        if (stats == null || !stats.isEnabled)
        {
            return;
        }
        var navmesh = obstacle.GetComponent<EnemyNavmesh>();
        if (navmesh != null)
            navmesh.insideKillboxTriggerAmt++;
        var enemytransform = obstacle.transform;
        var cartransform = transform.parent.parent.parent;
        //No collision or self collision -- do nothing
        if (other.gameObject.tag != "Enemy") // || obstacle.GetComponent<Rigidbody>() == null)
        {
            return;
        }

        //the obstacle is a zombie
        if (stats != null)
        {
            int points = mHandler.ZombieHit(other, cartransform.InverseTransformDirection(transform.TransformDirection(mainDirection)), stats, currentlyStopped);
            if (points > 0)
            {
                bool slomo;
                //StyleSystem.instance.AddPoints(CarManager.currentState, face);
                if (!navmesh.isFlying)
                {
                     slomo = NewStyleSystem.instance.AddCarKill();
                }
                else
                {
                    int hvpoints =NewStyleSystem.instance.AddPointsWithMultiplier(highValueKillPoints);
                    StyleTextbox.instance.AddRewardInfoAndHandleTimeout($"RARE ZOMBIE KILL", hvpoints, 2, 0, true);
                    slomo = true;
                }
                GameObject dvfx = Instantiate(deathVFX, enemytransform.position, Quaternion.LookRotation(obstacle.transform.position - cartransform.position)); //Instantiate VFX particle for physical hit
                //dvfx.GetComponent<VisualEffect>().SetVector3("Direction", Vector3.up* (obstacle.transform.position - cartransform.position).magnitude + -(obstacle.transform.position - cartransform.position)); // Set Visual effect direction.
                ////Debug.LogWarning(cartransform.GetComponent<Rigidbody>().velocity.magnitude);
                //dvfx.GetComponent<VisualEffect>().SetFloat("Speed", cartransform.GetComponent<Rigidbody>().velocity.magnitude); // Set velocity magnitude as factor of car speed.

                if (stats.spawner.restore)
                    stats.spawner.numEnemiesSpawned--;
                if (!currentlyStopped && slomo)
                {
                    StartCoroutine(HitStopCoroutine());
                }
                else
                {
                    SfxManager.instance.PlaySoundAtRandom(SfxManager.SfxCategory.Splat);
                }

                EnemyNavmesh enemyNavmesh = obstacle.GetComponent<EnemyNavmesh>();
                if (enemyNavmesh != null && enemyNavmesh.isSwole)
                {
                    SfxManager.instance.PlaySoundAtRandom(SfxManager.SfxCategory.SwoleZombieDeath, enemyNavmesh.audioSource);
                }
                else
                {
                    SfxManager.instance.PlaySoundAtRandom(SfxManager.SfxCategory.ZombieDeath, enemyNavmesh.audioSource);
                }
            }
        }
    }
    void OnTriggerStay(Collider other)
    {
        var obstacle = other.gameObject;
        //No collision or self collision -- do nothing
        if (other.gameObject.tag != "Enemy") // || obstacle.GetComponent<Rigidbody>() == null)
        {
            return;
        }
        var navmesh = obstacle.GetComponent<EnemyNavmesh>();
        if (navmesh != null)
            navmesh.insideKillboxTriggerAmt++;
        var stats = obstacle.GetComponent<EnemyStats>();
        var enemytransform = obstacle.transform;
        var cartransform = transform.parent.parent.parent;
        //Vector3 force = Vector3.zero;

        //if (CarManager.Instance.boost)
        //{
        //    if (CarManager.Instance.reverse)
        //    {
        //        force.z -= 10;
        //    }
        //    else
        //    {
        //        force.z += 10;
        //    }
        //}
        //if (CarManager.Instance.rightBoost)
        //{
        //    force.x += 3;
        //}
        //if (CarManager.Instance.leftBoost)
        //{
        //    force.x -= 3;
        //}
        //int direction = CarManager.Instance.reverse ? -1 : 1;
        //Vector3 dir = new Vector3(Mathf.Sin(Mathf.Deg2Rad * CarManager.steering), 0, Mathf.Cos(Mathf.Deg2Rad * CarManager.steering)) * direction;

        //force += dir;
        //force *= -1;
        //Vector3 localizedOtherPos = transform.InverseTransformPoint(other.gameObject.transform.position);
        //float scaling = Vector3.Dot(force,(localizedOtherPos - CarManager.Instance.gameObject.transform.position).normalized);
        /*
        if (CarManager.engineThrottle >= .9)
        {
            if (!stats.isSwole)
            {
                if (CarManager.currentState == CarManager.CarState.Idle
                    && Vector3.Dot(mainDirection, Vector3.forward) >= Mathf.Sqrt(2) / 2 )
                {
                    stats.Push(mainDirection * CarManager.engineThrottle * 20.0f, true);
                }
                else if (((CarManager.currentState == CarManager.CarState.Idle
                    && CarManager.steering < -20) || (CarManager.currentState == CarManager.CarState.Backward
                    && CarManager.steering > 20))
                    && Vector3.Dot(mainDirection, Vector3.left) >= Mathf.Sqrt(2) / 2
                    )
                {
                    stats.Push(mainDirection * CarManager.engineThrottle * 20.0f, true);
                }
                else if (((CarManager.currentState == CarManager.CarState.Idle
                    && CarManager.steering > -20) || (CarManager.currentState == CarManager.CarState.Backward
                    && CarManager.steering < 20))
                    && Vector3.Dot(mainDirection, Vector3.right) >= Mathf.Sqrt(2) / 2
                    )
                {
                    stats.Push(mainDirection * CarManager.engineThrottle * 20.0f, true);
                }
                else if (CarManager.currentState == CarManager.CarState.Backward
                    && Vector3.Dot(mainDirection, Vector3.back) >= Mathf.Sqrt(2) / 2)
                {
                    stats.Push(mainDirection * CarManager.engineThrottle * 20.0f, true);
                }
            }
        }
        */
        if (CarInputManager.Instance.boost && !CarInputManager.Instance.boostRefreshing)
        {
            //the obstacle is a zombie
            if (stats != null)
            {
                int points = mHandler.ZombieHit(other, cartransform.InverseTransformDirection(transform.TransformDirection(mainDirection)), stats, currentlyStopped);
                if (points > 0)
                {
                    bool slomo;
                    //StyleSystem.instance.AddPoints(CarManager.currentState, face);
                    if (!navmesh.isFlying)
                    {
                        slomo = NewStyleSystem.instance.AddCarKill();
                    }
                    else
                    {
                        int hvpoints = NewStyleSystem.instance.AddPointsWithMultiplier(highValueKillPoints);
                        StyleTextbox.instance.AddRewardInfoAndHandleTimeout($"RARE ZOMBIE KILL", hvpoints, 2, 0, true);
                        slomo = true;
                    }
                    GameObject dvfx = Instantiate(deathVFX, enemytransform.position, Quaternion.LookRotation(obstacle.transform.position - cartransform.position)); //Instantiate VFX particle for physical hit
                                                                                                                                                                    //dvfx.GetComponent<VisualEffect>().SetVector3("Direction", Vector3.up* (obstacle.transform.position - cartransform.position).magnitude + -(obstacle.transform.position - cartransform.position)); // Set Visual effect direction.
                                                                                                                                                                    ////Debug.LogWarning(cartransform.GetComponent<Rigidbody>().velocity.magnitude);
                                                                                                                                                                    //dvfx.GetComponent<VisualEffect>().SetFloat("Speed", cartransform.GetComponent<Rigidbody>().velocity.magnitude); // Set velocity magnitude as factor of car speed.

                    if (stats.spawner.restore)
                        stats.spawner.numEnemiesSpawned--;
                    if (!currentlyStopped && slomo)
                    {
                        StartCoroutine(HitStopCoroutine());
                    }
                    else
                    {
                        SfxManager.instance.PlaySoundAtRandom(SfxManager.SfxCategory.Splat);
                    }

                    EnemyNavmesh enemyNavmesh = obstacle.GetComponent<EnemyNavmesh>();
                    if (enemyNavmesh != null && enemyNavmesh.isSwole)
                    {
                        SfxManager.instance.PlaySoundAtRandom(SfxManager.SfxCategory.SwoleZombieDeath, enemyNavmesh.audioSource);
                    }
                    else
                    {
                        SfxManager.instance.PlaySoundAtRandom(SfxManager.SfxCategory.ZombieDeath, enemyNavmesh.audioSource);
                    }
                }
            }
        }
        else if(CarManager.Instance.carController.throttleInput > .2f)
        {
            stats.Push(999);
        }
    }
    private void OnTriggerExit(Collider other)
    {
        var obstacle = other.gameObject;
        var navmesh = obstacle.GetComponent<EnemyNavmesh>();
        if (navmesh != null)
            navmesh.insideKillboxTriggerAmt--;
    }
    IEnumerator HitStopCoroutine()
    {
        SfxManager.instance.PlaySoundAtRandom(SfxManager.SfxCategory.Splat);
        Time.timeScale = 0.0f;
        currentlyStopped = true;
        StartCoroutine(GameManager.instance.DoVibration(1f, 1f, 0.25f));
        yield return new WaitForSecondsRealtime(hitStopDuration);
        Time.timeScale = timeMultiplier;
        Time.fixedDeltaTime = 0.02F * Time.timeScale;
        yield return new WaitForSecondsRealtime(hitSlowDuration);
        Time.timeScale = 1;
        Time.fixedDeltaTime = 0.02F;
        if (enableHitStopCooldown)
        {
            yield return new WaitForSecondsRealtime(hitStopCooldownDuration - hitStopDuration - hitSlowDuration);
        }
        currentlyStopped = false;
    }
}
