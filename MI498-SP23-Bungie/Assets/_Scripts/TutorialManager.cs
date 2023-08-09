using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;

public abstract class TutorialManager : MonoBehaviour
{
    public GameObject[] popups;
    protected int popupIndex = 0;
    public InputAction moveOn;
    protected bool moveOnPressed = false;
    public AudioSource onTutorialProgress;
    public AudioClip onTutorialProgressClip;
    //public AudioSource clickSound;
    // Start is called before the first frame update
    private void Awake()
    {
        moveOn.Enable();
    }
    void Start()
    {
    }

    // Update is called once per frame
    virtual protected void Update()
    {
        moveOnPressed = moveOn.triggered;
        if (popupIndex < popups.Length && moveOnPressed)
        {
            SkipText();
        }
        for (int i = 0; i < popups.Length; i++)
        {
            if (i == popupIndex)
            {
                //if (!GameManager.GameisOver)
                    popups[i].SetActive(true);
                //else
                //    popups[i].SetActive(false);
            }
            else
            {
                popups[i].SetActive(false);
            }
        }
    }

    public void MoveOn()
    {
        ++popupIndex;
        onTutorialProgress.PlayOneShot(onTutorialProgressClip);
        //clickSound.ignoreListenerPause = true;
        //clickSound.Play();
    }

    public void SkipText()
    {
        TutorialText tt = FindObjectOfType<TutorialText>();
        if (tt.enabled && !tt.finished)
        {
            tt.StopCoroutine("PlayText");
            tt.transparencyIndex = tt.originalContentLength;
            tt.text.text = tt.text.text.Replace("<color=#" + ColorUtility.ToHtmlStringRGB(tt.text.color) + "00>", "").Insert(tt.transparencyIndex, "<color=#" + ColorUtility.ToHtmlStringRGB(tt.text.color) + "00>");
        }
    }

}
