using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyStats : MonoBehaviour
{
    public float health = 40;
    public int pointReward = 1;
    public float damage = 5;
    public Spawner spawner;
    public bool canEnable = true;
    public bool isEnabled = true;
    public bool insidePlayer = false;
    public float ragdollDuration = 1.2f;
    public float forceThreshold = 5.0f;
    private float timer = 0f;
    public float hitStun = .5f;
    public float getUpTimer = 0f;
    private bool downed = false;
    public bool isSwole = false;
    public MeshRenderer mr;
    private SkinnedMeshRenderer[] smrs;
    private bool flashing = false;
    private Animator animator;
    public bool waitingForGetup;
    public float totalForce;
    float forceTimer;
    public float forceTimerMax = 1.0f;
    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponentInChildren<Animator>();
        smrs = GetComponentsInChildren<SkinnedMeshRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        timer = Mathf.Max(timer - Time.deltaTime, 0f);
        if (canEnable && !isEnabled && !insidePlayer && health > 0)
        {
            enable();
            isEnabled = true;
        }
        if (canEnable && !insidePlayer && mr.enabled && !isEnabled)
            getUpTimer -= Time.deltaTime;
        if(getUpTimer <= -5)
        {
            animator.SetTrigger("Uppies");
            downed = false;
            getUpTimer = 0;
            waitingForGetup = false;
        }
        if (getUpTimer <= 0 && health > 0 && downed && !waitingForGetup)
        {
            waitingForGetup = true;
            StartCoroutine(GetupDelay());
        }
        forceTimer -= Time.deltaTime;
        if(forceTimer < 0)
        {
            totalForce = 0;
        }
    }
    public int Damage(float damageTaken)
    {
        if (!enabled)
        {
            return 0;
        }
        StartCoroutine(Flasher());
        if(timer > 0)
        {
            return 0;
        }
        health -= damageTaken;
        if (health <= 0)
        {

            bool frontFlipKill = CarManager.Instance.frontFlipKillIndicator;
            bool tiltKill = CarManager.Instance.tiltKillIndicator;
            bool groundPoundKill = CarManager.Instance.groundPoundKillIndicator;
            bool strafeKill = CarManager.Instance.horBoostKillIndicator;
            bool burnoutKill = CarManager.Instance.isSpinning;
            bool driftKill;
            if (!(frontFlipKill || tiltKill || groundPoundKill || strafeKill || burnoutKill) && (CarManager.currentState == CarManager.CarState.DriftingLeft || CarManager.currentState == CarManager.CarState.DriftingRight))
            {
                driftKill = true;
            }
            else
            {
                driftKill = false;
            }
            EnemyType thisEnemyType = EnemyType.regular;
            if (isSwole)
            {
                CarManager.Instance.swoleKill = true;
                thisEnemyType = EnemyType.swole;
            }

            Missions missionsComponent = Missions.Instance;
            if (frontFlipKill)
                missionsComponent.RegisterKill(thisEnemyType, KillType.frontFlip);
            if (tiltKill)
                missionsComponent.RegisterKill(thisEnemyType, KillType.tilt);
            if (groundPoundKill)
                missionsComponent.RegisterKill(thisEnemyType, KillType.groundPound);
            if (strafeKill)
                missionsComponent.RegisterKill(thisEnemyType, KillType.strafe);
            if (burnoutKill)
                missionsComponent.RegisterKill(thisEnemyType, KillType.burnout);
            if (driftKill)
                missionsComponent.RegisterKill(thisEnemyType, KillType.drift);
            if (!(frontFlipKill || tiltKill || groundPoundKill || strafeKill || burnoutKill || driftKill))
                missionsComponent.RegisterKill(thisEnemyType, KillType.driving);

            //CarManager.numPoints += pointReward;
            // PUT THINGS THAT SHOULD BE DISABLED ON DEATH HERE
            //Destroy(gameObject.transform.GetChild(1).gameObject);
            //Destroy(gameObject.transform.GetChild(2).gameObject);
            GetComponent<EnemyNavmesh>().enabled = false;
            GetComponent<NavMeshAgent>().enabled = false;
            GetComponent<CapsuleCollider>().enabled = false;
            GetComponent<BoxCollider>().enabled = false;
            // END SECTION
            //StartCoroutine(GameManager.instance.DoVibration(0.5f, 0.5f, 0.25f));
            CameraShaker.Instance.ShakeCamera(2, 0.1f);

            gameObject.transform.GetChild(0).GetComponent<ZombieRagdollHandler>().EnableRagdoll(true,true, CarManager.Instance.shouldGib && !CarManager.Instance.groundPoundKillIndicator);
            return pointReward;
        }
        return 0;
    }
    public void Push(Vector3 force, bool getUpWhenDone = false)
    {
        gameObject.GetComponent<Rigidbody>().AddForce(force/Time.deltaTime,ForceMode.Impulse);
        if(Vector3.Magnitude(force) > forceThreshold)
        {
            StartCoroutine(RagdollCoroutine(getUpWhenDone));
        }
    }
    public void Push(float force, bool getUpWhenDone = false)
    {
        if(force < .5f)
        {
            return;
        }
        totalForce += force;
        forceTimer = forceTimerMax;
        if (totalForce > forceThreshold)
        {
            StartCoroutine(RagdollCoroutine(getUpWhenDone));
        }
    }
    IEnumerator RagdollCoroutine(bool getUpWhenDone = false)
    {
        if (isEnabled)
        {
            canEnable = false;
            isEnabled = false;
            insidePlayer = true;
            getUpTimer = 8f + Random.Range(-1f, 1f);  // set to getup animation time
            disable();
            yield return new WaitForSeconds(ragdollDuration);
            canEnable = true;
            //if (getUpWhenDone)
                //enable();
        }


    }
    IEnumerator GetupDelay()
    {
        yield return new WaitForSeconds(Random.Range(0f, 2f));
        waitingForGetup = false;
        if (getUpTimer <= 0 && health > 0 && downed)
        {
            enableMovement();
            animator.SetTrigger("GetUp");
            downed = false;
        }
    }

    private void disable()
    {
        downed = true;
        gameObject.GetComponent<MeshRenderer>().enabled = false;
        gameObject.GetComponent<CapsuleCollider>().enabled = false;
        gameObject.GetComponent<BoxCollider>().enabled = true;
        gameObject.GetComponent<EnemyNavmesh>().enabled = false;
        gameObject.GetComponent<NavMeshAgent>().enabled = false;
        gameObject.transform.GetChild(0).GetComponent<ZombieRagdollHandler>().EnableRagdoll();

    }
    public void enable()
    {
        gameObject.transform.GetChild(0).GetComponent<ZombieRagdollHandler>().DisableRagdoll();
        gameObject.GetComponent<MeshRenderer>().enabled = true;
        gameObject.GetComponent<CapsuleCollider>().enabled = true;
        gameObject.GetComponent<BoxCollider>().enabled = false;
    }

    public void enableMovement()
    {
        gameObject.GetComponent<EnemyNavmesh>().enabled = true;
        gameObject.GetComponent<NavMeshAgent>().enabled = true;
    }

    IEnumerator Flasher()
    {
        if (!flashing)
        {
            flashing = true;
            SfxManager.instance.PlaySoundAtRandom(SfxManager.SfxCategory.CarHitZombie);
            for (int i = 0; i < smrs.Length; i++)
            {
                smrs[i].material.color = Color.red;
            }
            yield return new WaitForSeconds(.25f);
            for (int i = 0; i < smrs.Length; i++)
            {
                smrs[i].material.color = Color.white;
            }
            yield return new WaitForSeconds(.25f);
            flashing = false;
        }
    }

}
