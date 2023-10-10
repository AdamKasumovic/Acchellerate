using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using UnityEngine.VFX;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class CarManager : MonoBehaviour
{
    [HideInInspector]
    public RCC_CarControllerV3 carController;

    // DriftingLeft means the car is steering left while drifting, so the right side of the car should be damaging and vice versa
    public enum CarState { Idle, Destroyed, Forward, Backward, DriftingLeft, DriftingRight, TiltingLeft, TiltingRight };
    public static CarState currentState = CarState.Idle;

    public static float carMaxHealth = 50; // Current Max Health posted to all scripts at all times
    public static float carMaxSpeed = 50;  // Current Max Speed posted to all scripts at all times
    public static float carMaxGas; // Current Max Gas amount posted to all scripts at all times
    public static float carHealth = 50;  // Health posted to all scripts at all times
    public static float carSpeed;  // Speed posted to all scripts at all times, can be used to calculate damages to zombies and car
    public static float carGas; // Gas posted to all scripts at all times
    public static float numPoints = 0; // Total points gathered posted to all scripts at all times
    public static float pointMultiplier = 1f;
    public static float speedBuff = 0f;
    public static float speedDamageFactor = .5f; // Scale factor to transform speed to how much damage the car takes colliding with obstacles posted to all scripts at all times 
    public static Vector3 heading; // Forward direction of car at all times
    public static float hitStun = .25f;
    public static float lastHit;

    [Tooltip("Time before health begins to regenerate in seconds.")]
    public float timeBeforeHealthRegeneration = 3f;

    [Tooltip("Health regeneration rate.")]
    public float regenerationRate = 45f / 7f;

    [Header("State Threshold Numbers")]
    [Tooltip("Speed before being considered 'moving' (not Idle) in kmh.")]
    public float notIdleThreshold = 5f;
    public float damageThreshold = 4f;

    [Tooltip("When airborne, the speed at which steering input rotates the car along the Z-axis (think about twisting your hand like a drill)")]
    public float airborneRollSpeed = 20f;

    [Tooltip("When airborne, the speed at which steering input rotates the car along the X-axis (think about bending your wrist down and up)")]
    public float airbornePitchSpeed = 20f;

    [Tooltip("Speed at which to rotate when in the garage (sign controls direction)")]
    public float rotationSpeed = 5f;

    [Tooltip("Speed the car should be going to change the music state. If the car is near enemies, it will always play every instrument.")]
    public float speedToChangeMusic = 50f;

    [Tooltip("Speed requirement for tilting.")]
    public float minimumSpeedForTilting = 100f;

    [Tooltip("Maximum time one can tilt in seconds.")]
    public float maximumTimeTilting = 2f;
    private float tiltTimer = 0f;

    [Tooltip("Tilt cooldown in seconds")]
    public float tiltCooldown = 2f;
    public bool canTilt = true;

    [Tooltip("Additional gravity to apply. This is not a multiplier.")]
    public float additionalGravity = 9.8f;

    [Tooltip("Strength of horizontal boosts.")]
    public float horBoostStrength = 30f;

    [Tooltip("Horizontal boost DURATION in seconds.")]
    public float horBoostDuration = 2f;
    [Tooltip("Horizontal boost recharge in seconds.")]
    public float horBoostRecharge = 2f;
    public float horBoostTimer = 0f;

    [Tooltip("Strength of vertical boosts.")]
    public float vertBoostStrength = 10f;
    [Tooltip("Ground pound shockwave knockdown radius.")]
    public float shockRadius = 10f;
    public float shockRadiusScaling = 10f;
    public float maxShockRadius = 20f;
    public float minShockRadius = 5f;
    [Tooltip("Whether or not shockwave kills (knocks down otherwise).")]
    public bool shockKills = true; 
    [HideInInspector]
    public bool jumped = false;
    private bool wasGrounded = true;

    [HideInInspector]
    public float vertBoostCooldown = 2f;
    public float vertBoostTimer = 0f;

    [Tooltip("Toggles automatic burnouts.")]
    public bool autoSpinning = true;
    [Tooltip("Time after mashing before you have to start holding the button. This is effectively a minimum spinning time.")]
    public float minimumSpinningTime = 1.5f;
    // Idea is: Hit drift button twice within 0.5 seconds while drifting to activate burnout
    private float autoSpinTotalTime = 0.5f;
    private float autoSpinTimer = 0f;
    private int driftHitCount = 0;
    private bool autoSpinActivated = false;
    private bool wasDriftPressed = false;
    [Tooltip("Spin speed")]
    public float spinSpeed = 10f;
    [Tooltip("Maximum speed car can go to be able to spin")]
    [Range(40, 1000)]
    public float maximumSpeedForSpinning = 35f;

    [HideInInspector]
    public float spinCooldown = 5f;
    public float spinCooldownTimer;

    [Tooltip("Spin duration in seconds.")]
    public float spinDuration = 2f;
    public float spinDurationTimer = 0f;

    [Tooltip("Maximum time in the air before spin is canceled.")]
    public float spinCancelAirTime = 2f;

    [HideInInspector]
    public bool isSpinning = false;
    [HideInInspector]
    public float frontFlipCooldown = 3f;
    public float frontFlipTimer;
    private bool wasUpsideDown = false;

    [Tooltip("Maximum tornado time.")]
    public float tornadoTime = 5;
    private float tornadoTimer = 0f;

    [Tooltip("Speed at which to reduce speed to 0.")]
    public float minSpeed = 10f;
    [Tooltip("Rate at which to reduce speed in units/sec.")]
    public float speedReductionRate = 10f;

    [HideInInspector]
    // How long car has been in the air
    public float airTime = 0f;  // This is what you want, Aaron (delete these comments)

    private bool wasAirborne = false;
    private Vector3 initialRotation;
    [HideInInspector]
    public float totalRotation = 0f;  // You *may* want this, Aaron, this is the signed angle traveled while airborne (for example, hasFlipped = true and totalRotation >= (720-leniency) means two flips!)
    private float lastEulerZ;
    public int flipPointMultiplier = 1;
    public int airtimePointMultiplier = 100;
    public float minimumAirtimeForPoints = 5f;
    private Vector3 initialRotationX;
    [HideInInspector]
    public float totalRotationX = 0f;  // You *may* want this, Aaron, this is the signed angle traveled while airborne (for example, hasFlipped = true and totalRotation >= (720-leniency) means two flips!)
    private float lastEulerX;
    public int flipPointMultiplierX = 1;
    public int airtimePointMultiplierX = 100;
    public float minimumAirtimeForPointsX = 5f;
    [Tooltip("Degree angle required to have rotated since first going airborne to count as a flip")]
    [Range(270, 360)]
    public float angleTraveledToFlip = 315f;  // leniency is 45 degrees

    [HideInInspector]
    // True if car has rotated angleTraveledToFlip much whilst airborne, currently set to false on landing, but that can be changed to be set false after being used in style points calculations
    public bool hasFlipped = false;  // You also want this, Aaron
    [HideInInspector]
    public bool hasFlippedX = false;

    // Use "isSpinning" to check for kills while doing a burnout/donuts, Aaron
    [HideInInspector]
    public bool frontFlipKillIndicator = false;  // Use to tell if a kill was caused by a front flip, Aaron
    [HideInInspector]
    public bool tiltKillIndicator = false;  // Use to tell if a kill was caused by a tilt, Aaron
    [HideInInspector]
    public bool groundPoundKillIndicator = false;  // Use to tell if a kill was caused by a ground pound, Aaron
    [HideInInspector]
    public bool horBoostKillIndicator = false;  // Use to tell if a kill was caused by a horizontal boost, Aaron
    [Tooltip("How fast the car needs to be moving in the purely horizontal direction for it to count as a horizontal kill in m/s.")]
    public float horBoostKillThreshold = 25f;  // Adjust this value as necessary if you notice the kill recognition isn't accurate, Aaron.
    [Tooltip("Car torque adjustment when upside down strength")]
    private float flipTorque = 1000000f;

    [HideInInspector]
    public bool tornado = false;
    private bool tornadoTimedOut = false;


    [Header("Animation/Effects")]
    public Animator animator;
    public GameObject wheelModels;
    public CinemachineFreeLook tiltCamera;
    public int tiltCameraHighPriority = 120;
    public int tiltCameraLowPriority = 8;
    public CinemachineFreeLook donutCamera;
    public int donutCameraHighPriority = 120;
    public int donutCameraLowPriority = 8;
    public CinemachineFreeLook airborneCamera;
    public int airborneCameraHighPriority = 14;
    public int airborneCameraLowPriority = 8;
    public CinemachineVirtualCamera supermanCamera;
    private int supermanCameraHighPriority = 16;
    private int supermanCameraLowPriority = 8;
    [Tooltip("Shockwave VFX that plays upon impact from a ground pound.")]
    public GameObject gpImpactVFX;

    public SphereCollider zombieFollowTrigger;

    [HideInInspector]
    public bool inspectingUpgrade = false;

    [HideInInspector] 
    public bool inEnemySpawner;

    public Rigidbody rb;
    private Vector3 previousTorque = Vector3.zero;

    private static CarManager _instance;
    public static float engineThrottle;
    public static float direction;
    public static float steering;
    public static float engineRPM;
    public static float engineMaxRPM;
    public AudioSource carManagerAudio;

    public MeshTrail meshTrail;

    public bool shouldGib;
    public bool swoleKill;
    [HideInInspector]
    public bool gotHitRecently = false;

    public bool inTutorial = false;

    public bool autoDriving = false;
    private float autoDrivingTimer = 0f;
    public bool griddy = false;
    public bool vrMode = false;

    public bool carTouchedGround = false;

    public Transform startTunnel;
    public Transform endTunnel;

    [HideInInspector]
    public bool insideGravityField = false;
    [HideInInspector]
    public float minSpeedForGravityField = 0;

    private Missions missionsComponent;

    public static CarManager Instance { get { return _instance; } }
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


    // Start is called before the first frame update
    void Start()
    {
        vrMode = SceneManager.GetActiveScene().name.Contains("TJ");
        inTutorial = SceneManager.GetActiveScene().name.Contains("Tutorial");
        carController = GetComponent<RCC_CarControllerV3>();
        missionsComponent = Missions.Instance;
        if (carController == null)
        {
            Debug.LogWarning("Missing Car Controller!");
        }
        rb = GetComponent<Rigidbody>();
        if (rb == null)
        {
            Debug.LogWarning("Missing Car Rigidbody!");
        }
        vertBoostCooldown = 3f;
        frontFlipCooldown = 3f;
        spinCooldown = 3f;
        carHealth = carMaxHealth;
        carGas = carMaxGas;
        numPoints = 0;
        pointMultiplier = 1f;
        speedBuff = 0f;
        currentState = CarState.Idle;
        engineThrottle = 0.0f;

        // Upgrade applications -- change these values as necessary
        tornadoTime -= UpgradeUnlocks.groundPoundUnlockNum > 2 ? 0 : 2;
        if (!inTutorial)
        {
            //airbornePitchSpeed = UpgradeUnlocks.airControl > 0 ? airbornePitchSpeed : 0;
            //airborneRollSpeed = UpgradeUnlocks.airControl > 0 ? airborneRollSpeed : 0;
            //airbornePitchSpeed = UpgradeUnlocks.airControl > 1 ? airbornePitchSpeed : airbornePitchSpeed / 2f;
            //airborneRollSpeed = UpgradeUnlocks.airControl > 1 ? airborneRollSpeed : airborneRollSpeed / 2f;
        }
        if (UpgradeUnlocks.tiltDuration == 0)
        {
            maximumTimeTilting -= 7;
        }
        else if (UpgradeUnlocks.tiltDuration == 1)
        {
            maximumTimeTilting -= 5;
        }
        if (UpgradeUnlocks.burnoutDuration == 0)
        {
            spinDuration -= 5;
        }
        else if (UpgradeUnlocks.burnoutDuration == 1)
        {
            spinDuration -= 3;
        }
        if (!inTutorial)
        {
            //if (UpgradeUnlocks.jumpCooldown == 0)
            //{
            //    vertBoostCooldown += 4;
            //}
            //else if (UpgradeUnlocks.jumpCooldown == 1)
            //{
            //    vertBoostCooldown += 2;
            //}
            //if (UpgradeUnlocks.boostCooldown == 0)
            //{
            //    horBoostRecharge += 2f;
            //    horBoostDuration -= 1f;
            //}
            //else if (UpgradeUnlocks.boostCooldown == 1)
            //{
            //    horBoostRecharge += 1f;
            //    horBoostDuration -= 0.5f;
            //}
            //if (UpgradeUnlocks.flipCooldown == 0)
            //{
            //    frontFlipCooldown += 2;
            //}
            //else if (UpgradeUnlocks.flipCooldown == 1)
            //{
            //    frontFlipCooldown += 1;
            //}
            //if (UpgradeUnlocks.burnoutCooldown == 0)
            //{
            //    spinCooldown += 4;
            //}
            //else if (UpgradeUnlocks.burnoutCooldown == 1)
            //{
            //    spinCooldown += 2;
            //}
            //if (UpgradeUnlocks.tiltCooldown == 0)
            //{
            //    tiltCooldown += 2;
            //}
            //else if (UpgradeUnlocks.tiltCooldown == 1)
            //{
            //    tiltCooldown += 1;
            //}
        }
        
        horBoostTimer = horBoostDuration;
        vertBoostTimer = vertBoostCooldown;
        spinCooldownTimer = spinCooldown;
        frontFlipTimer = frontFlipCooldown;
    }

    public void UnlockAllUpgrades()
    {
        UpgradeUnlocks.groundPoundUnlockNum = 3;
        UpgradeUnlocks.boostUnlockNum = 2;
        UpgradeUnlocks.airControl = 2;
        UpgradeUnlocks.horizontalBoosts = 1;
        UpgradeUnlocks.barrelRolls = 1;
        UpgradeUnlocks.tiltDuration = 2;
        UpgradeUnlocks.burnoutDuration = 2;
        UpgradeUnlocks.jumpCooldown = 2;
        UpgradeUnlocks.boostCooldown = 2;
        UpgradeUnlocks.flipCooldown = 2;
        UpgradeUnlocks.burnoutCooldown = 2;
        UpgradeUnlocks.tiltCooldown = 2;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    float horizontalTilt = 0f;
    void OnTilt(InputValue input)
    {
        if (autoDriving)
            horizontalTilt = 0f;
        else
            horizontalTilt = input.Get<Vector2>().x;
    }

    float boostDirectionY = 0f;
    float boostDirectionX = 0f;
    void OnLook(InputValue input)
    {
        if (autoDriving)
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

    float horizontal = 0f;
    float vertical = 0f;
    void OnMove(InputValue input)
    {
        if (autoDriving)
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
        northButton = input.isPressed && !autoDriving;
    }

    bool rightBumper = false;
    bool wasRightBumper = false;
    bool rightBumperPressed = false;
    void OnRightBumper(InputValue input)
    {
        rightBumper = input.isPressed && !autoDriving;
    }

    public bool boost = false;
    bool wasBoost = false;
    bool boostPressed = false;
    [HideInInspector]
    public bool boostRefreshing = false;
    public float horBoostSpeedAirborne = 78f;
    void OnBoost(InputValue input)
    {
        boost = input.isPressed && !autoDriving;
    }

    public bool leftBoost = false;
    bool wasLeftBoost = false;
    bool leftBoostPressed = false;
    void OnLeftBoost(InputValue input)
    {
        leftBoost = input.isPressed && !autoDriving;
    }

    public bool rightBoost = false;
    bool wasRightBoost = false;
    bool rightBoostPressed = false;
    void OnRightBoost(InputValue input)
    {
        rightBoost = input.isPressed && !autoDriving;
    }

    public bool reverse = false;
    bool wasReverse = false;
    bool reversePressed = false;
    void OnReverse(InputValue input)
    {
        reverse = input.isPressed && !autoDriving;
    }

    public bool rearView = false;
    void OnRearView(InputValue input)
    {
        rearView = input.isPressed && !autoDriving;
    }

    bool jump = false;
    bool wasJump = false;
    bool jumpPressed = false;
    void OnJump(InputValue input)
    {
        jump = input.isPressed && !autoDriving;
        //Debug.Log("JUMPED!");
    }

    bool frontFlip = false;
    bool wasFrontFlip = false;
    bool frontFlipPressed = false;
    void OnFrontFlip(InputValue input)
    {
        frontFlip = input.isPressed && !autoDriving;
    }

    bool headlights = false;
    bool wasHeadlights = false;
    bool headlightsPressed = false;

    public Light headlightLeft;
    public Light headlightRight;
    void OnHeadlights(InputValue input)
    {
        headlights = input.isPressed && !autoDriving;
    }

    void SetPressedInputs()
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
    void LogInputs()
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

    // Update is called once per frame
    void Update()
    {
        //if (jump)
        //{
        //    Debug.Log("JUMP HELD!");
        //}
        //else
        //{
        //    Debug.Log("JUMP NOT HELD!");
        //}
        // Temporary debug upgrade thing
        if (Input.GetKeyDown(KeyCode.F6))
        {
            UnlockAllUpgrades();
        }

        if (autoDriving)
        {
            transform.rotation = Quaternion.identity;
            transform.position = Vector3.Lerp(startTunnel.position, endTunnel.position, autoDrivingTimer/12f);
            autoDrivingTimer += Time.deltaTime;
            //transform.position += 50f*Vector3.forward * Time.deltaTime;
        }

        if (!carTouchedGround && carController.isGrounded)
            carTouchedGround = true;

        SetPressedInputs();
        EnvironmentalStyle();
        steering = carController.steerInput * carController.steerAngle;
        carSpeed = carController.speed;
        carGas = carController.fuelTank;
        carMaxGas = carController.fuelTankCapacity;
        carMaxSpeed = carController.maxspeed;
        heading = transform.forward;
        direction = carController.direction;
        engineThrottle = carController.throttleInput;
        engineRPM = carController.engineRPM;
        engineMaxRPM = carController.maxEngineRPM;
        swoleKill = false;
        // Health and loss
        if ((carHealth <= 0 || carGas <= 0) && GameManager.instance.gameState != GameManager.GameStates.lose && !inTutorial)
        {
            currentState = CarState.Destroyed;
            GameManager.instance.HandleLose();
        }

        // Remaining car states
        else if (carController.driftingNow)  // Drifting
        {
            if (carController.driftAngle < 0)
            {
                currentState = CarState.DriftingRight;
            }
            else if (carController.driftAngle > 0)
            {
                currentState = CarState.DriftingLeft;
            }
        }
        else if (carController.speed > notIdleThreshold)  // Forward or backwards
        {
            if (CheckDirection())
            {
                currentState = CarState.Forward;
            }
            else
            {
                currentState = CarState.Backward;
            }
        }
        else
        {
            currentState = CarState.Idle;
        }
        // TILT STUFF
        if (GameManager.instance.gameState == GameManager.GameStates.play && (!GasStationManager.inGasStation && (horizontalTilt > 0.7) && carController.isGrounded && carSpeed >= minimumSpeedForTilting && currentState == CarState.Forward && tiltTimer < maximumTimeTilting && canTilt))
        {
            animator.SetBool("BalancingRight", true);
            animator.SetBool("BalancingLeft", false);
            wheelModels.SetActive(false);
            if (currentState != CarState.TiltingLeft)
                currentState = CarState.TiltingRight;
            tiltTimer += Time.deltaTime;
        }
        else if (GameManager.instance.gameState == GameManager.GameStates.play && (!GasStationManager.inGasStation && (horizontalTilt < -0.7) && carController.isGrounded && carSpeed >= minimumSpeedForTilting && currentState == CarState.Forward && tiltTimer < maximumTimeTilting && canTilt))
        {
            animator.SetBool("BalancingRight", false);
            animator.SetBool("BalancingLeft", true);
            wheelModels.SetActive(false);
            if (currentState != CarState.TiltingRight)
                currentState = CarState.TiltingLeft;
            tiltTimer += Time.deltaTime;
        }
        else
        {
            animator.SetBool("BalancingRight", false);
            animator.SetBool("BalancingLeft", false);
            if (animator.GetCurrentAnimatorStateInfo(0).IsName("Default"))
            {
                if (!wheelModels.activeInHierarchy)
                {
                    StartCoroutine(TiltCooldown(tiltCooldown));
                    tiltTimer = 0f;
                }
                wheelModels.SetActive(true);
            }
            else if (animator.GetCurrentAnimatorStateInfo(0).IsName("LeftBalance"))
                currentState = CarState.TiltingLeft;
            else if (animator.GetCurrentAnimatorStateInfo(0).IsName("RightBalance"))
                currentState = CarState.TiltingRight;
        }
        if (tiltTimer >= 0.5f && Mathf.Abs(horizontalTilt) <= 0.7)
        {
            tiltTimer = maximumTimeTilting;
        }
        if (currentState == CarState.TiltingLeft || currentState == CarState.TiltingRight)
        {
            tiltCamera.m_Priority = tiltCameraHighPriority;
            if (!tiltKillIndicator)
                missionsComponent.RegisterMove(MoveType.tilt);
            tiltKillIndicator = true;
            if (tiltTimer < maximumTimeTilting)// && northButton && (rightBumper || boost))
                StartCoroutine(GameManager.instance.DoVibration(0.75f, 0.75f, Time.unscaledDeltaTime));
        }
        else
        {
            tiltCamera.m_Priority = tiltCameraLowPriority;
            if (tiltKillIndicator)
                StartCoroutine(GameManager.instance.DoVibration(1f, 1f, 0.25f));
            tiltKillIndicator = false;
        }

        // END TILT STUFF

        // HORIZONTAL BOOST STUFF  -- the state should be "drifting"
        if (horBoostTimer >= horBoostDuration)
            boostRefreshing = false;
        if (!tornado && !isSpinning && GameManager.instance.gameState == GameManager.GameStates.play && (horBoostTimer >= horBoostDuration || (boost && horBoostTimer > 0 && !boostRefreshing)) && Time.timeScale > 0)
        {
            if ((currentState == CarState.TiltingLeft || currentState == CarState.TiltingRight) && UpgradeUnlocks.boostUnlockNum < 1)
            {

            }
            //else if (!carController.isGrounded && UpgradeUnlocks.boostUnlockNum < 2 && !inTutorial)
            //{

            //}
            else if (boost || (UpgradeUnlocks.horizontalBoosts > 0 && (leftBoostPressed || rightBoostPressed) && !(currentState == CarState.TiltingLeft || currentState == CarState.TiltingRight)))
            {
                if (horBoostTimer >= horBoostDuration)
                {
                    SfxManager.instance.PlaySoundAtRandom(SfxManager.SfxCategory.Boost);
                    missionsComponent.RegisterMove(MoveType.boost);
                }
                    
                meshTrail.DoFlashTrails();
                Vector3 direction;
                float horBoostSpeedMultiplier = 1;
                if (boost)
                {
                    horBoostSpeedMultiplier = Time.deltaTime;
                    direction = transform.forward * (reverse ? -1 : 1);
                }
                else if (leftBoostPressed)
                    direction = -transform.right;
                else
                    direction = transform.right;

                if (carController.isGrounded && ((reverse && Vector3.Dot(rb.velocity, transform.forward) > 0) || (!reverse && Vector3.Dot(rb.velocity, transform.forward) < 0)))
                    rb.velocity = Vector3.zero;

                if (carController.isGrounded)
                    rb.velocity += direction * horBoostStrength * horBoostSpeedMultiplier;
                else
                {
                    if (boost)
                    {
                        direction = transform.forward;
                        //direction = Vector3.ProjectOnPlane(direction, Vector3.up);
                        direction = direction.normalized;
                    }
                    else if (leftBoostPressed)
                    {
                        direction = Vector3.ProjectOnPlane(-transform.right, Vector3.up);
                        direction = direction.normalized;
                    }
                    else
                    {
                        direction = Vector3.ProjectOnPlane(transform.right, Vector3.up);
                        direction = direction.normalized;
                    }
                    
                    if (reverse && Vector3.Dot(rb.velocity, transform.forward) < 0)
                        rb.velocity = Vector3.zero;
                    if (horBoostSpeedMultiplier == 1)
                        rb.velocity += direction * horBoostStrength / 2f * horBoostSpeedMultiplier;
                    else
                    {
                        float horMagnitude = Mathf.Min(39f, Mathf.Max(horBoostStrength / 2f, rb.velocity.magnitude));
                        Vector3 tempRBVelocity = direction * (horMagnitude + horBoostStrength / 2f * horBoostSpeedMultiplier);
                        rb.velocity = new Vector3(tempRBVelocity.x, Mathf.Max(0,rb.velocity.y), tempRBVelocity.z);
                    }       
                }
                horBoostTimer = horBoostSpeedMultiplier == 1 ? 0 : horBoostTimer - 2*Time.deltaTime;
                if (horBoostTimer <= 0)
                    boostRefreshing = true;
                StartCoroutine(GameManager.instance.DoVibration(1, 1, 0.25f));
            }
        }

        horBoostTimer += (boostRefreshing ? horBoostDuration/horBoostRecharge : 1) * Time.deltaTime;
        horBoostTimer = Mathf.Min(horBoostDuration, horBoostTimer);

        //if ((!boost && wasBoost && !boostRefreshing && !(!carController.isGrounded && UpgradeUnlocks.boostUnlockNum < 2 && !inTutorial)) || ((gotHitRecently || (!tornado && !isSpinning && GameManager.instance.gameState == GameManager.GameStates.play && (frontFlipTimer >= frontFlipCooldown && !(currentState == CarState.TiltingLeft || currentState == CarState.TiltingRight) && frontFlipPressed))) && !boostRefreshing && horBoostTimer < horBoostDuration))  // boost let go
        //{
        //    horBoostTimer = 0;
        //    boostRefreshing = true;
        //}
        if ((!boost && wasBoost && !boostRefreshing) || ((gotHitRecently || (!tornado && !isSpinning && GameManager.instance.gameState == GameManager.GameStates.play && (frontFlipTimer >= frontFlipCooldown && !(currentState == CarState.TiltingLeft || currentState == CarState.TiltingRight) && frontFlipPressed))) && !boostRefreshing && horBoostTimer < horBoostDuration))  // boost let go
        {
            horBoostTimer = 0;
            boostRefreshing = true;
        }

        horBoostKillIndicator = Vector3.Project(rb.velocity, transform.right).magnitude >= horBoostKillThreshold;

       

        // END HORIZONTAL BOOST STUFF

        // VERTICAL BOOST STUFF  -- the state should be "drifting"
        if (GameManager.instance.gameState == GameManager.GameStates.play && (vertBoostTimer >= vertBoostCooldown && !(currentState == CarState.TiltingLeft || currentState == CarState.TiltingRight) && jumpPressed))
        {
            if (carController.isGrounded)
            {
                rb.velocity += transform.up * vertBoostStrength;
                jumped = true;
                StartCoroutine(GameManager.instance.DoVibration(0.5f, 0.5f, 0.25f));
                if (UpgradeUnlocks.groundPoundUnlockNum < 1)
                {
                    vertBoostTimer = 0;
                }
                if (UpgradeUnlocks.groundPoundUnlockNum > 1 && isSpinning)
                {
                    if (!tornado)
                        SfxManager.instance.PlaySoundAtRandom(SfxManager.SfxCategory.Tornado);
                    tornado = true;
                    spinCooldownTimer = 0f;
                    spinDurationTimer = 0f;
                    donutCamera.m_Priority = donutCameraLowPriority;
                    isSpinning = false;
                    autoSpinTimer = -minimumSpinningTime - 1;
                }
                //Debug.Log("JUMPED");
                SfxManager.instance.PlaySoundAtRandom(SfxManager.SfxCategory.Jump);
                missionsComponent.RegisterMove(MoveType.jump);
            }
            else if (UpgradeUnlocks.groundPoundUnlockNum > 0 && jumped && !carController.isGrounded)  // Arsenii, this is the ground pound code
            {
                //Debug.Log("POUNDED");
                int layerMask = 1 << 0;
                RaycastHit hit;
                rb.angularVelocity = Vector3.zero;
                Physics.Raycast(transform.position, Vector3.down, out hit, Mathf.Infinity, layerMask);
                shockRadius = Mathf.Clamp(shockRadiusScaling * hit.distance, minShockRadius, maxShockRadius);
                rb.velocity = -10 * vertBoostStrength * Vector3.up;
                jumped = false;
                tornado = false;
                vertBoostTimer = 0;
                groundPoundKillIndicator = true;
                Physics.IgnoreLayerCollision(8, 17);
                Physics.IgnoreLayerCollision(9, 17);
                transform.eulerAngles = new Vector3(0, transform.eulerAngles.y + (Vector3.Dot(transform.up, Vector3.up) >= 0 ? 0 : 180), 0);
                StartCoroutine(GameManager.instance.DoVibration(0.5f, 0.5f, 0.25f));
            }
        }

        if (jumped && !wasGrounded && carController.isGrounded)
        {
            //Debug.Log("LANDED");
            jumped = false;
            tornado = false;
            //vertBoostTimer = 0;
        }
        else if (airTime >= 0.5f)
        {
            jumped = true;
        }

        if (jump && tornado && tornadoTimer <= tornadoTime)
        {
            rb.velocity = 4.9f*Vector3.up;
            rb.angularVelocity = rb.angularVelocity.y > 0 ? Vector3.up * spinSpeed : Vector3.down * spinSpeed;
            tornadoTimer += Time.deltaTime;
            zombieFollowTrigger.radius = 4.9f*tornadoTime + 80;
            StartCoroutine(GameManager.instance.DoVibration(1f, 1f, Time.unscaledDeltaTime));
        }
        else if ((wasJump && !jump && tornado) || tornadoTimer > tornadoTime)
        {
            if (tornadoTimer > tornadoTime)
            {
                tornadoTimedOut = true;
            }
            int layerMask = 1 << 0;
            RaycastHit hit;
            Physics.Raycast(transform.position, Vector3.down, out hit, Mathf.Infinity, layerMask);
            shockRadius = Mathf.Clamp(shockRadiusScaling * hit.distance, minShockRadius, maxShockRadius);
            rb.velocity = -10 * vertBoostStrength * Vector3.up;
            jumped = false;
            tornado = false;
            vertBoostTimer = 0;
            groundPoundKillIndicator = true;
            Physics.IgnoreLayerCollision(8, 17);
            Physics.IgnoreLayerCollision(9, 17);
            transform.eulerAngles = new Vector3(0, transform.eulerAngles.y + (Vector3.Dot(transform.up, Vector3.up) >= 0 ? 0 : 180), 0);
            StartCoroutine(GameManager.instance.DoVibration(0.5f, 0.5f, 0.25f));
            tornadoTimer = 0;
            zombieFollowTrigger.radius = 40;
        }
        //Debug.Log(tornadoTimer);

        //if (carController.isGrounded)
        //{
        //    if (groundPoundKillIndicator)
        //    {
        //        gpImpactVFX.transform.localScale = new Vector3(shockRadius, 1, shockRadius);
        //        Instantiate(gpImpactVFX, transform.position - 0.5f*Vector3.up, transform.rotation, null);
        //        StartCoroutine(GameManager.instance.DoVibration(1, 1, 0.25f));
        //        SendGPShockwave();
        //        CameraShaker.Instance.ShakeCamera(4f, 0.25f);
        //        StartCoroutine(ResetMomentum());
        //        SfxManager.instance.PlaySoundAtRandom(SfxManager.SfxCategory.GroundPound, carManagerAudio, true);
        //    }
        //    groundPoundKillIndicator = false;
        //}

        vertBoostTimer += Time.deltaTime;

        // END VERTICAL BOOST STUFF

        // SPIN STUFF -- the state should be "drifting"
        if (!autoSpinActivated && carSpeed >= maximumSpeedForSpinning && Mathf.Abs(carController.steerInput) > 0.7 && carController.isGrounded && (currentState == CarState.DriftingLeft || currentState == CarState.DriftingRight) && spinCooldownTimer >= spinCooldown)
        {
            if (carController.handbrakeInput > 0 && !wasDriftPressed)
            {
                ++driftHitCount;
            }
            if (driftHitCount < 2)
            {
                autoSpinTimer = autoSpinTotalTime;
            }
            else if (autoSpinTimer > 0)
            {
                autoSpinActivated = true;
                autoSpinTimer = 0f;
            }
            else
            {
                driftHitCount = 1;
                autoSpinTimer = autoSpinTotalTime;
            }
        }
        else
        {
            driftHitCount = 0;
        }

        if (autoSpinTimer < -minimumSpinningTime)
        {
            autoSpinActivated = false;
        }

        autoSpinTimer -= Time.deltaTime;

        wasDriftPressed = carController.handbrakeInput > 0;

        if (GameManager.instance.gameState == GameManager.GameStates.play && (currentState == CarState.DriftingLeft || currentState == CarState.DriftingRight) && ((autoSpinning && (autoSpinActivated || (isSpinning && carController.handbrakeInput > 0)) && spinDurationTimer < spinDuration && spinCooldownTimer >= spinCooldown && airTime <= spinCancelAirTime) 
            || (carSpeed <= maximumSpeedForSpinning && spinDurationTimer < spinDuration && spinCooldownTimer >= spinCooldown && airTime <= spinCancelAirTime &&
            (((currentState == CarState.DriftingLeft || isSpinning) && false) || ((currentState == CarState.DriftingRight || isSpinning) && false)))))  // falses were formerly Fire9 inputs if we want to bring back the old way of doing burnouts
        {
            if (!isSpinning)
            {
                SfxManager.instance.PlaySoundAtRandom(SfxManager.SfxCategory.Burnout);
                missionsComponent.RegisterMove(MoveType.burnout);
            }
            rb.velocity = rb.velocity.normalized * 9.8f;
            if (currentState == CarState.DriftingRight)
            {
                rb.angularVelocity = transform.up * spinSpeed;
            }
            else if (currentState == CarState.DriftingLeft)
            {
                rb.angularVelocity = -transform.up * spinSpeed; 
            }
            spinDurationTimer += Time.deltaTime;
            donutCamera.m_Priority = donutCameraHighPriority;
            isSpinning = true;
            StartCoroutine(GameManager.instance.DoVibration(1, 1, Time.unscaledDeltaTime));

            if (spinDurationTimer >= spinDuration)
            {
                spinCooldownTimer = 0f;
                spinDurationTimer = 0f;
                donutCamera.m_Priority = donutCameraLowPriority;
                isSpinning = false;
            }
        }
        else if (spinDurationTimer > 0)
        {
            spinCooldownTimer = 0f;
            spinDurationTimer = 0f;
            donutCamera.m_Priority = donutCameraLowPriority;
            isSpinning = false;
        }

        spinCooldownTimer += Time.deltaTime;

        // END SPIN STUFF

        // FRONT FLIP STUFF
        if (!tornado && !isSpinning && GameManager.instance.gameState == GameManager.GameStates.play && (frontFlipTimer >= frontFlipCooldown && !(currentState == CarState.TiltingLeft || currentState == CarState.TiltingRight) && frontFlipPressed))
        {
            //if (carController.isGrounded)
            //{
            //    rb.velocity = Vector3.up * 10f;
            //    rb.angularVelocity = transform.right * 8f;
            //}
            //else
            //{
            rb.velocity += Vector3.up * 10f;
            if (Mathf.Abs(horizontal) <= 0.7f)
                rb.angularVelocity = vertical >= 0 ? transform.right * 8f : -transform.right * 8f;
            else
            {
                //if (UpgradeUnlocks.barrelRolls > 0 || inTutorial)
                    rb.angularVelocity = horizontal >= 0 ? -transform.forward * 10f : transform.forward * 10f;
                //else
                //    rb.angularVelocity = vertical >= 0 ? transform.right * 8f : -transform.right * 8f;
            }
            //}
            frontFlipTimer = 0f;
            wasUpsideDown = false;
            SfxManager.instance.PlaySoundAtRandom(SfxManager.SfxCategory.Flip);
            missionsComponent.RegisterMove(MoveType.flip);
            StartCoroutine(GameManager.instance.DoVibration(0.5f, 0.5f, 0.25f));
        }

        frontFlipTimer += Time.deltaTime;

        if (Vector3.Dot(transform.up, Vector3.up) < 0)
        {
            wasUpsideDown = true;
            frontFlipKillIndicator = true;
        }

        if (carController.isGrounded)
        {
            wasUpsideDown = false;
            if (frontFlipKillIndicator)
                StartCoroutine(GameManager.instance.DoVibration(0.66f, 0.66f, 0.25f));
            frontFlipKillIndicator = false;
        }

        // END FRONT FLIP STUFF

        //Debug.Log(currentState);  // Comment this out to prevent console clutter

        // SLOW DOWN WHEN UNDER X SPEED
        if (carController.handbrakeInput > 0 || (((carController.brakeInput + carController.throttleInput < 0.1 && carController.speed < minSpeed) || carController.brakeInput > 0.1f) && carController.speed >= 1 && carController.isGrounded))
        {
            float newMagnitude = Mathf.Max(0, rb.velocity.magnitude - speedReductionRate * Time.deltaTime);
            rb.velocity = rb.velocity.normalized * newMagnitude;
        }

        // Airborne timer
        if (!carController.isGrounded)
        {
            airTime += Time.deltaTime;

            // The number of wrong answers to this problem on the Unity forums is amazing
            // A lot of people would kill for this solution xD
            if (!wasAirborne)
            {
                totalRotation = 0f;
                initialRotation = transform.up;
                lastEulerZ = transform.localEulerAngles.z;

                totalRotationX = 0f;
                initialRotationX = transform.forward;
                lastEulerX = transform.localEulerAngles.x;
            }
            else
            {
                var signOfAngle = Mathf.Sign(transform.localEulerAngles.z - lastEulerZ);
                var newAngle = Mathf.Acos(Vector3.Dot(initialRotation, transform.up)) * 180/Mathf.PI;
                totalRotation += signOfAngle*newAngle;
                initialRotation = transform.up;
                lastEulerZ = transform.localEulerAngles.z;

                if (Mathf.Abs(totalRotation) >= angleTraveledToFlip && !hasFlipped)
                {
                    hasFlipped = true;
                    //Debug.Log("Did a flip!");
                }

                // transform.localEulerAngles.x is annoying
                var angle = transform.localEulerAngles.x;
                if (Vector3.Dot(Vector3.up, transform.up) < 0)
                {
                    angle = 180 - angle;
                    if (angle < 0)
                    {
                        angle += 360;
                    }
                }
                var signOfAngleX = Mathf.Sign(angle - lastEulerX);
                var newAngleX = Mathf.Acos(Vector3.Dot(initialRotationX, transform.forward)) * 180 / Mathf.PI;
                totalRotationX += signOfAngleX * newAngleX;
                initialRotationX = transform.forward;
                lastEulerX = angle;

                if (Mathf.Abs(totalRotationX) >= angleTraveledToFlip && !hasFlippedX)
                {
                    hasFlippedX = true;
                    //Debug.Log("Did a flip!");
                }
            }
        }
        else
        {
            airTime = 0f;
            hasFlipped = false;  // Aaron, you may want to move this line to after you use the flipped status for style
            hasFlippedX = false;
        }

        //Debug.Log(hasFlipped);

        wasGrounded = carController.isGrounded;
        wasAirborne = !carController.isGrounded;

        if (carTouchedGround)
            airborneCamera.m_Priority = airTime > 0 ? airborneCameraHighPriority : airborneCameraLowPriority;

        //Debug.Log(shockRadius);

        // HEALTH REGEN
        if (carHealth < carMaxHealth && Time.time - lastHit >= timeBeforeHealthRegeneration)
        {
            carHealth += regenerationRate * Time.deltaTime;
            carHealth = Mathf.Min(carHealth, carMaxHealth);
        }
        // END HEALTH REGEN

        if (GameManager.instance.uiManager.currentPage == GameManager.instance.upgradeMenuPage && !inspectingUpgrade)
            Rotate();

        // Max horizontal speed cap
        Vector3 horizontalVelocity = new(rb.velocity.x, 0, rb.velocity.z);
        if (horizontalVelocity.magnitude > (carMaxSpeed + speedBuff)/3.6f)  // 3.6f is conversion rate
        {
            rb.velocity = new Vector3(Mathf.MoveTowards(rb.velocity.x, rb.velocity.normalized.x * carMaxSpeed / 3.6f, 0.5f), rb.velocity.y, Mathf.MoveTowards(rb.velocity.z, rb.velocity.normalized.z * carMaxSpeed / 3.6f, 100*Time.deltaTime/3.6f));
        }

        gotHitRecently = false;

        LogInputs();
    }

    private void FixedUpdate()
    {
        // Handle airborne car rotation when not boosting
        if (!carController.isGrounded && !(boost && horBoostTimer > 0 && !boostRefreshing))
        {
            float horizontalInput = horizontal;
            float verticalInput = vertical;
            verticalInput = verticalInput > 0 ? 0 : verticalInput;  // help keyboard people out for now (they use the same button for gas as to lean)
            
            rb.AddTorque(-airborneRollSpeed * horizontalInput * transform.forward + airbornePitchSpeed * verticalInput * transform.right, ForceMode.Acceleration);

        }
        else if (!carController.isGrounded && boost && horBoostTimer > 0 && !boostRefreshing) // Handle it when the car is boosting
        {
            if (supermanCamera.m_Priority != supermanCameraHighPriority)
                rb.angularVelocity = Vector3.zero;
            float horizontalInput = horizontal;
            float verticalInput = vertical;

            float offsetAngle = 0;
            if (horizontalInput >= 0.3)
                offsetAngle = 30;
            else if (horizontalInput <= -0.3)
                offsetAngle = -30;

            // Calculate the target direction which is offsetAngle degrees clockwise of Vector3.up based on transform.forward
            Vector3 targetDirection = Quaternion.AngleAxis(-offsetAngle, transform.forward) * Vector3.up;
            // Calculate the rotation needed to align transform.up with Vector3.up
            Vector3 newUp = Vector3.RotateTowards(transform.up, targetDirection, 240 * Mathf.Deg2Rad * Time.fixedDeltaTime, 0.0f);
            Quaternion targetRotation = Quaternion.FromToRotation(transform.up, newUp) * transform.rotation;

            // Apply the rotation
            transform.rotation = targetRotation;
            rb.AddTorque(airborneRollSpeed*0.75f * horizontalInput * transform.up, ForceMode.Acceleration);
            supermanCamera.m_Priority = supermanCameraHighPriority;
        }
        if (!(!carController.isGrounded && boost && horBoostTimer > 0 && !boostRefreshing))
        {
            supermanCamera.m_Priority = supermanCameraLowPriority;
        }
        //if ((!(!carController.isGrounded && boost && horBoostTimer > 0 && !boostRefreshing && !(!carController.isGrounded && UpgradeUnlocks.boostUnlockNum < 2 && !inTutorial))) && !(insideGravityField && carSpeed >= minSpeedForGravityField))
        if ((!(!carController.isGrounded && boost && horBoostTimer > 0 && !boostRefreshing)) && !(insideGravityField && carSpeed >= minSpeedForGravityField))
        {
            if (wasUpsideDown && !carController.isGrounded && Vector3.Dot(transform.up, Vector3.up) > 0)
                rb.AddForce(4 * additionalGravity * rb.mass * Vector3.down);
            else
                rb.AddForce(additionalGravity * rb.mass * Vector3.down);
            rb.useGravity = true;
        }
        else if (insideGravityField && carSpeed >= minSpeedForGravityField)
        {
            rb.useGravity = false;
            rb.AddForce(4 * additionalGravity * rb.mass * -transform.up);
        }

        if (groundPoundKillIndicator)
        {
            int layerMask = (1 << 0) | (1 << 16);
            Collider[] hitColliders = Physics.OverlapBox(transform.position - transform.forward * 0.1f + transform.up * 0.25f, new Vector3(4.5f, 3f, 8f) / 2, Quaternion.identity, layerMask);
            int i = 0;
            //Check when there is a new collider coming into contact with the box
            while (carController.isGrounded || i < hitColliders.Length)
            {
                //Output all of the collider names
                //Debug.Log("Hit : " + hitColliders[i].name + i);
                if (hitColliders[i].tag == "Environment")
                {
                    gpImpactVFX.GetComponentInChildren<VisualEffect>().SetFloat("ImpactSize", shockRadius);
                    //gpImpactVFX.transform.localScale = new Vector3(shockRadius, 1, shockRadius);
                    Instantiate(gpImpactVFX, transform.position - 0.5f * Vector3.up, transform.rotation, null);
                    StartCoroutine(GameManager.instance.DoVibration(1, 1, 0.25f));
                    SendGPShockwave();
                    CameraShaker.Instance.ShakeCamera(4f, 0.25f);
                    StartCoroutine(ResetMomentum());
                    SfxManager.instance.PlaySoundAtRandom(SfxManager.SfxCategory.GroundPound, carManagerAudio, true);
                    groundPoundKillIndicator = false;
                    Physics.IgnoreLayerCollision(8, 17, false);
                    Physics.IgnoreLayerCollision(9, 17, false);
                    airTime = 0f;
                    if (tornadoTimedOut)
                    {
                        StartCoroutine(TornadoSlow());
                    }
                    tornadoTimedOut = false;
                    break;
                }
                ++i;
            }
        }
        // TODO: Make everything here a user setting called "landing assist"
        Vector3 localAngularVelocity = transform.InverseTransformDirection(rb.angularVelocity);

        // Calculate the "upside down" factor
        float upsideDownFactor = Vector3.Dot(transform.up, Vector3.down);

        // Check if the car is more upside down than upright
        if (upsideDownFactor > 0 && !(!carController.isGrounded && boost && horBoostTimer > 0 && !boostRefreshing))
        {
            // Calculate the flip torque magnitude, using the upside down factor
            float flipTorqueMagnitude = upsideDownFactor * flipTorque;

            // Determine the direction of most rotation and apply the flip torque
            //if (Mathf.Abs(localAngularVelocity.x) > Mathf.Abs(localAngularVelocity.z) && !hasFlippedX)
            //{
            //    // The car is rotating more around the right axis
            //    Vector3 flipTorqueDirection = localAngularVelocity.x > 0 ? transform.right : -transform.right;
            //    rb.AddTorque(flipTorqueDirection * flipTorqueMagnitude, ForceMode.Force);
            //}
            if (Mathf.Abs(localAngularVelocity.x) <= Mathf.Abs(localAngularVelocity.z))
            {
                // The car is rotating more around the forward axis
                Vector3 flipTorqueDirection = localAngularVelocity.z > 0 ? transform.forward : -transform.forward;
                rb.AddTorque(flipTorqueDirection * flipTorqueMagnitude, ForceMode.Force);
            }
        }
    }

    public void EnvironmentalStyle()
    {
        if (carController.isGrounded && wasAirborne)
        {
            //Debug.Log(totalRotation);
            if (hasFlipped && !(float.IsPositiveInfinity(totalRotation)  || float.IsNegativeInfinity(totalRotation) || float.IsNaN(totalRotation)))
            {
                int numFlips = ((int)Mathf.Abs(totalRotation) / 360) + (Mathf.Abs(totalRotation) % 360 > angleTraveledToFlip ? 1 : 0);
                if(numFlips > 0)
                {
                    int pts = NewStyleSystem.instance.AddPointsWithMultiplier(numFlips * flipPointMultiplier);
                    StyleTextbox.instance.AddRewardInfoAndHandleTimeout($"Barrel Roll x{numFlips}", pts, 3f);
                }
            }
            if (hasFlippedX && !(float.IsPositiveInfinity(totalRotationX) || float.IsNegativeInfinity(totalRotationX) || float.IsNaN(totalRotationX)))
            {
                int numFlips = ((int)Mathf.Abs(totalRotationX) / 360) + (Mathf.Abs(totalRotationX) % 360 > angleTraveledToFlip ? 1 : 0);
                if (numFlips > 0)
                {
                    int pts = NewStyleSystem.instance.AddPointsWithMultiplier(numFlips * flipPointMultiplier);
                    StyleTextbox.instance.AddRewardInfoAndHandleTimeout($"Flip x{numFlips}", pts, 3f);
                }
            }
            if (airTime > minimumAirtimeForPoints)
            {
                int points = (int)airTime*airtimePointMultiplier;
                int pts = NewStyleSystem.instance.AddPointsWithMultiplier(points);
                StyleTextbox.instance.AddRewardInfoAndHandleTimeout($"{(int)airTime}Sec jump", pts, 3f);
            }
        }

    }
    public void Damage(float damage)
    {
        carHealth -= damage;
    }
    public void SpeedToDamage()
    {
        carHealth -= carSpeed * speedDamageFactor;

    }
    public void ImpulseToDamage(Vector3 impulse)
    {
        float totalInwardImpulse = Mathf.Sqrt(impulse.x * impulse.x + impulse.y * impulse.y);
        float changeInSpeed = totalInwardImpulse / rb.mass;
        if(changeInSpeed > damageThreshold)
        {
            carHealth -= changeInSpeed * speedDamageFactor;
        }
    }

    public void SetGas(float gasAmount)
    {
        if(carController != null)
        {
            carGas = gasAmount;
            carController.fuelTank = gasAmount;
        }
    }

    public void UpgradeGas(float upgradeAmount)
    {
        if (carController != null)
        {
            carController.fuelTankCapacity += upgradeAmount;
            carMaxGas += upgradeAmount;
            carGas += upgradeAmount;
        }
    }
    public void UpgradeSpeed(float upgradeAmount)
    {
        if (carController != null)
        {
            carController.maxspeed += upgradeAmount;
        }
    }


    // Helper function to find out if we are moving forward or backward, true means forward, false means backward
    private bool CheckDirection()
    {
        float dot = Vector3.Dot(transform.forward, rb.velocity);

        return dot >= 0;
    }

    void OnDestroy() 
    {
        if(this == _instance) 
        {
            _instance = null; 
        } 
    }

    public void VisualRepair()
    {
        carController.damage.repairNow = true;
    }

    public void Rotate()
    {
        transform.eulerAngles = new Vector3(0, transform.eulerAngles.y, 0);
        transform.Rotate(0, Time.deltaTime * rotationSpeed, 0);
    }

    public void AutoDrive(bool on)
    {
        autoDriving = on;
    }

    private IEnumerator TiltCooldown(float seconds)
    {
        canTilt = false;
        yield return new WaitForSeconds(seconds);
        canTilt = true;
    }

    private void SendGPShockwave()
    {
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, shockRadius);
        foreach (var hitCollider in hitColliders)
        {
            if (hitCollider.enabled && hitCollider.tag == "Enemy")
            {
                if (shockKills)
                {
                    hitCollider.GetComponent<EnemyStats>().Damage(999);
                    NewStyleSystem.instance.AddCarKill();
                }
                else
                    hitCollider.GetComponent<EnemyStats>().Push(Vector3.up * 999f, true);
            }   
        }
    }

    public IEnumerator ResetMomentum()
    {
        rb.isKinematic = true;
        rb.velocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
        float oldDrag = rb.drag;
        rb.drag = 1000000;
        float oldAngularDrag = rb.angularDrag;
        rb.angularDrag = 1000000;
        yield return new WaitForSeconds(1/10f);
        rb.isKinematic = false;
        rb.drag = oldDrag;
        rb.angularDrag = oldAngularDrag;
        if (carController.throttleInput > 0.7)
            rb.velocity += transform.forward * horBoostStrength;
    }

    IEnumerator TornadoSlow()
    {
        Time.timeScale = 0.1f;
        Time.fixedDeltaTime = 0.02F * Time.timeScale;
        yield return new WaitForSecondsRealtime(1f);
        Time.timeScale = 1;
        Time.fixedDeltaTime = 0.02F;
    }

    //Draw the Box Overlap as a gizmo to show where it currently is testing. Click the Gizmos button to see this
    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        //Check that it is being run in Play Mode, so it doesn't try to draw this in Editor mode
        //Draw a cube where the OverlapBox is (positioned where your GameObject is as well as a size)
        Gizmos.DrawWireCube(transform.position - transform.forward*0.1f + transform.up*0.25f, new Vector3(4.5f, 3f, 8f));
    }
}
