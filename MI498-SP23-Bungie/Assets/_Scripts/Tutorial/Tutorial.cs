using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using Cinemachine;

public class Tutorial : TutorialManager
{
    // Put any necessary objects here
    public GameObject tutorialCanvas;
    private List<TutorialText> popupTexts = new List<TutorialText>();
    private float timer = 0f;
    public static bool[] enteredAreas = new bool[100];
    public GameObject cooldownUI;
    public GameObject pointsDisplay1;
    public GameObject pointsDisplay2;
    public GameObject firstAreaBarrier;
    public GameObject movementAreaBarrier;
    public GameObject[] speedometerUI = new GameObject[9];
    public GameObject firstZombieAreaBarrier;
    public GameObject finalAreaBarrier;
    public GameObject endBarrier;
    public GameObject hud;
    public GameObject zombieSpawns1;
    public GameObject zombieSpawn2;
    public Vector3 introCutsceneStart = new Vector3(-26.399999618530275f, 2.630000114440918f, 1859.699951171875f);
    public CinemachineFreeLook cfl;
    public GameObject[] remainingUI = new GameObject[7];
    public HUDUIManager huduiManager;
    public GameObject powerupPedestal;
    public BoxCollider gasStation;

    void Start()
    {
        //clickSound = GameObject.Find("LevelEssentials/SoundBank/UI/Special3").GetComponent<AudioSource>();
        for (int i = 0; i < tutorialCanvas.transform.childCount; i++)
        {
            popupTexts.Add(tutorialCanvas.transform.GetChild(i).Find("Text").GetComponent<TutorialText>());  // Must name text components "Text"!
        }
        timer = 0f;

        enteredAreas = new bool[100];
    }

