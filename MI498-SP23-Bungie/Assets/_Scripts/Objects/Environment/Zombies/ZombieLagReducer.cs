using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class ZombieLagReducer : MonoBehaviour
{
    public List<Behaviour> disableTheseComponents;
    public List<GameObject> disableTheseGameObjects;
    [Tooltip("Threshold for which to despawn zombies in meters.")]
    public float despawnThreshold = 150f;
    public float alwaysRenderedThreshold = 25f;
    private MeshRenderer mRenderer;
    private float currentDistance = 0;
    private CarManager cmInstance;
    private EnemyStats thisEnemyStats;
    private CapsuleCollider thisCC;
    private Animator anim;
    private EnemyNavmesh nm;
    public float timeInBewteenChecks = .1f;
    private float timer = 0;
    private bool disabled = false;
    // Start is called before the first frame update
    void Start()
    {
        cmInstance = CarManager.Instance;
        thisEnemyStats = GetComponent<EnemyStats>();
        thisCC = GetComponent<CapsuleCollider>();
        mRenderer = GetComponent<MeshRenderer>();
        anim = GetComponentInChildren<Animator>();
    }

    // Update is called once per frame
    void Update()
    {

        currentDistance = (cmInstance.transform.position - transform.position).magnitude;
        Vector3 positionInCamera = Camera.main.WorldToViewportPoint(transform.position);
        bool visibleByCamera = positionInCamera.x >= -0.2 && positionInCamera.x <= 1.2 && positionInCamera.y >= -0.2 && positionInCamera.y <= 1.2 && positionInCamera.z > -0.2;
        if (disabled && thisEnemyStats.getUpTimer >-5)
        {
            thisEnemyStats.getUpTimer -= Time.deltaTime;
        }
        if ((currentDistance > despawnThreshold || !visibleByCamera) && thisEnemyStats.health >= 1)
        {
            if (!disabled)
            {
                disabled = true;
                DisableObjects();
            }

        }
        else if (thisEnemyStats.health > 0)// && thisEnemyStats.canEnable && !thisEnemyStats.insidePlayer && thisEnemyStats.getUpTimer <= 0)
        {
            if (disabled)
            {
                disabled = false;
                EnableObjects();
            }
        }

        if (NukeManager.Instance.nukeActive)
        {
            thisEnemyStats.enabled = true;
        }


    }

    void DisableObjects()
    {
        thisEnemyStats.enabled = false;
        mRenderer.enabled = false;
        thisCC.enabled = false;
        anim.enabled = false;
        foreach (var d in disableTheseComponents)
        {
            d.enabled = false;
        }

        foreach (GameObject g in disableTheseGameObjects)
        {
            if (g != null)
                g.SetActive(false);
        }
    }

    void EnableObjects()
    {
        thisEnemyStats.enabled = true;
        mRenderer.enabled = true;
        anim.enabled=true;
        bool isAttacking = anim.GetCurrentAnimatorStateInfo(0).IsName("Attack1") || anim.GetCurrentAnimatorStateInfo(0).IsName("Attack2Cycle");
        if (isAttacking)
        {
            anim.SetTrigger("StopAttack");
        }
        if (thisEnemyStats.isEnabled && !thisEnemyStats.waitingForGetup)
        {
            thisCC.enabled = true;
            foreach (var d in disableTheseComponents)
            {
                d.enabled = true;
            }
        }

        foreach (GameObject g in disableTheseGameObjects)
        {
            g.SetActive(true);
        }
    }
}
