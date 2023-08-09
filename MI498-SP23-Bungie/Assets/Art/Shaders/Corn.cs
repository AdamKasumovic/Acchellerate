using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Corn : MonoBehaviour
{
	public Material[] foliagemats;
	
    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
	 foreach (Material mat in foliagemats)
     mat.SetVector("_VehiclePos", this.transform.position);  
    }
}
