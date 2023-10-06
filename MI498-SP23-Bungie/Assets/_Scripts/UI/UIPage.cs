using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UIPage : MonoBehaviour
{
    [Tooltip("The default gameobject to be selected when this page is brought up.")]
    public GameObject defaultGameObject;

    public void SetAsDefaultGameObject()
    {
        // Set the default object using the UI manager's event system.
        if (defaultGameObject != null && GameManager.instance != null && GameManager.instance.uiManager != null)
        {
            GameManager.instance.uiManager.eventSystem.SetSelectedGameObject(null);
            GameManager.instance.uiManager.eventSystem.SetSelectedGameObject(defaultGameObject);
        }
    }

    private void OnEnable()
    {
        SetAsDefaultGameObject();
    }

    void Update()
    {
        if (EventSystem.current.currentSelectedGameObject == null || !EventSystem.current.currentSelectedGameObject.activeInHierarchy)
        {
            EventSystem.current.SetSelectedGameObject(null);
            EventSystem.current.SetSelectedGameObject(defaultGameObject);
        }
        //Debug.Log(EventSystem.current.currentSelectedGameObject.name);
    }
}