using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using Cinemachine;

public class ResetGarage : MonoBehaviour
{
    bool wasPressedLastFrame = false;
    public GameObject defaultGameObject;
    public GameObject[] vcamObjects;
    public GameObject[] boards;
    private CinemachineVirtualCamera[] vcam;
    public GameObject[] videoPlayers;
    public GameObject[] buttons;
    GameObject lastSelected;
    bool inMenu = false;
    // Start is called before the first frame update
    void Start()
    {
        vcam = new CinemachineVirtualCamera[vcamObjects.Length];
        for(int i = 0; i < vcamObjects.Length; i++)
        {
            vcam[i] = vcamObjects[i].GetComponent<CinemachineVirtualCamera>();
        }
        Cancel();
    }

    // Update is called once per frame
    void Update()
    {
        if(!wasPressedLastFrame && (CarManager.Instance.carController.handbrakeInput > 0 || Input.GetKeyDown(KeyCode.Escape)) && inMenu)
        {
            inMenu = false;
            Cancel();
        }
        wasPressedLastFrame = (CarManager.Instance.carController.handbrakeInput > 0 || Input.GetKeyDown(KeyCode.Escape));
    }
    public void SetObject(GameObject g)
    {
        foreach (var board in boards)
        {
            var buttons = board.GetComponentsInChildren<Button>();
            foreach(var button in buttons)
            {
                button.enabled = true;
            }
        }
        foreach (GameObject button in buttons)
        {
            button.SetActive(false);
        }

        GameManager.instance.uiManager.eventSystem.SetSelectedGameObject(null);
        GameManager.instance.uiManager.eventSystem.SetSelectedGameObject(g);
    }
    public void Cancel()
    {
        for (int i = 0; i < vcam.Length; i++)
        {
            vcam[i].m_Priority = 9;
        }
        foreach (var board in boards)
        {
            var buttons = board.GetComponentsInChildren<Button>();
            foreach (var button in buttons)
            {
                button.enabled = false;
            }
        }

        foreach (GameObject button in buttons)
        {
            button.SetActive(true);
        }

        foreach (var vidPlayer in videoPlayers)
        {
            var pv = vidPlayer.GetComponent<PlayVideo>();
            pv.EndVid();
        }
        GameManager.instance.uiManager.eventSystem.SetSelectedGameObject(null);
        if (lastSelected == null)
        {
            GameManager.instance.uiManager.eventSystem.SetSelectedGameObject(defaultGameObject);
        }
        else
        {
            GameManager.instance.uiManager.eventSystem.SetSelectedGameObject(lastSelected);
        }
    }
    public void SetLastObject(GameObject game)
    {
        inMenu = true;
        lastSelected = game;
    }
}
