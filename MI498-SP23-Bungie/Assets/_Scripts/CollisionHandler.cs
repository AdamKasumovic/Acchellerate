using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class CollisionHandler : MonoBehaviour
{
    public float shopCoolDownTimer = 5f;
    private float timer;
    public Transform garageLocation;
    public CinemachineVirtualCamera defaultGarageCamera;
    public GameObject[] DisableThese;
    // Start is called before the first frame update
    void Start()
    {
        AkSoundEngine.StopAll();
        GameManager.instance.HandleMenu("GaragePage");
        CarManager.Instance.carController.useFuelConsumption = false;
        CarManager.Instance.carController.enabled = false;
        CarManager.Instance.GetComponent<Rigidbody>().isKinematic = true;
        CarManager.Instance.transform.position = garageLocation.position;
        CarManager.Instance.transform.rotation = Quaternion.identity;
        //defaultGarageCamera.m_Priority = 1000;
        GasStationManager.inGasStation = true;
        foreach(GameObject g in DisableThese)
        {
            if (g!=null)
                g.SetActive(false);
        }
        timer = 0f;
    }

    // Update is called once per frame
    void Update()
    {
        timer += Time.deltaTime;
    }
    void OnTriggerEnter(Collider other)
    {
        // GOTO SHOP
        if (other.gameObject.layer == 8 && timer >= shopCoolDownTimer)
        {
            AkSoundEngine.StopAll();
            GameManager.instance.HandleMenu("GaragePage");
            CarManager.Instance.carController.useFuelConsumption = false;
            CarManager.Instance.carController.enabled = false;
            CarManager.Instance.GetComponent<Rigidbody>().isKinematic = true;
            CarManager.Instance.transform.position = garageLocation.position;
            CarManager.Instance.transform.rotation = Quaternion.identity;
            defaultGarageCamera.m_Priority = 1000;
            GasStationManager.inGasStation = true;
            timer = 0f;
        }
      
    }
}
