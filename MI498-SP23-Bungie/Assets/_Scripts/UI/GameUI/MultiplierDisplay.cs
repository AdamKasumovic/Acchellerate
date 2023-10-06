using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
public class MultiplierDisplay : MonoBehaviour
{
    public TextMeshProUGUI display;

    // Start is called before the first frame update
    void Start()
    {
        display = GetComponent<TextMeshProUGUI>();
    }

    // Update is called once per frame
    void Update()
    {
        display.text = "" + NewStyleSystem.instance.multiplier;
    }
}
