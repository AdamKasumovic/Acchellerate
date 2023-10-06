using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class EnsureUISelection : MonoBehaviour
{
    [Tooltip("GameObject to have selected by default. Clicking off the buttons would make it impossible to navigate via keyboard or controller without this script.")]
    public GameObject selectedGameObject;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (EventSystem.current.currentSelectedGameObject == null)
        {
            EventSystem.current.SetSelectedGameObject(null);
            EventSystem.current.SetSelectedGameObject(selectedGameObject);
        }
        //Debug.Log(EventSystem.current.currentSelectedGameObject.name);
    }
}
