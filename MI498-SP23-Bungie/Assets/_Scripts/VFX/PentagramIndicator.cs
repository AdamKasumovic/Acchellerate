using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class PentagramIndicator : MonoBehaviour
{
    public GameObject pentagramDecal;
    public DecalProjector proj;
    public float rotationsPerSecond = 5f;
    private void Start()
    {
        proj = pentagramDecal.GetComponent<DecalProjector>();
    }
    void FixedUpdate()
    {
        if(UpgradeUnlocks.groundPoundUnlockNum > 0)
        {
            int layerMask = 1 << 0;

            RaycastHit hit;
            // Does the ray intersect any objects excluding the player layer
            if (Physics.Raycast(transform.position, Vector3.down, out hit, Mathf.Infinity, layerMask))
            {
                float size = 2 * Mathf.Clamp(CarManager.Instance.shockRadiusScaling * hit.distance, CarManager.Instance.minShockRadius, CarManager.Instance.maxShockRadius);
                proj.size = new Vector3(size, size, 2);
                pentagramDecal.transform.position = hit.point + Vector3.up * pentagramDecal.transform.localScale.y / 4;
                pentagramDecal.transform.forward = hit.normal;
                pentagramDecal.transform.Rotate(0, 0, rotationsPerSecond * Time.time * 360);

            }
        }

    }

    private void Update()
    {
        pentagramDecal.SetActive(!CarManager.Instance.carController.isGrounded);
    }
}
