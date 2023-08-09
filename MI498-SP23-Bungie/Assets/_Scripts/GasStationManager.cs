using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class GasStationManager : MonoBehaviour
{

    public float firstGasUpgradeCost = 1;
    public float firstSpeedUpgradeCost = 1;
    public float firstHealthUpgradeCost = 1;

    public float gasUpgradeAmount = 1;
    public float speedUpgradeAmount = 1;
    public float healthUpgradeAmount = 1;

    public float gasUpgradeCostScaling = 2;
    public float speedUpgradeCostScaling = 2;
    public float healthUpgradeCostScaling = 2;
    
    public float healthCostPerUnit = .5f;
    public float gasCostPerUnit = .5f;

    [Tooltip("Transform of where to put the car after leaving the gas station.")]
    public Transform spawnPoint;
    
    private float numGasUpgradesPurchased = 0;
    private float numSpeedUpgradesPurchased = 0;
    private float numHealthUpgradesPurchased = 0;

    public static bool inGasStation = false;

    [Header("Cinemachine Cameras -- For when players view upgrades.")]
    [Tooltip("This camera will be active when no upgrade is selected and the car is rotating. Otherwise, the car will not be rotating and a camera getting a good shot of the upgrade to add will be active.")]
    public CinemachineVirtualCamera defaultGarageCamera;
    // Start is called before the first frame update
    void Start()
    {
        inGasStation = false;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void RefillGas()
    {
        if (CarManager.numPoints >= GasCost())
        {
            CarManager.numPoints -= GasCost();
            CarManager.Instance.SetGas(CarManager.carMaxGas);
        }
    }

    public void RefillHealth()
    {
        if (CarManager.numPoints >= HealthCost())
        {
            CarManager.numPoints -= HealthCost();
            CarManager.carHealth = CarManager.carMaxHealth;
            CarManager.Instance.VisualRepair();
        }
    }

    public void UpgradeGas()
    {
        if(CarManager.numPoints >= UpgradeGasCost())
        {
            CarManager.numPoints -= UpgradeGasCost();
            CarManager.Instance.UpgradeGas(gasUpgradeAmount);
            numGasUpgradesPurchased++;
        }

    }
    public void UpgradeHealth()
    {
        if (CarManager.numPoints >= UpgradeHealthCost())
        {
            CarManager.numPoints -= UpgradeHealthCost();
            CarManager.carMaxHealth += healthUpgradeAmount;
            CarManager.carHealth += healthUpgradeAmount;
            numHealthUpgradesPurchased++;
        }
    }
    public void UpgradeSpeed()
    {
        if (CarManager.numPoints >= UpgradeSpeedCost())
        {
            CarManager.numPoints -= UpgradeSpeedCost();
            CarManager.Instance.UpgradeSpeed(speedUpgradeAmount);
            numSpeedUpgradesPurchased++;
        }

    }
    public float GasCost() {
        return Mathf.Ceil((CarManager.carMaxGas - CarManager.carGas) * gasCostPerUnit);
    }
    public float HealthCost()
    {
        return Mathf.Ceil((CarManager.carMaxHealth - CarManager.carHealth) * healthCostPerUnit);
    }
    public float UpgradeSpeedCost()
    {
        return Mathf.Ceil((numSpeedUpgradesPurchased * speedUpgradeCostScaling + firstSpeedUpgradeCost));

    }
    public float UpgradeHealthCost()
    {
        return Mathf.Ceil(numHealthUpgradesPurchased * healthUpgradeCostScaling + firstHealthUpgradeCost);

    }
    public float UpgradeGasCost()
    {
        return Mathf.Ceil(numGasUpgradesPurchased * gasUpgradeCostScaling + firstGasUpgradeCost);

    }

    // LEAVE SHOP
    public void ResetCar()  // Place car somewhere after upgrading
    {
        AkSoundEngine.PostEvent("MusicEvent", AudioManager.instance.gameObject.GetComponentInChildren<AkAmbient>().gameObject);
        CarManager.Instance.transform.position = spawnPoint.transform.position;
        CarManager.Instance.transform.rotation = spawnPoint.transform.rotation;
        Rigidbody rigidbody = CarManager.Instance.GetComponent<Rigidbody>();
        rigidbody.isKinematic = false;
        rigidbody.velocity = Vector3.zero;
        rigidbody.angularVelocity = Vector3.zero;
        CarManager.Instance.carController.enabled = true;
        CarManager.Instance.carController.useFuelConsumption = true;
        CarManager.Instance.carController.speed = 0;
        defaultGarageCamera.m_Priority = 0;
        inGasStation = false;
    }
}
