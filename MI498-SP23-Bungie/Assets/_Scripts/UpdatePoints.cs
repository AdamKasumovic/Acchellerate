using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class UpdatePoints : MonoBehaviour
{
    TextMeshProUGUI points;
    // Start is called before the first frame update
    void Start()
    {
        points = GetComponent<TextMeshProUGUI>();
    }

    // Update is called once per frame
    void Update()
    {
        points.text = "Souls: " + UpgradeUnlocks.credits;
    }
}
