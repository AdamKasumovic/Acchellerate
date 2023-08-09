using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UnlockImageChanger : MonoBehaviour
{
    public Sprite highImage;
    public Sprite lowImage;
    public ImageSetter[] setters;
    private void Start()
    {
        setters = GetComponentsInChildren<ImageSetter>();
    }
    private void Update()
    {
        setters[0].UpdateImage(UpgradeUnlocks.jumpCooldown, highImage, lowImage);
        setters[1].UpdateImage(UpgradeUnlocks.boostCooldown, highImage, lowImage);
        setters[2].UpdateImage(UpgradeUnlocks.flipCooldown, highImage, lowImage);
        setters[3].UpdateImage(UpgradeUnlocks.burnoutCooldown, highImage, lowImage);
        setters[4].UpdateImage(UpgradeUnlocks.tiltCooldown, highImage, lowImage);
        setters[5].UpdateImage(UpgradeUnlocks.groundPoundUnlockNum, highImage, lowImage);
        setters[6].UpdateImage(UpgradeUnlocks.boostUnlockNum, highImage, lowImage);
        setters[7].UpdateImage(UpgradeUnlocks.airControl, highImage, lowImage);
        setters[8].UpdateImage(UpgradeUnlocks.horizontalBoosts, highImage, lowImage);
        setters[9].UpdateImage(UpgradeUnlocks.barrelRolls,highImage, lowImage);
        setters[10].UpdateImage(UpgradeUnlocks.tiltDuration,highImage, lowImage);
        setters[11].UpdateImage(UpgradeUnlocks.burnoutDuration, highImage, lowImage);
        
    }

}
