using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClusterFall : MonoBehaviour
{
    private PhysicsOnCarCollision[] m_Colliders;
    // Start is called before the first frame update
    void Start()
    {
        m_Colliders = GetComponentsInChildren<PhysicsOnCarCollision>();
        foreach(var collider in m_Colliders)
        {
            collider.cf = this;
        }
    }
    public void TriggerCollapse()
    {
        foreach (var collider in m_Colliders)
        {
            collider.MakeKinematic();
        }
    }
}
