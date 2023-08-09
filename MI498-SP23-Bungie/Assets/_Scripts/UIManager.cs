using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour
{
    [Header("Page Management")]
    [Tooltip("The list of pages being used by the game")]
    public List<UIPage> pages;
    [Tooltip("The page in the list being used currently")]
    public int currentPage = 0;
    [Tooltip("The page in the list to be used by default")]
    public int defaultPage = 0;

    [Header("Pausing")]
    [Tooltip("The index that the pause page is set to")]
    public int pauseIndex = 1;
    
    [HideInInspector]
    public bool paused = false;

    [Header("Cooldown UI Management")] 
    [Tooltip("The fill used for Jump Smash")]
    public Image jumpSmashFill;
    [Tooltip("The fill used for Flip")]
    public Image flipFill;
    [Tooltip("The fill used for Tilt")]
    public Image tiltFill;
    [Tooltip("The fill used for Donut")]
    public Image donutFill;
    [Tooltip("The fill used for Side Swipe")]
    public Image sideSwipeFill;

    [Tooltip("In Use Color")]
    public Color activeColor;

    private float tiltCooldownTimer;

    private bool doingTiltTimer = false;

    private bool wasCanTilt = true;
    
    // Start is called before the first frame update
    void Awake()
    {
        SetUpEventSystem();
        SetUpUIElements();
        UpdateUI();
        if (SceneManager.GetActiveScene().name == "MainMenu")
        {
            Time.timeScale = 1;
            Time.fixedDeltaTime = 0.02f;
        }
    }

    // Update is called once per frame
    void Update()
    {
        UpdateUI();
        //AudioListener.pause = currentPage != defaultPage;
        if (CarManager.Instance != null)
            wasCanTilt = CarManager.Instance.canTilt;
    }
    
    [HideInInspector]
    public EventSystem eventSystem;
    private void SetUpEventSystem()
    {
        eventSystem = FindObjectOfType<EventSystem>();
        if (eventSystem == null)
        {
            Debug.LogError("There is no event system found in the scene.");
        }
    }

    private List<UIElement> uiElements;
    private void SetUpUIElements()
    {
        uiElements = FindObjectsOfType<UIElement>().ToList();
    }

    
    public void GoToPage(int index)
    {
        if (index < pages.Count && pages[index] != null)
        {
            // Set all active pages to false
            SetAllActivePages(false);
            // Set the desired page to true
            pages[index].gameObject.SetActive(true);
            currentPage = index;
            // Set selected UI to default using a function in UIPage
        }
    }
    
    
    public void GoToPageByName(string name)
    {
        // Find the page by its name
        UIPage page = pages.Find(item => item.name == name);
        int pageIndex = pages.IndexOf(page);
        GoToPage(pageIndex);
        ChangeButton(page);
    }

    public void ChangeButton(UIPage page)
    {
        Button button = page.gameObject.GetComponentInChildren<Button>();
        if (button != null)
        {
            GameObject buttonChild = button.gameObject;
            //Debug.Log(buttonChild.name);
            eventSystem.SetSelectedGameObject(buttonChild);
        }
    }

    public void SetAllActivePages(bool active)
    {
        // Make sure that each respective page as well as the list itself isn't null.
        // Then set all pages to the specified variable
        if (pages != null)
        {
            foreach (UIPage page in pages)
            {
                if (page != null)
                {
                    page.gameObject.SetActive(active);
                }
            }
        }
    }
    
    public void UpdateUI()
    {
        // Update the UI for every single UI element
        foreach(UIElement element in uiElements)
        {
            element.UpdateUI();
        }

        bool carJumped = false;
        if (CarManager.Instance != null)
            carJumped = CarManager.Instance.jumped;
        if (jumpSmashFill != null)
        {
            if (UpgradeUnlocks.groundPoundUnlockNum > 0)
            {
                jumpSmashFill.color = carJumped ? activeColor : Color.white;
                jumpSmashFill.transform.parent.GetChild(3).gameObject.SetActive(!carJumped);
                jumpSmashFill.transform.parent.GetChild(4).gameObject.SetActive(carJumped);
            }

            donutFill.color = CarManager.Instance.isSpinning ? activeColor : Color.white;

            tiltFill.color = (CarManager.currentState == CarManager.CarState.TiltingLeft || CarManager.currentState == CarManager.CarState.TiltingRight) ? activeColor : Color.white;

            jumpSmashFill.fillAmount =
                Mathf.Lerp(0, 1, CarManager.Instance.vertBoostTimer / CarManager.Instance.vertBoostCooldown);
            flipFill.fillAmount =
                Mathf.Lerp(0, 1, CarManager.Instance.frontFlipTimer / CarManager.Instance.frontFlipCooldown);
            donutFill.fillAmount =
                Mathf.Lerp(0, 1, CarManager.Instance.spinCooldownTimer / CarManager.Instance.spinCooldown);
            sideSwipeFill.fillAmount =
                Mathf.Lerp(0, 1, CarManager.Instance.horBoostTimer / CarManager.Instance.horBoostDuration);

            if (!CarManager.Instance.canTilt && wasCanTilt)
            {
                doingTiltTimer = true;
            }
            else if (CarManager.Instance.canTilt && !wasCanTilt)
            {
                doingTiltTimer = false;
                tiltCooldownTimer = 0f;
            }
            if (doingTiltTimer)
            {
                tiltCooldownTimer += Time.deltaTime;
                tiltFill.fillAmount =
                    Mathf.Lerp(0, 1, tiltCooldownTimer / CarManager.Instance.tiltCooldown);
            }
        }
    }
    
    public void HandlePause()
    {
        GoToPage(paused ? defaultPage : pauseIndex);
        ChangeButton(pages[paused ? defaultPage : pauseIndex]);
        paused = !paused;
        //Debug.Log("Current page: " + currentPage);
    }
}
