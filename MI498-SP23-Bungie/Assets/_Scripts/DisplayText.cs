using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DisplayText : MonoBehaviour
{
    public static string[][] flavorText;
    private TextMeshProUGUI box;
    public TextAsset dataFile;
    private void Start()
    {
        box = GetComponent<TextMeshProUGUI>();
        if (flavorText == null && dataFile != null)
        {
            flavorText = new string[99][];
            ReadData();
        }
    }
    void ReadData()
    {
        string[] lines = dataFile.text.Split('\n');
        for (int i = 0; i < lines.Length; i++)
        {
            if (i >= flavorText.Length)  // Check if current index is within bounds of flavorText
            {
                Debug.LogWarning("More lines in dataFile than expected. Consider increasing the size of flavorText.");
                break;  // exit the loop if you're trying to write beyond the end of the flavorText array
            }
            string[] data = lines[i].Trim('\r').Split(',');
            flavorText[i] = data;
        }
    }

    public void DisplayGroundPoundFlavorText()
    {
        DisplayTextbox(flavorText[0][UpgradeUnlocks.groundPoundUnlockNum]);
    }
    public void DisplayBoostFlavorText()
    {
        DisplayTextbox(flavorText[1][UpgradeUnlocks.boostUnlockNum]);
    }
    public void DisplayAirControlFlavorText()
    {
        DisplayTextbox(flavorText[2][UpgradeUnlocks.airControl]);
    }
    public void DisplayHorizontalBoostlavorText()
    {
        DisplayTextbox(flavorText[3][UpgradeUnlocks.horizontalBoosts]);
    }
    public void DisplayBarrelRollUnlockFlavorText()
    {
        DisplayTextbox(flavorText[4][UpgradeUnlocks.barrelRolls]);
    }
    public void DisplayTiltDurationFlavorText()
    {
        DisplayTextbox(flavorText[5][UpgradeUnlocks.tiltDuration]);
    }
    public void DisplayBurnoutDurationFlavorText()
    {
        DisplayTextbox(flavorText[6][UpgradeUnlocks.burnoutDuration]);
    }
    public void DisplayJumpCooldownFlavorText()
    {
        DisplayTextbox(flavorText[7][UpgradeUnlocks.jumpCooldown]);
    }
    public void DisplayBoostCooldownFlavorText()
    {
        DisplayTextbox(flavorText[8][UpgradeUnlocks.boostCooldown]);
    }
    public void DisplayFlipCooldownFlavorText()
    {
        DisplayTextbox(flavorText[9][UpgradeUnlocks.flipCooldown]);
    }
    public void DisplayBurnoutCooldownFlavorText()
    {
        DisplayTextbox(flavorText[10][UpgradeUnlocks.boostCooldown]);
    }
    public void DisplayTiltCooldownFlavorText()
    {
        DisplayTextbox(flavorText[11][UpgradeUnlocks.tiltCooldown]);
    }
    public void DisplayTextbox(string text)
    {
        if (text == "")
        {
            box.text =  "<b>Fully Upgraded</b>";
        }
        else
        {
            box.text = text;
        }
    }
}
