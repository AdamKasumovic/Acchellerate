using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PhysicalGarageEnter : MonoBehaviour
{
    LevelLoadButton llb;
    public string SceneToLoad;
    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            llb = GetComponent<LevelLoadButton>();
            llb.LoadLevel(SceneToLoad);
        }

    }
}
