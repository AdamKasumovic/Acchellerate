using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public class EnemyNavmesh : MonoBehaviour
{
    [Header("Attributes")] [Tooltip("The speed that the zombie should go when chasing the player")]
    public float speed = 2.0f;

    [Tooltip("The speed that the zombie should move when wandering")]
    public float wanderingSpeed = 0.5f;

    [Tooltip("The distance away from the car that the zombie should stop")]
    public float stoppingDistance = 100f;

    /*[Tooltip("The distance that the zombies should stop at when heading towards each other")]
    public float zombieStoppingDistance = 3f;*/

    [Tooltip("The amount of time a zombie needs to wait before wandering to a new spot.")]
    private float zombieCooldownTime = 0.2f;

    private GameObject _entityTarget;

    private Vector3 _wanderingTarget;

    private NavMeshAgent _agent;

    private bool _touchingPlayer = false;

    [Tooltip("Time in seconds after the zombie hits the car before it will move again.")]
    private float contactCooldownThreshold = 1f;  // set to attack animation time

    public Animator animator;

    private float _contactTimer = 0f;

    [Tooltip("To prevent the car from shaking, the zombie will immediately step away after scoring a hit this many meters.")]
    public float stepBackMultiplier = 0.25f;

    private Vector3 _hitPosition; // Position of zombie at most recent car hit

    private float _cooldownStartTime;

    private float _currentCooldownTime;

    private EnemyStats _stats;

    private bool _tryWander = true;

    private bool wasAttacking = false;
    
    public int insideKillboxTriggerAmt = 0;

    public float maximumHitDistance = 5.5f;

    public float slowdownDist = 10f;

    public AudioSource audioSource;

    public bool isFlying = false;
    
    public bool isSwole = false;

    float loopTimer = 0;

    public bool inTornado = false;
    bool wasInTornado = false;

    public float speedMultiplier = 1f;

    void Awake()
    {
        _agent = GetComponent<NavMeshAgent>();
        _agent.speed = wanderingSpeed * speedMultiplier;

        NavMeshHit hit;
        if (_agent.isActiveAndEnabled && NavMesh.SamplePosition(_agent.transform.position, out hit, 1.0f, NavMesh.AllAreas))
        {
            _agent.isStopped = true;
        }

        _agent.stoppingDistance = stoppingDistance;

        _stats = GetComponent<EnemyStats>();

        _currentCooldownTime = 0;
        _cooldownStartTime = 0;

        audioSource = GetComponent<AudioSource>();
        int rand = Random.Range(1, 4);
        animator.SetInteger("AnimationVariant", rand);
        //Debug.Log($"Zombie Audio Mixer: {audioSource.outputAudioMixerGroup}");
    }

    void Update()
    {
       
        if (ZombieSpeedController.Instance != null && speedMultiplier != ZombieSpeedController.Instance.speedMultiplier)
        {
            speedMultiplier = ZombieSpeedController.Instance.speedMultiplier;
            CancelMove();
        }
        // Makes Zombies hit the griddy if carmanager boolean is set to True
        if (CarManager.Instance.griddy )
        {
            if (!animator.GetCurrentAnimatorStateInfo(0).IsName("Griddy"))
            {
                animator.SetTrigger("Griddy");
            }
            _agent.speed = 0;
        }

        else if (!isFlying)
        {
            bool isAttacking = animator.GetCurrentAnimatorStateInfo(0).IsName("Attack1") || animator.GetCurrentAnimatorStateInfo(0).IsName("Attack2Cycle");

            //if (!isAttacking)
            //{
            if (!animator.GetCurrentAnimatorStateInfo(0).IsName("OnBack") && !animator.GetCurrentAnimatorStateInfo(0).IsName("Getup") && !isAttacking)
            {
                CheckIfStuck();
                MoveToTarget();
                Wander();
            }
            if((transform.position - CarManager.Instance.gameObject.transform.position).magnitude > maximumHitDistance)
            {
                animator.SetTrigger("StopAttack");
            }
            if ((transform.position - CarManager.Instance.gameObject.transform.position).magnitude < slowdownDist)
            {
                _agent.speed = ((wanderingSpeed + speed) / 2) * speedMultiplier;
            }
            else
            {
                _agent.speed = _tryWander ? (wanderingSpeed) * speedMultiplier : (speed) * speedMultiplier;
            }

            
            if (_contactTimer > 0)
            {
                //Debug.Log("Contact timer is greater than zero.");
                // This just steps the zombie away from the car gradually
                _contactTimer -= Time.deltaTime;
                if (_entityTarget != null)
                    transform.position = Vector3.Lerp(_hitPosition, _hitPosition + (_hitPosition - _entityTarget.transform.position).normalized * stepBackMultiplier, (contactCooldownThreshold - _contactTimer) / contactCooldownThreshold);
            }
            
            /*if (isAttacking && (!wasAttacking || animator.GetCurrentAnimatorStateInfo(0).length/3 < loopTimer))
            {
                loopTimer = 0;
                StartCoroutine(DamageCoroutine(1f));
            }*/
            //}
            //else
            //{

            //CancelMove();
            //}
            wasAttacking = isAttacking;
            if (inTornado && !wasInTornado || !inTornado && wasInTornado)
            {
                transform.GetChild(0).Rotate(0, 180, 0);
            }
            wasInTornado = inTornado;
        }
    }

    void Wander()
    {
        if (_entityTarget == null && _tryWander)
        {
            if (!float.IsPositiveInfinity(_agent.remainingDistance) &&
                _agent.pathStatus == NavMeshPathStatus.PathComplete && _agent.remainingDistance == 0)
            {
                _agent.isStopped = true;
            }
            
            if (_agent.isStopped && _tryWander)
            {
                _currentCooldownTime += Time.deltaTime;
                if (_currentCooldownTime >= _cooldownStartTime + zombieCooldownTime)
                {
                    _currentCooldownTime = Time.time;
                    _cooldownStartTime = Time.time;
                
                    SetWanderTarget();
                
                    NavMeshPath path = new NavMeshPath();
                    _agent.CalculatePath(_wanderingTarget, path);
                    if (path.status == NavMeshPathStatus.PathPartial || _contactTimer > 0)
                    {
                        CancelMove();
                    }
                    else
                    {
                        _agent.SetDestination(_wanderingTarget);
                        _agent.isStopped = false;
                    }
                }
            }
        }
    }

    private void CheckIfStuck()
    {
        if (_agent.pathStatus != NavMeshPathStatus.PathComplete)
        {
            //Debug.Log("Failed");
            CancelMove();
        }
    }

    public void SetWanderTarget()
    {
        if (_stats.spawner != null)
        {
            Bounds bounds = _stats.spawner.collider.bounds;
            _wanderingTarget = new Vector3(
                Random.Range(bounds.min.x, bounds.max.x),
                transform.position.y,
                Random.Range(bounds.min.z, bounds.max.z)
            );
        }

    }
    
    // https://answers.unity.com/questions/361810/draw-path-along-navmesh-agent-path.html
    void OnDrawGizmosSelected()
    {
        if( _agent == null || _agent.path == null )
            return;
 
        var line = GetComponent<LineRenderer>();
        if( line == null )
        {
            line = gameObject.AddComponent<LineRenderer>();
            line.material = new Material( Shader.Find( "Sprites/Default" ) ) { color = Color.yellow };
            line.SetWidth( 0.5f, 0.5f );
            line.SetColors( Color.yellow, Color.yellow );
        }
 
        var path = _agent.path;
 
        line.SetVertexCount( path.corners.Length );
 
        for( int i = 0; i < path.corners.Length; i++ )
        {
            line.SetPosition( i, path.corners[i] );
        }
    }
    
    public void SetCarTarget(GameObject newTarget/*, bool isZombie*/)
    {
        _entityTarget = newTarget;
        _agent.stoppingDistance = /*isZombie ? zombieStoppingDistance : */stoppingDistance;
        NavMeshHit hit;
        if(_agent.isActiveAndEnabled && NavMesh.SamplePosition(_agent.transform.position, out hit, 1.0f, NavMesh.AllAreas))
        {
            _agent.destination = _entityTarget.transform.position; // ERROR LINE
            _agent.isStopped = false; // ERROR LINE
        }
        _tryWander = false;
        _agent.speed = speed * speedMultiplier;
    }
    
    

    public void RemoveTarget()
    {
        _entityTarget = null;
        _tryWander = true;
        NavMeshHit hit;
        if (_agent.isActiveAndEnabled && NavMesh.SamplePosition(_agent.transform.position, out hit, 1.0f, NavMesh.AllAreas))
        {
            _agent.isStopped = true; // ERROR LINE
        }
        _agent.speed = wanderingSpeed * speedMultiplier;
    }

    public void CancelMove()
    {
        NavMeshHit hit;
        if (_agent.isActiveAndEnabled && NavMesh.SamplePosition(_agent.transform.position, out hit, 1.0f, NavMesh.AllAreas))
        {
            _agent.isStopped = true;
            _agent.ResetPath();
        }
    }

    public void MoveToTarget()
    {
        if (_entityTarget != null)
        {
            NavMeshPath path = new NavMeshPath();
            _agent.CalculatePath(_entityTarget.transform.position, path);
            if (path.status == NavMeshPathStatus.PathPartial || _contactTimer > 0)
            {
                CancelMove();
            }
            else
            {
                _agent.SetDestination(_entityTarget.transform.position);
            }
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
       if (collision.gameObject.tag == "Player")
       {
            _touchingPlayer = true;
            /*if (!animator.GetCurrentAnimatorStateInfo(0).IsName("OnBack"))
            {
                if(Time.time > CarManager.lastHit + CarManager.hitStun)
                {
                    animator.SetTrigger("Attack");
                }
            }*/
            
            _contactTimer = contactCooldownThreshold;
            _hitPosition = transform.position;
       }
    }
    private void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            _touchingPlayer = false;
        }
    }
    IEnumerator DamageCoroutine(float hitTime)
    {
        yield return new WaitForSeconds(hitTime);
    }
}
