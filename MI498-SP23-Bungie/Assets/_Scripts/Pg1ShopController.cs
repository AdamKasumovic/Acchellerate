using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Pg1ShopController : MonoBehaviour
{
    private GasStationManager gasStationManager;
    private TextMeshProUGUI healthDisplay;
    private TextMeshProUGUI pointsCounterDisplay;
    private TextMeshProUGUI fuelDisplay;
    private TextMeshProUGUI maxGasDisplay;
    private TextMeshProUGUI maxHealthDisplay;
    private TextMeshProUGUI refuelButtonDisplay;
    private TextMeshProUGUI healButtonDisplay;
    private TextMeshProUGUI gasUpgradeButtonDisplay;
    private TextMeshProUGUI healthUpgradeButtonDisplay;

    // Start is called before the first frame update
    void Start()
    {
        gasStationManager = transform.gameObject.GetComponent<GasStationManager>();
        if (gasStationManager == null)
        {
            Debug.Log("No Gas Station Manager found in scene");
        }
        healthDisplay = transform.Find("Health").gameObject.GetComponent<TextMeshProUGUI>();
        if (healthDisplay == null)
        {
            Debug.Log("No Health Display found in scene");
        }
        pointsCounterDisplay = transform.Find("PointsCounter").gameObject.GetComponent<TextMeshProUGUI>();
        if (pointsCounterDisplay == null)
        {
            Debug.Log("No Points Display found in scene");
        }
        fuelDisplay = transform.Find("Fuel Level").gameObject.GetComponent<TextMeshProUGUI>();
        if (fuelDisplay == null)
        {
            Debug.Log("No Fuel Display found in scene");
        }
        maxGasDisplay = transform.Find("MaxFuel").gameObject.GetComponent<TextMeshProUGUI>();
        if (maxGasDisplay == null)
        {
            Debug.Log("No Max Gas Display found in scene");
        }
        maxHealthDisplay = transform.Find("MaxHealth").gameObject.GetComponent<TextMeshProUGUI>();
        if (maxHealthDisplay == null)
        {
            Debug.Log("No Max Health Display found in scene");
        }
        refuelButtonDisplay = transform.Find("Refuel/Refuel Button").gameObject.GetComponent<TextMeshProUGUI>();
        if (refuelButtonDisplay == null)
        {
            Debug.Log("No refuel button found in scene");
        }
        gasUpgradeButtonDisplay = transform.Find("UpgradeFuel/Fuel Upgrade Button").gameObject.GetComponent<TextMeshProUGUI>();
        if (gasUpgradeButtonDisplay == null)
        {
            Debug.Log("No gas upgrade button found in scene");
        }
        healButtonDisplay = transform.Find("Repair/Repair Button").gameObject.GetComponent<TextMeshProUGUI>();
        if (healButtonDisplay == null)
        {
            Debug.Log("No heal button found in scene");
        }
        healthUpgradeButtonDisplay = transform.Find("UpgradeHealth/Health Upgrade Button").gameObject.GetComponent<TextMeshProUGUI>();
        if (healthUpgradeButtonDisplay == null)
        {
            Debug.Log("No health upgrade button found in scene");
        }
    }

    // Update is called once per frame
    void Update()
    {
        pointsCounterDisplay.text = "Current Points:\n" + $"{CarManager.numPoints:0.}";
        healthDisplay.text = "Health Left:\n" + $"{CarManager.carHealth:0.}" + "/" + CarManager.carMaxHealth;
        fuelDisplay.text = "Fuel Left:\n" + $"{CarManager.carGas:0.0}" + "/" + CarManager.carMaxGas;
        maxGasDisplay.text = "Max Fuel:\n" + $"{CarManager.carMaxGas:0.}";
        maxHealthDisplay.text = "Max Health:\n" + $"{CarManager.carMaxHealth:0.}";
        refuelButtonDisplay.text = "Refuel\nCost: " + $"{gasStationManager.GasCost():0.}";
        healButtonDisplay.text = "Repair\nCost: " + $"{gasStationManager.HealthCost():0.}"; 
        gasUpgradeButtonDisplay.text = "Upgrade\nCost: " + $"{gasStationManager.UpgradeGasCost():0.}" + "\nGives: " + gasStationManager.gasUpgradeAmount;
        healthUpgradeButtonDisplay.text = "Upgrade\nCost: " + $"{gasStationManager.UpgradeHealthCost():0.}" + "\nGives: " + gasStationManager.healthUpgradeAmount;
        
    }

}
