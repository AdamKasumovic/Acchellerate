using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClusterFall : MonoBehaviour
{
    private PhysicsOnCarCollision[] m_Colliders;
    Missions missions;
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
        if(missions == null)
        {
            missions = Missions.Instance;
            Missions.Instance.RegisterDestruction();
        }
        foreach (var collider in m_Colliders)
        {
            collider.MakeKinematic();
        }
    }
}
