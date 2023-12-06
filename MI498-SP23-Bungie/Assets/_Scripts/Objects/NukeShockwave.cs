using System.Collections;
using UnityEngine;
using Gaia;

public class NukeShockwave : MonoBehaviour
{
    public float shockwaveSpeed = 20;
    public float timeBeforeZombiesSpawnAgain = 10;
    private void Start()
    {
        // VFX/SFX here

        Collider[] hitColliders = Physics.OverlapSphere(transform.position, 1000000f);
        foreach (var hitCollider in hitColliders)
        {
            if (hitCollider.enabled && hitCollider.tag == "Enemy")
            {
                StartCoroutine(KillAfterTime(shockwaveSpeed, Vector3.Distance(transform.position, hitCollider.transform.position), hitCollider));
            }
        }

        StartCoroutine(ResetNuke());
    }

    private IEnumerator KillAfterTime(float divisorDelay, float distance, Collider collider)
    {
        yield return new WaitForSeconds(distance / divisorDelay);
        collider.GetComponent<EnemyStats>().Damage(999);
        NewStyleSystem.instance.AddCarKill();
    }

    private IEnumerator ResetNuke()
    {
        yield return new WaitForSeconds(timeBeforeZombiesSpawnAgain);
        GaiaAPI.StopWeatherRain();
        NukeManager.Instance.nukeActive = false;
        Destroy(gameObject);
    }
}
