using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class CarInputManager : MonoBehaviour
{
    #region Manager Instance
    private static CarInputManager _instance;
    public static CarInputManager Instance { get { return _instance; } }
    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            _instance = this;
        }
    }
    #endregion

    #region Input Functions
    public float horizontalTilt = 0f;
    void OnTilt(InputValue input)
    {
        if (CarManager.Instance.autoDriving)
            horizontalTilt = 0f;
        else
            horizontalTilt = input.Get<Vector2>().x;
    }

    float boostDirectionY = 0f;
    float boostDirectionX = 0f;
    void OnLook(InputValue input)
    {
        if (CarManager.Instance.autoDriving)
        {
            boostDirectionY = 0;
            boostDirectionX = 0;
        }
        else
        {
            boostDirectionY = input.Get<Vector2>().y;
            boostDirectionX = input.Get<Vector2>().x;
        }
    }

    public float horizontal = 0f;
    public float vertical = 0f;
    void OnMove(InputValue input)
    {
        if (CarManager.Instance.autoDriving)
        {
            horizontal = 0;
            vertical = 0;
        }
        else
        {
            horizontal = input.Get<Vector2>().x;
            vertical = input.Get<Vector2>().y;
        }
    }

    bool northButton = false;
    bool wasNorthButton = false;
    bool northButtonPressed = false;
    void OnNorthButton(InputValue input)
    {
        northButton = input.isPressed && !CarManager.Instance.autoDriving;
    }

    bool rightBumper = false;
    bool wasRightBumper = false;
    bool rightBumperPressed = false;
    void OnRightBumper(InputValue input)
    {
        rightBumper = input.isPressed && !CarManager.Instance.autoDriving;
    }

    public bool boost = false;
    public bool wasBoost = false;
    bool boostPressed = false;
    [HideInInspector]
    public bool boostRefreshing = false;
    public float horBoostSpeedAirborne = 78f;
    void OnBoost(InputValue input)
    {
        boost = input.isPressed && !CarManager.Instance.autoDriving;
    }

    bool leftBoost = false;
    bool wasLeftBoost = false;
    public bool leftBoostPressed = false;
    void OnLeftBoost(InputValue input)
    {
        leftBoost = input.isPressed && !CarManager.Instance.autoDriving;
    }

    bool rightBoost = false;
    bool wasRightBoost = false;
    public bool rightBoostPressed = false;
    void OnRightBoost(InputValue input)
    {
        rightBoost = input.isPressed && !CarManager.Instance.autoDriving;
    }

    public bool reverse = false;
    bool wasReverse = false;
    bool reversePressed = false;
    void OnReverse(InputValue input)
    {
        reverse = input.isPressed && !CarManager.Instance.autoDriving;
    }

    public bool rearView = false;
    void OnRearView(InputValue input)
    {
        rearView = input.isPressed && !CarManager.Instance.autoDriving;
    }

    public bool jump = false;
    public bool wasJump = false;
    public bool jumpPressed = false;
    void OnJump(InputValue input)
    {
        jump = input.isPressed && !CarManager.Instance.autoDriving;
        //Debug.Log("JUMPED!");
    }

    bool frontFlip = false;
    bool wasFrontFlip = false;
    public bool frontFlipPressed = false;
    void OnFrontFlip(InputValue input)
    {
        frontFlip = input.isPressed && !CarManager.Instance.autoDriving;
    }

    bool headlights = false;
    bool wasHeadlights = false;
    bool headlightsPressed = false;

    public Light headlightLeft;
    public Light headlightRight;
    void OnHeadlights(InputValue input)
    {
        headlights = input.isPressed && !CarManager.Instance.autoDriving;
    }

    #endregion

    #region Setup

    public void SetPressedInputs()
    {
        northButtonPressed = northButton && !wasNorthButton;
        rightBumperPressed = rightBumper && !wasRightBumper;
        boostPressed = boost && !wasBoost;
        leftBoostPressed = leftBoost && !wasLeftBoost;
        rightBoostPressed = rightBoost && !wasRightBoost;
        reversePressed = reverse && !wasReverse;
        jumpPressed = jump && !wasJump;
        frontFlipPressed = frontFlip && !wasFrontFlip;
        headlightsPressed = headlights && !wasHeadlights;
        if (headlightsPressed)
        {
            headlightLeft.enabled = !headlightLeft.enabled;
            headlightRight.enabled = !headlightRight.enabled;
            missionsComponent.RegisterHeadlights();
        }
    }

    public void LogInputs()
    {
        wasNorthButton = northButton;
        wasRightBumper = rightBumper;
        wasBoost = boost;
        wasLeftBoost = leftBoost;
        wasRightBoost = rightBoost;
        wasReverse = reverse;
        wasJump = jump;
        wasFrontFlip = frontFlip;
        wasHeadlights = headlights;
    }

    #endregion

    private Missions missionsComponent;
    private void Start()
    {
        missionsComponent = Missions.Instance;
    }
}
