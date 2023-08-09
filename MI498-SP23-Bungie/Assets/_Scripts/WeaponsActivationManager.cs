using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponsActivationManager : MonoBehaviour
{
    public enum WeaponLoactions { Front, Hood, Top, Sides, Engine, Fuel }
    public enum WeaponTypes { Chainsaws, Spikes, Guns, Flamethrowers, Turrets, Boost, Engine, Fuel, Nitro }
    public Dictionary<WeaponLoactions, Dictionary<WeaponTypes, Dictionary<int, GameObject>>> allWeapons;
    public Dictionary<WeaponLoactions, Dictionary<WeaponTypes, Dictionary<int, GameObject>>> ownedWeapons;
    List<GameObject> deactivatedUpgrades;
    List<GameObject> activatedUpgrades;

    // Start is called before the first frame update
    public static WeaponsActivationManager Instance { get; private set; }
    void Start()
    {
        allWeapons = new Dictionary<WeaponLoactions, Dictionary<WeaponTypes, Dictionary<int, GameObject>>>();
        ownedWeapons = new Dictionary<WeaponLoactions, Dictionary<WeaponTypes, Dictionary<int, GameObject>>>();
        deactivatedUpgrades = new List<GameObject>();
        Instance = this;
        WeaponTyping [] data = GetComponentsInChildren<WeaponTyping>();
        foreach(WeaponTyping weapon in data)
        {
            if (allWeapons.ContainsKey(weapon.location))
            {
                if (!allWeapons[weapon.location].ContainsKey(weapon.type))
                {
                    allWeapons[weapon.location][weapon.type] = new Dictionary<int, GameObject>();
                }
            }
            else
            {
                allWeapons[weapon.location] = new Dictionary<WeaponTypes, Dictionary<int, GameObject>>();
                allWeapons[weapon.location][weapon.type] = new Dictionary<int, GameObject>();
            }
            if (weapon.gameObject.activeSelf)
            {
                ownedWeapons[weapon.location][weapon.type][weapon.level] = weapon.gameObject;
            }
            allWeapons[weapon.location][weapon.type][weapon.level] = weapon.gameObject;
        }
    }

    // Update is called once per frame
    void Update()
    {
    }
    public Dictionary<WeaponTypes, Dictionary<int, GameObject>> GetObjectsByLocation(WeaponLoactions location)
    {
        return allWeapons[location];
    }
    public void SelectUpgrade(WeaponLoactions loc, WeaponTypes type, int level)
    {
        RevertToEntryConfig();
        foreach(var weapon in allWeapons[loc][type])
        {
            weapon.Value.SetActive(false);
        }
        allWeapons[loc][type][level].SetActive(true);
    }
    public bool BuyUpgrade(WeaponLoactions loc, WeaponTypes type, int level)
    {
        var weaponType = allWeapons[loc][type][level].GetComponent<WeaponTyping>();
        if (weaponType.cost > CarManager.numPoints)
        {
            return false;
        }
        CarManager.numPoints -= weaponType.cost;
        RevertToEntryConfig();

        foreach (var weapon in allWeapons[loc][type])
        {
            if (weapon.Value.activeSelf)
            {
                //weapon.Value.Deactivate(); TODO: Implement Make weapons do stuff
                ownedWeapons[loc][type].Remove(weapon.Key);
                weapon.Value.SetActive(false);
            }
        }
        //allWeapons[loc][type][level].Activate(); TODO: Make weapons do stuff
        ownedWeapons[loc][type][level] = allWeapons[loc][type][level];
        allWeapons[loc][type][level].SetActive(true);
        weaponType.cost = 0;
        return true;
    }
    public void RevertToEntryConfig()
    {
        foreach (var location in allWeapons)
        {
            foreach(var type in location.Value)
            {
                foreach(var level in type.Value)
                {
                    level.Value.SetActive(ownedWeapons[location.Key][type.Key].ContainsKey(level.Key));
                }

            }
        }
    }
    public void HandleLeavingShop()
    {
        RevertToEntryConfig();
    }
}
