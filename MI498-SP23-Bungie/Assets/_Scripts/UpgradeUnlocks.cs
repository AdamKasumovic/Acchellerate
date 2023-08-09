using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UpgradeUnlocks : MonoBehaviour
{
    // All upgrades are additive, of course :)
    // The higher the number, the farther along the unlock
    public static int groundPoundUnlockNum = 0;  // 0 is no ground pound, 1 is ground pound unlocked, 2 is tornado unlocked, 3 is max tornado duration
    public static int boostUnlockNum = 0;  // 0 is no boost move combinations, 1 is boost while tilting, 2 is boost in midair
    public static int airControl = 0;  // 0 is no air control, 1 is half-as-good air control, 2 is best air control
    public static int horizontalBoosts = 0;  // Horizontal boosts unlocked? (0 or 1)
    public static int barrelRolls = 0;  // Barrel rolls (sideways flips) unlocked? (0 or 1)
    public static int tiltDuration = 0;  // 0 is lowest duration, 1 is middle duration, 2 is longest duration
    public static int burnoutDuration = 0;  // 0 is lowest duration, 1 is middle duration, 2 is longest duration
    public static int jumpCooldown = 0;  // 0 is longest cooldown, 1 is middle cooldown, 2 is lowest cooldown
    public static int boostCooldown = 0;  // 0 is longest cooldown, 1 is middle cooldown, 2 is lowest cooldown
    public static int flipCooldown = 0;  // 0 is longest cooldown, 1 is middle cooldown, 2 is lowest cooldown
    public static int burnoutCooldown = 0;  // 0 is longest cooldown, 1 is middle cooldown, 2 is lowest cooldown
    public static int tiltCooldown = 0;  // 0 is longest cooldown, 1 is middle cooldown, 2 is lowest cooldown
    public static int credits = 0;

    // Map of upgrade names to maximum allowed integer, MUST match PlayerPrefs keys!
    private Dictionary<string, int> validUpgrades = new Dictionary<string, int>() { { "groundPoundUnlockNum", 3 },
                                                                                    { "boostUnlockNum", 2 },
                                                                                    { "airControl", 2 },
                                                                                    { "horizontalBoosts", 1 },
                                                                                    { "barrelRolls", 1 },
                                                                                    { "tiltDuration", 2 },
                                                                                    { "burnoutDuration", 2 },
                                                                                    { "jumpCooldown", 2 },
                                                                                    { "boostCooldown", 2 },
                                                                                    { "flipCooldown", 2 },
                                                                                    { "burnoutCooldown", 2 },
                                                                                    { "tiltCooldown", 2 } };

    private void Awake()
    {
        QueryUpgrades();
        QueryCredits();
    }

    // The variables are used for actually applying the upgrades in CarManager, 
    // while PlayerPrefs is used for saving upgrades, applying upgrades, and checking if upgrades are valid.
    // This function should be called whenever an upgrade or reset happens so the CarManager knows.
    public static void QueryUpgrades()
    {
        groundPoundUnlockNum = PlayerPrefs.GetInt("groundPoundUnlockNum", 0);
        boostUnlockNum = PlayerPrefs.GetInt("boostUnlockNum", 0);
        airControl = PlayerPrefs.GetInt("airControl", 0);
        horizontalBoosts = PlayerPrefs.GetInt("horizontalBoosts", 0);
        barrelRolls = PlayerPrefs.GetInt("barrelRolls", 0);
        tiltDuration = PlayerPrefs.GetInt("tiltDuration", 0);
        burnoutDuration = PlayerPrefs.GetInt("burnoutDuration", 0);
        jumpCooldown = PlayerPrefs.GetInt("jumpCooldown", 0);
        boostCooldown = PlayerPrefs.GetInt("boostCooldown", 0);
        flipCooldown = PlayerPrefs.GetInt("flipCooldown", 0);
        burnoutCooldown = PlayerPrefs.GetInt("burnoutCooldown", 0);
        tiltCooldown = PlayerPrefs.GetInt("tiltCooldown", 0);
    }

    private int SumUpgrades()
    {
        return PlayerPrefs.GetInt("credits", 0) +
        PlayerPrefs.GetInt("groundPoundUnlockNum", 0) +
        PlayerPrefs.GetInt("boostUnlockNum", 0) +
        PlayerPrefs.GetInt("airControl", 0) +
        PlayerPrefs.GetInt("horizontalBoosts", 0) +
        PlayerPrefs.GetInt("barrelRolls", 0) +
        PlayerPrefs.GetInt("tiltDuration", 0) +
        PlayerPrefs.GetInt("burnoutDuration", 0) +
        PlayerPrefs.GetInt("jumpCooldown", 0) +
        PlayerPrefs.GetInt("boostCooldown", 0) +
        PlayerPrefs.GetInt("flipCooldown", 0) +
        PlayerPrefs.GetInt("burnoutCooldown", 0) +
        PlayerPrefs.GetInt("tiltCooldown", 0);
    }

    public void ResetUpgrades()  // call this from a reset button
    {
        PlayerPrefs.SetInt("credits", SumUpgrades());
        QueryCredits();

        PlayerPrefs.SetInt("groundPoundUnlockNum", 0);
        PlayerPrefs.SetInt("boostUnlockNum", 0);
        PlayerPrefs.SetInt("airControl", 0);
        PlayerPrefs.SetInt("horizontalBoosts", 0);
        PlayerPrefs.SetInt("barrelRolls", 0);
        PlayerPrefs.SetInt("tiltDuration", 0);
        PlayerPrefs.SetInt("burnoutDuration", 0);
        PlayerPrefs.SetInt("jumpCooldown", 0);
        PlayerPrefs.SetInt("boostCooldown", 0);
        PlayerPrefs.SetInt("flipCooldown", 0);
        PlayerPrefs.SetInt("burnoutCooldown", 0);
        PlayerPrefs.SetInt("tiltCooldown", 0);
        QueryUpgrades();
    }

    public static void QueryCredits()
    {
        credits = PlayerPrefs.GetInt("credits", 0);
    }

    // Use return value of this function to see if upgrade succeeded
    // Actually upgrade value of false used to gray out upgrade boxes if invalid on UI load based on return value of this function
    public bool UpgradeByName(string name, bool actuallyUpgrade = false)  // Not static since this script will go on an object in scene and this function will be used by upgrade buttons
    {
        if (!validUpgrades.ContainsKey(name))  // If upgrade name invalid...
            return false;
        int upgradeValue = PlayerPrefs.GetInt(name, 0);
        if (upgradeValue >= validUpgrades[name])  // If upgrade already at max...
            return false;
        QueryCredits();
        if (credits < 1)  // If no credits...
            return false;

        if (actuallyUpgrade)
        {
            PlayerPrefs.SetInt(name, upgradeValue + 1);  // Upgrade!
            PlayerPrefs.SetInt("credits", credits - 1);  // Deduct a credit

            QueryUpgrades();  // Update static vars, CarManager will take the values and apply them to the car
            QueryCredits();
        }
        return true;
    }
    public void ActuallyUpgradeByName(string name) 
    {
        UpgradeByName(name, true);
    }


    public static void AddCredits(int numToAdd)
    {
        QueryCredits();
        PlayerPrefs.SetInt("credits", credits + numToAdd);
        QueryCredits();
    }
}