    // Update is called once per frame
    override protected void Update()
    {
        base.Update();
        try
        {
            // Example from USJ
            //if (popupIndex == 0)
            //{
            //    if (GameController.gameState != GameController.GameState.NotStarted && timer >= 0.25f)
            //    {
            //        GameController.p1.transform.Find("PivotPoint").GetComponent<CapsuleCollider>().enabled = false;
            //        MoveOn();
            //        timer = 0f;
            //    }
            //    else
            //    {
            //        timer += Time.deltaTime;
            //    }
            //}
            //else if (popupIndex == 1 || popupIndex == 6 || popupIndex == 9 || popupIndex == 12 || popupIndex == 13 || popupIndex == 18)
            //{
            //    Time.timeScale = 0f;
            //    if (popupTexts[popupIndex].finished && (Input.GetKeyDown(KeyCode.Return) || Input.GetButtonDown("Submit")))
            //    {
            //        MoveOn();
            //    }
            //}
            //else if (popupIndex == 2)
            //{
            //    Time.timeScale = 0f;
            //    if (popupTexts[popupIndex].finished && (Input.GetKeyDown(KeyCode.Return) || Input.GetButtonDown("Submit")))
            //    {
            //        MoveOn();
            //        marker1.SetActive(true);
            //        Time.timeScale = 1f;
            //    }
            //}
            //else if (popupIndex == 3)
            //{
            //    if (timer >= 0.25f)
            //    {
            //        MoveOn();
            //        timer = 0f;
            //        marker1.SetActive(false);
            //    }
            //    else if ((GameController.p1.transform.position - marker1.transform.position).magnitude <= 3f)
            //    {
            //        timer += Time.deltaTime;
            //    }
            //}
            //else if (popupIndex == 4 || popupIndex == 7 || popupIndex == 10 || popupIndex == 23)
            //{
            //    Time.timeScale = 0f;
            //    if (popupTexts[popupIndex].finished && (Input.GetKeyDown(KeyCode.Return) || Input.GetButtonDown("Submit")))
            //    {
            //        MoveOn();
            //        Time.timeScale = 1f;
            //    }
            //}
            //else if (popupIndex == 5)
            //{
            //    if (!hasSpun && GameController.p1.GetComponent<Unicycle>().spinAround)
            //    {
            //        hasSpun = true;
            //    }
            //    if (timer >= 3f)
            //    {
            //        MoveOn();
            //        timer = 0f;
            //    }
            //    else if (hasSpun)
            //    {
            //        timer += Time.deltaTime;
            //        popupTexts[popupIndex].GetComponent<TextMeshProUGUI>().color = new Color32(43, 186, 141, 255);
            //    }
            //}
            //else if (popupIndex == 8)
            //{
            //    if (!hasDashed && GameController.p1.GetComponent<Unicycle>().dashing)
            //    {
            //        hasDashed = true;
            //    }
            //    if (timer >= 3f)
            //    {
            //        MoveOn();
            //        timer = 0f;
            //    }
            //    else if (hasDashed)
            //    {
            //        timer += Time.deltaTime;
            //        popupTexts[popupIndex].GetComponent<TextMeshProUGUI>().color = new Color32(43, 186, 141, 255);
            //    }
            //}
            //else if (popupIndex == 11)
            //{
            //    if (!hasDodged && GameController.p1.GetComponent<Unicycle>().dodging)
            //    {
            //        hasDodged = true;
            //    }
            //    if (timer >= 3f)
            //    {
            //        MoveOn();
            //        timer = 0f;
            //    }
            //    else if (hasDodged)
            //    {
            //        timer += Time.deltaTime;
            //        popupTexts[popupIndex].GetComponent<TextMeshProUGUI>().color = new Color32(43, 186, 141, 255);
            //    }
            //}
            //else if (popupIndex == 14)
            //{
            //    Time.timeScale = 0f;
            //    if (popupTexts[popupIndex].finished && (Input.GetKeyDown(KeyCode.Return) || Input.GetButtonDown("Submit")))
            //    {
            //        MoveOn();
            //        marker2.SetActive(true);
            //        Time.timeScale = 1f;
            //        GameController.p1.transform.position = blueSpawn.transform.position;
            //    }
            //}
            //else if (popupIndex == 15)
            //{
            //    if (marker2.activeInHierarchy && timer >= 0.25f)
            //    {
            //        timer = 0f;
            //        marker2.SetActive(false);
            //        marker3.SetActive(true);
            //    }
            //    else if (!marker2.activeInHierarchy && timer >= 0.25f)
            //    {
            //        MoveOn();
            //        timer = 0f;
            //        marker3.SetActive(false);
            //    }
            //    if (marker2.activeInHierarchy && (GameController.p1.transform.position - marker2.transform.position).magnitude <= 3f)
            //    {
            //        timer += Time.deltaTime;
            //    }
            //    else if (!marker2.activeInHierarchy && (GameController.p1.transform.position - marker3.transform.position).magnitude <= 3f && marker3.activeInHierarchy)
            //    {
            //        timer += Time.deltaTime;
            //    }
            //}
            //else if (popupIndex == 16)
            //{
            //    Time.timeScale = 0f;
            //    if (popupTexts[popupIndex].finished && (Input.GetKeyDown(KeyCode.Return) || Input.GetButtonDown("Submit")))
            //    {
            //        MoveOn();
            //        Time.timeScale = 1f;
            //        Transform dummy = GameController.p2.transform;
            //        dummy.Find("VFX").gameObject.SetActive(true);
            //        dummy.Find("Canvas").gameObject.SetActive(true);
            //        dummy.Find("PivotPoint").GetComponent<CapsuleCollider>().enabled = true;
            //        Transform dummyVisuals = dummy.Find("PivotPoint/RedMan");
            //        foreach (Transform dvc in dummyVisuals)
            //        {
            //            dvc.gameObject.SetActive(true);
            //        }
            //    }
            //}
            //else if (popupIndex == 17)
            //{
            //    if (timer >= 3f)
            //    {
            //        MoveOn();
            //        timer = 0f;
            //    }
            //    else if (timer >= 2f)
            //    {
            //        Transform dummy = GameController.p2.transform;
            //        Transform dummyVisuals = dummy.Find("PivotPoint/RedMan");
            //        dummyVisuals.position = new Vector3(GameController.p2.transform.position.x, 1000000, GameController.p2.transform.position.z);
            //        dummy.Find("VFX").gameObject.SetActive(false);
            //        dummy.Find("PivotPoint").GetComponent<CapsuleCollider>().enabled = false;
            //        dummy.Find("Canvas").gameObject.SetActive(false);
            //        timer += Time.deltaTime;
            //    }
            //    else if (GameController.p2.hp < 2)
            //    {
            //        timer += Time.deltaTime;
            //        popupTexts[popupIndex].GetComponent<TextMeshProUGUI>().color = new Color32(43, 186, 141, 255);
            //    }
            //}
            //else if (popupIndex == 19)
            //{
            //    Time.timeScale = 0f;
            //    if (popupTexts[popupIndex].finished && (Input.GetKeyDown(KeyCode.Return) || Input.GetButtonDown("Submit")))
            //    {
            //        MoveOn();
            //        present1.SetActive(true);
            //        present2.SetActive(true);
            //        Time.timeScale = 1f;
            //    }
            //}
            //else if (popupIndex == 20)
            //{
            //    if (timer >= 5f)
            //    {
            //        MoveOn();
            //        timer = 0f;
            //    }
            //    else if (present1 == null && present2 == null)
            //    {
            //        timer += Time.deltaTime;
            //        popupTexts[popupIndex].GetComponent<TextMeshProUGUI>().color = new Color32(43, 186, 141, 255);
            //    }
            //}
            //else if (popupIndex == 21)
            //{
            //    Time.timeScale = 0f;
            //    if (popupTexts[popupIndex].finished && (Input.GetKeyDown(KeyCode.Return) || Input.GetButtonDown("Submit")))
            //    {
            //        MoveOn();
            //        present3.SetActive(true);
            //        Time.timeScale = 1f;
            //    }
            //}
            //else if (popupIndex == 22)
            //{
            //    if (timer >= 10f)
            //    {
            //        MoveOn();
            //        timer = 0f;
            //    }
            //    else if (present3 == null && !GameController.p1.GetComponent<Unicycle>().hasStars)
            //    {
            //        timer += Time.deltaTime;
            //        popupTexts[popupIndex].GetComponent<TextMeshProUGUI>().color = new Color32(43, 186, 141, 255);
            //    }
            //}
            //else
            //{
            //    if (Input.GetKeyDown(KeyCode.Return) || Input.GetButtonDown("Submit"))
            //    {
            //        SceneManager.LoadScene("MainMenu");
            //    }
            //}
            if (popupIndex == 0)
            {
                if (timer >= 1)
                {
                    MoveOn();
                    timer = 0f;
                }
                timer += Time.deltaTime;
            }
            else if (popupIndex == 1)
            {
                if (timer >= 1 && enteredAreas[0])
                {
                    MoveOn();
                    timer = 0f;
                }
                if (popupTexts[popupIndex].finished)
                {
                    timer += Time.deltaTime;
                }
            }
            else if (popupIndex == 2)
            {
                if (timer >= 1 && enteredAreas[1])
                {
                    MoveOn();
                    timer = 0f;
                }
                if (popupTexts[popupIndex].finished)
                {
                    timer += Time.deltaTime;
                }
            }
            else if (popupIndex == 3)
            {
                if (timer >= 1 && enteredAreas[2])
                {
                    MoveOn();
                    timer = 0f;
                }
                if (popupTexts[popupIndex].finished)
                {
                    timer += Time.deltaTime;
                }
            }
            else if (popupIndex == 4)
            {
                if (timer >= 1 && enteredAreas[3])
                {
                    MoveOn();
                    timer = 0f;
                }
                if (popupTexts[popupIndex].finished)
                {
                    timer += Time.deltaTime;
                }
            }
            else if (popupIndex == 5)
            {
                if (timer >= 1 && enteredAreas[4])
                {
                    firstAreaBarrier.SetActive(false);
                    movementAreaBarrier.SetActive(true);
                    MoveOn();
                    timer = 0f;
                }
                if (popupTexts[popupIndex].finished)
                {
                    timer += Time.deltaTime;
                }
            }
            else if (popupIndex == 6)
            {
                if (timer >= 1 && CarManager.numPoints >= 5000)
                {
                    movementAreaBarrier.SetActive(false);
                    MoveOn();
                    timer = 0f;
                }
                if (popupTexts[popupIndex].finished)
                {
                    timer += Time.deltaTime;
                }
            }
            else if (popupIndex == 7)
            {
                if (timer >= 1 && enteredAreas[5])
                {
                    movementAreaBarrier.SetActive(true);
                    firstZombieAreaBarrier.SetActive(true);
                    MoveOn();
                    timer = 0f;
                }
                if (popupTexts[popupIndex].finished)
                {
                    timer += Time.deltaTime;
                }
            }
            else if (popupIndex == 8)
            {
                if (timer >= 10)
                {
                    zombieSpawns1.SetActive(true);
                    MoveOn();
                    timer = 0f;
                }
                timer += Time.deltaTime;
            }
            else if (popupIndex == 9)
            {
                if (timer >= 1 && CarManager.numPoints >= 50000)
                {
                    firstZombieAreaBarrier.SetActive(false);
                    zombieSpawns1.SetActive(false);
                    GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
                    foreach (GameObject enemy in enemies)
                        Destroy(enemy);
                    MoveOn();
                    timer = 0f;
                }
                if (popupTexts[popupIndex].finished)
                {
                    timer += Time.deltaTime;
                }
            }
            else if (popupIndex == 10)
            {
                if (timer >= 1 && enteredAreas[6])
                {
                    CarManager.numPoints = 9999999;
                    movementAreaBarrier.SetActive(false);
                    //firstZombieAreaBarrier.SetActive(true);
                    finalAreaBarrier.SetActive(false);
                    hud.SetActive(false);
                    CarManager.Instance.rb.velocity = Vector3.zero;
                    CarManager.Instance.rb.angularVelocity = Vector3.zero;
                    CarManager.Instance.transform.position = introCutsceneStart;
                    CarManager.Instance.AutoDrive(true);
                    cfl.m_Priority = 10000;
                    MoveOn();
                    timer = 0f;
                }
                if (popupTexts[popupIndex].finished)
                {
                    timer += Time.deltaTime;
                }
            }
            else if (popupIndex >= 11 && popupIndex <= 14)
            {
                if (timer >= 3)
                {
                    if (popupIndex == 14)
                    {
                        CarManager.Instance.AutoDrive(false);
                        CarManager.numPoints = 0;
                        hud.SetActive(true);
                        cfl.m_Priority = -1000;
                    }   
                    MoveOn();
                    timer = 0f;
                }
                timer += Time.deltaTime;
            }
            else if (popupIndex == 15)
            {
                if (timer >= 10)
                {
                    zombieSpawn2.SetActive(true);
                    foreach (GameObject go in remainingUI)
                    {
                        go.SetActive(true);
                    }
                    huduiManager.infiniteGas = false;
                    LevelTimer.initTme = 60;
                    LevelTimer.numSeconds = 60;
                    MoveOn();
                    timer = 0f;
                }
                timer += Time.deltaTime;
            }
            else if (popupIndex == 16)
            {
                if (LevelTimer.numSeconds <= 30)
                {
                    powerupPedestal.SetActive(true);
                    MoveOn();
                    timer = 0f;
                }
                timer += Time.deltaTime;
            }
            else if (popupIndex == 17)
            {
                if (powerupPedestal.GetComponent<PedestalDecals>().collected)
                {
                    //powerupPedestal.SetActive(false);
                    zombieSpawn2.GetComponent<Spawner>().restore = false;
                    MoveOn();
                    timer = 0f;
                }
                timer += Time.deltaTime;
            }
            else if (popupIndex == 18)
            {
                if (LevelTimer.numSeconds < 2)
                {
                    gasStation.enabled = true;
                    endBarrier.SetActive(false);
                    UpgradeUnlocks.AddCredits(1);
                    MoveOn();
                    timer = 0f;
                }
                timer += Time.deltaTime;
            }
        }
        catch
        {
            Debug.LogError("Tutorial Errors Happening!");
        }
        if (!cooldownUI.activeInHierarchy && enteredAreas[1])
            cooldownUI.SetActive(true);
        if (GameManager.instance.gameState == GameManager.GameStates.play && !pointsDisplay1.activeInHierarchy && enteredAreas[4] && !enteredAreas[6])
        {
            CarManager.numPoints = 0;
            pointsDisplay1.SetActive(true);
            pointsDisplay2.SetActive(true);
        }
        if (!speedometerUI[0].activeInHierarchy && enteredAreas[5])
        {
            foreach (GameObject go in speedometerUI)
            {
                go.SetActive(true);
            }
        }
        if (!finalAreaBarrier.activeInHierarchy && enteredAreas[7])
        {
            finalAreaBarrier.SetActive(true);
            endBarrier.SetActive(true);
        }
    }
}
