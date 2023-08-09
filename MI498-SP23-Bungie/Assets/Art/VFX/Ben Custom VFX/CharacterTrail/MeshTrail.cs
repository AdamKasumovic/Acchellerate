using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

public class MeshTrail : MonoBehaviour
{
    public float activeTime = 2f;

    [Header("Mesh Related")]
    public float meshRefreshRate = 0.1f;
    public float meshDestroyDelay = 3f;
    public Transform positionToSpawn;
    public Transform rotationToFollow;

    [Header("Shader Related")]
    public Material mat;
    public string shaderVarRef;
    public float shaderVarRate = 0.1f;
    public float shaderVarRefreshRate = 0.05f;

    [Header("VFX Related")]
    public VisualEffect vfxGraphParticlesTrail;
    public VisualEffect vfxGraphInitialImpact;


    private SkinnedMeshRenderer[] skinnedRenderers;
    private bool isTrailActive;

    private void Start()
    {
        vfxGraphParticlesTrail.Stop();
        vfxGraphInitialImpact.Stop();
    }

    void Update()
    {
        vfxGraphInitialImpact.transform.SetPositionAndRotation(rotationToFollow.position, rotationToFollow.rotation);
    }

    public void DoFlashTrails()
    {
        if (!isTrailActive)
        {
            isTrailActive = true;
            StartCoroutine(ActivateTrail(activeTime));
        }
    }

    IEnumerator ActivateTrail(float timeActivated)
    {
        vfxGraphParticlesTrail.Play();
        vfxGraphInitialImpact.Play();

        while (timeActivated > 0 && !CarManager.Instance.gotHitRecently)
        {
            timeActivated -= meshRefreshRate;

            if (skinnedRenderers == null)
                skinnedRenderers = positionToSpawn.GetComponentsInChildren<SkinnedMeshRenderer>();

            for (int i = 0; i < skinnedRenderers.Length; i++)
            {
                GameObject gObj = new GameObject();
                gObj.AddComponent<DestroyWhenCarHit>();
                gObj.transform.SetPositionAndRotation(positionToSpawn.position-Vector3.up*0.22f, rotationToFollow.rotation);
                if (CarManager.currentState == CarManager.CarState.TiltingLeft || CarManager.currentState == CarManager.CarState.TiltingRight)
                {
                    gObj.transform.position = rotationToFollow.position - Vector3.up * 0.22f;
                }
                gObj.transform.localScale = new Vector3(0.82f, 0.65f, 0.62f);
                MeshRenderer mr = gObj.AddComponent<MeshRenderer>();
                mr.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
                MeshFilter mf = gObj.AddComponent<MeshFilter>();

                Mesh m = new Mesh();
                skinnedRenderers[i].BakeMesh(m);
                mf.mesh = m;
                mr.material = mat;

                //StartCoroutine(AnimateMaterialFloat(mr.material, 0, shaderVarRate, shaderVarRefreshRate));

                Destroy(gObj, meshDestroyDelay);
            }
            yield return new WaitForSeconds(meshRefreshRate);
        }

        vfxGraphParticlesTrail.Stop();
        vfxGraphInitialImpact.Stop();
        isTrailActive = false;
    }

    IEnumerator AnimateMaterialFloat(Material m, float valueGoal, float rate, float refreshRate)
    {
        float valueToAnimate = m.GetFloat(shaderVarRef);

        while (valueToAnimate > valueGoal)
        {
            valueToAnimate -= rate;
            m.SetFloat(shaderVarRef, valueToAnimate);
            yield return new WaitForSeconds(refreshRate);
        }
    }

}
