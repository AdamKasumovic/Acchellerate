using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class MustangToggler : MonoBehaviour
{
    public static GameObject charger;
    public static GameObject mustang;

    // Temporary stuff for fun
    private static int wesleyCount = 0;

    // Start is called before the first frame update
    void Start()
    {
        charger = transform.GetChild(0).gameObject;
        mustang = transform.GetChild(1).gameObject;
        string carType = PlayerPrefs.GetString("CarName", "mustang");
        if (carType == "mustang")
        {
            ToggleMustang();
        }
        else if (carType == "charger")
        {
            ToggleCharger();
        }
    }

    // Update is called once per frame
    void Update()
    {
        // This is temporary, remove and just use below functions to upgrade
        if ((wesleyCount == 0 && Input.GetKeyDown(KeyCode.W)) || ((wesleyCount == 1 || wesleyCount == 4) && Input.GetKeyDown(KeyCode.E)) || (wesleyCount == 2 && Input.GetKeyDown(KeyCode.S)) || 
            (wesleyCount == 3 && Input.GetKeyDown(KeyCode.L)) || (wesleyCount == 5 && Input.GetKeyDown(KeyCode.Y)))
        {
            ++wesleyCount;
        }
        else if (Input.anyKeyDown)
        {
            wesleyCount = 0;
        }
        if (wesleyCount >= 6)
        {
            wesleyCount = 0;
            if (charger.activeInHierarchy)
                ToggleMustang();
            else
                ToggleCharger();
        }

        //Debug.Log(wesleyCount);
    }

    public static void ToggleMustang()
    {
        mustang.SetActive(true);
        charger.SetActive(false);
    }

    public static void ToggleCharger()
    {
        charger.SetActive(true);
        mustang.SetActive(false);
    }

    public static void SetCarVisual(string carName)
    {
        if (carName.ToLower() == "mustang")
        {
            PlayerPrefs.SetString("CarName", "mustang");
            ToggleMustang();
        }
        else if (carName.ToLower() == "charger")
        {
            PlayerPrefs.SetString("CarName", "charger");
            ToggleCharger();
        }
    }
}
