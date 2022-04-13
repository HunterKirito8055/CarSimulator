using System.Collections;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TrafficSimulation;
//Code By Ashish
public enum PlayerLane
{
    None, lane1, lane2
}
public class SimulationManager : MonoBehaviour
{
    #region Private Variables
    [Range(30f, 80f)]
    [SerializeField] float offsetAngle = 80f;
    float rayLength = 25;
    int playerStopVal = 0;

    //Player Damage Causing to Tra
    float damageCauseSpeed = 8;
    float damageCauseDistance = 12;
    float damageCauserayLength = 42f;
    float damageCauseAiCarSpeed = 1;

    int crashOtherCars;
    int crashObstacles;

    bool isIntersectionEntered = false;
    bool isIntersectionExit = false;

    bool isStopSignBoard = false;
    bool isPlayerStopped = false;

    bool isFirstLane;
    bool isSecondLane;
    bool isTouchedFirstLane;
    bool isTouchedSecondlane;
    bool dontCastRayOnTrafficSignal;
    bool checkBackHit;

    bool isRedSignal_Enter = false;
    bool isRedSignal_Exit = false;

   [SerializeField] Vector3 backNormal;
    [SerializeField] Vector3 frontNormal;

    RaycastHit playerCauseDamageHit;
    RaycastHit fronthit;
    RaycastHit backhit;

    TrafficLightSignal currentTrafficSignal = TrafficLightSignal.NONE;
    Intersection intersectionObj;

    Vector3 storeFrontRayVector;
    Vector3 storeBackRayVector;
    bool isTrafficCrossed = true;
    bool isEnteredBlinkers = false;
    #endregion

    PlayerLane currentPlayerLane;
    public LayerMask intersectionLayer;
    [SerializeField] Transform raycastPoint;
    [HideInInspector] public bool isRightIndicatorOn;
    [HideInInspector] public bool isLeftIndicatorOn;


    [Header("TrafficLightSignal Colors")]
    [SerializeField] Color defaultColor;
    [SerializeField] Color lightUpColor;

    [SerializeField] GameObject fpsModeCamera;//Fps mode camera to enable side and back mirror

    List<AudioSource> audioSources = new List<AudioSource>();
    #region UnityMethods
    private void Start()
    {
        currentPlayerLane = PlayerLane.None;
        //To stop all audioSources on Pause
        AudioSource[] audioS = GetComponentsInChildren<AudioSource>();
        audioSources = audioS.ToList();
    }

    private void Update()
    {
        if (SceneManager.GetActiveScene().buildIndex != 1)
            return;

        FPPModeEnable();
        if (!dontCastRayOnTrafficSignal)
            CastRay(10f);
        else if (dontCastRayOnTrafficSignal)
            ShowandChangeTrafficLightsOnScreen(fronthit.normal);
        CheckBackHitForTurns();
        ResetplayerStopAtStopSign();
        PlayerCausingDamage();
    }
    #endregion
    #region OnCollisions


    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == StringConstants.aiCarLayer)
        {
            crashOtherCars++;
            if (crashOtherCars == 1)
            {
                CreateNotification(StringConstants.dontCrashOtherCars, 50, false, "");
                InstructionManager.instance.GiveInstruction(InstructionType.Crash);
                Invoke("ResetCollisions", 3f);
            }
        }
        if (collision.gameObject.tag == StringConstants.obstacles)
        {
            crashObstacles++;
            if (crashObstacles == 1)
            {
                CreateNotification(StringConstants.dontDamageCar, 80, false, "");
                Invoke("ResetCollisions", 3f);
            }
        }
    }
    void ResetCollisions()
    {
        crashObstacles = 0;
        crashOtherCars = 0;
    }
    #endregion

    #region OnTriggers
    private void OnTriggerEnter(Collider other)
    {
        //Stop Sign
        if (RCC_SceneManager.Instance.activePlayerVehicle != null)
        {
            float angle = Vector3.Angle(other.gameObject.transform.forward, RCC_SceneManager.Instance.activePlayerVehicle.transform.forward);
            // means he is going forward
            if (other.tag == StringConstants.stopSign && RCC_SceneManager.Instance.activePlayerVehicle.direction > 0)
            {
                if (angle < offsetAngle)
                {
                    isStopSignBoard = true;
                    StartCoroutine(ICheckPlayerAtStopBoard());
                }
                else
                {
                }
            }
        }
        //Stop Sign
        //Check if the player had Changed the Lane
        if (other.tag == StringConstants.laneOne)
        {
            if (!isFirstLane && !isSecondLane)
                isFirstLane = true;
            if (currentPlayerLane == PlayerLane.lane2)
            {
                if (!isFirstLane)
                    CreateNotification(StringConstants.wrongLane, 50, false, StringConstants.wrongLane1_ID);
                isTouchedFirstLane = true;
            }
            else
            {
                currentPlayerLane = PlayerLane.lane1;
            }
        }
        else if (other.tag == StringConstants.laneTwo)
        {
            if (!isSecondLane && !isFirstLane)
                isSecondLane = true;
            if (currentPlayerLane == PlayerLane.lane1)
            {
                if (!isSecondLane)
                    CreateNotification(StringConstants.wrongLane, 50, false, StringConstants.wrongLane2_ID);
                isTouchedSecondlane = true;
            }
            else
            {
                currentPlayerLane = PlayerLane.lane2;
            }
        }



        //Check if the player had Changed the Lane


        //Check if the player had crossed the traffic signal on red or green signal 
        if (other.tag == StringConstants.intersection)
        {
            Intersection intersection = other.GetComponent<Intersection>();
            if (intersection != null && intersection.intersectionType == IntersectionType.TRAFFIC_LIGHT)
            {
                if (isIntersectionEntered)
                {
                    return;
                }
                isIntersectionEntered = true;
                if (currentTrafficSignal == TrafficLightSignal.RED)
                {
                    isRedSignal_Enter = true;
                }
                else
                {
                    isRedSignal_Enter = false;
                }
            }

            //Reset the lanes when we touch intersection
            isSecondLane = false;
            isFirstLane = false;
        }
        //Check if the player had crossed the traffic signal on red or green signal 
    }

    IEnumerator SetIntersectionBooleansFalse()
    {
        yield return new WaitForSeconds(0.6f);
        isIntersectionEntered = false;
        isIntersectionExit = false;
    }
    private void OnTriggerExit(Collider other)
    {
        //Check if the player had Changed the Lane
        if (other.CompareTag(StringConstants.laneOne))
        {
            if (isTouchedSecondlane)
            {
                currentPlayerLane = PlayerLane.lane2;
                //  isSecondLane = true;
                //  isFirstLane = false;
                isTouchedSecondlane = false;
            }
            else if (isSecondLane == false)
            {
                currentPlayerLane = PlayerLane.None;
                //  isFirstLane = false;
            }
            isTouchedFirstLane = false;
        }
        else if (other.tag == StringConstants.laneTwo)
        {
            if (isTouchedFirstLane)
            {
                currentPlayerLane = PlayerLane.lane1;
                // isFirstLane = true;
                //   isSecondLane = false;
                isTouchedFirstLane = false;
            }
            else if (isFirstLane == false)
            {
                currentPlayerLane = PlayerLane.None;
                // isSecondLane = false;
            }
            isTouchedSecondlane = false;
        }
        //Check if the player had Changed the Lane

        //Stop Sign
        if (RCC_SceneManager.Instance.activePlayerVehicle != null)
        {
            float angle = Vector3.Angle(other.gameObject.transform.forward, RCC_SceneManager.Instance.activePlayerVehicle.transform.forward);
            if (other.tag == StringConstants.stopSign && RCC_SceneManager.Instance.activePlayerVehicle.direction > 0)
            {
                isStopSignBoard = false;
                if (angle < offsetAngle)
                {
                    if (isPlayerStopped)
                    {
                        return;
                    }
                    if (!isPlayerStopped)
                    {
                        //When player first triggered to the collider then increment and reset the playerstop value
                        playerStopVal++;
                        if (playerStopVal == 1 || playerStopVal == 2)
                            CreateNotification(StringConstants.playerNotStopped, 60, false, StringConstants.playerNotStopped_ID);
                    }
                }
            }
        }
        //Stop Sign

        //Speedlimit Sign 
        if (other.tag == StringConstants.speedSign && RCC_SceneManager.Instance.activePlayerVehicle.direction > 0)
        {
            SpeedLimit speedLimit = other.GetComponent<SpeedLimit>();
            if (speedLimit.speedLimit < RCC_SceneManager.Instance.activePlayerVehicle.speed)
            {
                CreateNotification(StringConstants.highSpeed, 30, false, StringConstants.highSpeed_ID);
            }
        }
        //Speedlimit Sign 

        //Check Player Traffic Light Point
        if (other.tag == StringConstants.intersection)
        {
            checkBackHit = true;
            IntersectionOntriggerExit();
            StartCoroutine(ICheckPlayerBlinkers());
            Intersection intersection = other.GetComponent<Intersection>();
            if (intersection != null && intersection.intersectionType == IntersectionType.TRAFFIC_LIGHT)
            {
                if (isIntersectionExit)
                {
                    return;
                }
                isIntersectionExit = true;

                if (currentTrafficSignal == TrafficLightSignal.RED)
                {
                    isRedSignal_Exit = true;
                }
                else
                {
                    isRedSignal_Exit = false;
                }
                StartCoroutine(IETrafficCross());
            }
        }

        if (other.tag == StringConstants.trafficScreen)
        {
            IntersectionOntriggerExit();
        }
        //Check Player Traffic Light Point
    }
    #endregion
    void IntersectionOntriggerExit()
    {
        //if traffic lights are turned on in the Screen , then deactivate them
        dontCastRayOnTrafficSignal = false;
        GameManager.instance.uIManager.playerScreenTrafficLights.SetActive(false);
        SetSignalsToDefaultColour();
    }


    #region TrafficLights
    void SetSignalsToDefaultColour()
    {
        GameManager.instance.uIManager.greenSignal.color = defaultColor;
        GameManager.instance.uIManager.redSignal.color = defaultColor;
        GameManager.instance.uIManager.yellowSignal.color = defaultColor;
    }
    private void ShowandChangeTrafficLightsOnScreen(Vector3 normal)
    {
        if (frontNormal.x == 1)
        {
            TrafficLightDirectionCheck(intersectionObj.isRightSideRed);
        }
        else if (frontNormal.x == -1)
        {
            TrafficLightDirectionCheck(intersectionObj.isLeftSideRed);
        }
        else if (frontNormal.z == 1)
        {
            TrafficLightDirectionCheck(intersectionObj.isFrontSideRed);
        }
        else if (frontNormal.z == -1)
        {
            TrafficLightDirectionCheck(intersectionObj.isBackSideRed);
        }
        SetSignalsToDefaultColour();

        switch (currentTrafficSignal)
        {
            case TrafficLightSignal.GREEN:
                GameManager.instance.uIManager.greenSignal.color = lightUpColor;
                break;
            case TrafficLightSignal.YELLOW:
                GameManager.instance.uIManager.yellowSignal.color = lightUpColor;
                break;
            case TrafficLightSignal.RED:
                GameManager.instance.uIManager.redSignal.color = lightUpColor;
                break;
            default:
                break;
        }
    }

    void TrafficLightDirectionCheck(bool _lightDirection)
    {
        if (_lightDirection)
            currentTrafficSignal = intersectionObj.currentSignal1;
        else
            currentTrafficSignal = intersectionObj.currentSignal2;
    }

    IEnumerator IETrafficCross()
    {
        yield return new WaitForSeconds(0.65f);
        if (isTrafficCrossed == true)
            if (isRedSignal_Enter == true && isRedSignal_Exit == true) //If entered and exit are red lights then deduct points
            {
                CreateNotification(StringConstants.redSignal, 50, false, "");
            }
            else
            {
                CreateNotification(StringConstants.greenSignal, 30, true, "");
            }

        // currentTrafficSignal = TrafficLightSignal.NONE; //Resetting the traffic screen
        StartCoroutine(SetIntersectionBooleansFalse()); //Reset the booleans
    }
    #endregion

    #region Blinkers
    //Blinkers
    IEnumerator ICheckPlayerBlinkers()
    {
        if (isEnteredBlinkers)
        {
            yield break;
        }
        isEnteredBlinkers = true;
        //Storing the first trigger entered normal values to check if he crossed the signals
        storeFrontRayVector = frontNormal;
        storeBackRayVector = backNormal;

        yield return new WaitForSeconds(0.5f);

        if (frontNormal != new Vector3(0, 0, 0) || backNormal != new Vector3(0, 0, 0))
        {
            //Blinkers Check 
            //Hitting the player ray to collider and taking the normal of local position from hitpoint to collider to check the player turn direction
            if (isRightIndicatorOn)
            {
                if ((backNormal.z == 1 && frontNormal.x == 1) || (frontNormal.z == 1 && backNormal.x == -1) ||
                   (frontNormal.z == -1 && backNormal.x == 1) || (frontNormal.x == -1 && backNormal.z == -1) ||
                   //player doesnt show the backside to the intersection Collider
                   (frontNormal.z == 1 && storeFrontRayVector.x == 1) || (frontNormal.x == -1 && storeFrontRayVector.z == 1) ||
                   (frontNormal.z == -1 && storeFrontRayVector.x == -1) || (frontNormal.x == 1 && storeFrontRayVector.z == -1) ||

                    //player doesnt show the frontside to the intersection Collider
                    (backNormal.z == 1 && storeBackRayVector.x == 1) || (backNormal.x == -1 && storeBackRayVector.z == 1) ||
                   (backNormal.z == -1 && storeBackRayVector.x == -1) || (backNormal.x == 1 && storeBackRayVector.z == -1)
                   )
                {
                    CreateNotification(StringConstants.rightBlinkers, 20, true, "");
                }
                else
                {
                    CreateNotification(StringConstants.wrongIndicators, 15, false, "");
                    InstructionManager.instance.GiveInstruction(InstructionType.Blinkers);

                }
            }
            else if (isLeftIndicatorOn)
            {
                if ((backNormal.z == -1 && frontNormal.x == 1) || (frontNormal.z == 1 && backNormal.x == 1) ||
                    (frontNormal.z == -1 && backNormal.x == -1) || (frontNormal.x == -1 && backNormal.z == 1) ||

                   //player doesnt show the backside to the intersection Collider
                   (frontNormal.z == -1 && storeFrontRayVector.x == 1) || (frontNormal.x == 1 && storeFrontRayVector.z == 1) ||
                   (frontNormal.z == 1 && storeFrontRayVector.x == -1) || (frontNormal.x == -1 && storeFrontRayVector.z == -1) ||

                    //player doesnt show the frontside to the intersection Collider
                    (backNormal.z == -1 && storeBackRayVector.x == 1) || (backNormal.x == 1 && storeBackRayVector.z == 1) ||
                   (backNormal.z == 1 && storeBackRayVector.x == -1) || (backNormal.x == -1 && storeBackRayVector.z == -1))
                {
                    CreateNotification(StringConstants.leftBlinkers, 20, true, "");
                }
                else
                {
                    CreateNotification(StringConstants.wrongIndicators, 15, false, "");
                    InstructionManager.instance.GiveInstruction(InstructionType.Blinkers);


                }
            }
            else if (frontNormal.x == -backNormal.x || frontNormal.z == -backNormal.z)
            {
                //player havent took any turn
            }
            else
            {
                CreateNotification(StringConstants.switchOnIndicators, 15, false, "");
            }
        }

        //Ashish 
        //Check if player crossed the traffic lane
        if ((frontNormal == storeFrontRayVector && RCC_SceneManager.Instance.activePlayerVehicle.direction < 0) ||
    (storeFrontRayVector == backNormal && RCC_SceneManager.Instance.activePlayerVehicle.direction > 0) ||
    (storeBackRayVector == backNormal && storeBackRayVector != new Vector3(0, 0, 0) && RCC_SceneManager.Instance.activePlayerVehicle.direction > 0))
        {
            print("he is back");
            isTrafficCrossed = false;
        }
        else
        {
            isTrafficCrossed = true;
        }
        //Ashish
        StartCoroutine(ISetEnteredBlinkersBooleansFalse());
        IntersectionOntriggerExit();

        //Reset blinkers Data
        checkBackHit = false;
        frontNormal = Vector3.zero;
        backNormal = Vector3.zero;
        isLeftIndicatorOn = false;
        isRightIndicatorOn = false;
        RCC_SceneManager.Instance.activePlayerVehicle.indicatorsOn = RCC_CarControllerV3.IndicatorsOn.Off;
        //Reset blinkers Data
    }
    void CheckBackHitForTurns()
    {
        if (RCC_SceneManager.Instance.activePlayerVehicle != null)
            if (checkBackHit || RCC_SceneManager.Instance.activePlayerVehicle.direction < 0)
                if (Physics.Raycast(transform.position, -transform.forward, out backhit, 15f, intersectionLayer))
                {
                    backNormal = backhit.normal;
                    backNormal = backhit.transform.TransformDirection(backNormal);
                }
    }
    //Blinkers
    #endregion


    #region StopSign
    //Player Stop At stopSign
    IEnumerator ICheckPlayerAtStopBoard()
    {
        yield return new WaitForSeconds(0.1f);
        while (isStopSignBoard)
        {
            if (RCC_SceneManager.Instance.activePlayerVehicle.speed <= 0.2f)
            {
                isPlayerStopped = true;
                CreateNotification(StringConstants.playerStopped, 80, true, StringConstants.playerStopped_ID);
                yield break;
            }
            yield return new WaitForSeconds(Time.deltaTime);
        }
    }
    void ResetplayerStopAtStopSign()
    {
        //Make the playerstop bool to false
        RaycastHit playerStopHit;
        if (Physics.Raycast(raycastPoint.position, transform.forward, out playerStopHit, 20f, LayerMask.GetMask(StringConstants.stopSign)))
        {
            if (playerStopHit.collider.tag == StringConstants.stopSign)
            {
                if (playerStopHit.collider != null)
                    if (Vector3.Distance(transform.position, playerStopHit.collider.transform.position) > 4f)
                    {
                        playerStopVal = 0;
                        isPlayerStopped = false;
                    }
            }
        }
    }
    //Player Stop At stopSign
    #endregion

    #region Audio
    public void StopAllAudioSources()//Stop all audios when paused
    {
        foreach (var item in audioSources)
        {
            item.Stop();
        }
    }
    #endregion


    //If Camera Switched To FPP Mode
    void FPPModeEnable()
    {
        if (RCC_SceneManager.Instance.activePlayerCamera.cameraMode == RCC_Camera.CameraMode.FPS)
        {
            fpsModeCamera.gameObject.SetActive(true);
        }
        else
        {
            fpsModeCamera.gameObject.SetActive(false);
        }
    }

    //RayCast
    void CastRay(float raylength)
    {
        if (Physics.Raycast(raycastPoint.position, transform.forward, out fronthit, raylength, intersectionLayer))
        {
            intersectionObj = fronthit.collider.GetComponent<Intersection>();
            if (intersectionObj != null)
            {
                frontNormal = fronthit.normal;
                frontNormal = fronthit.transform.TransformDirection(frontNormal);

                if (GameManager.instance != null)
                    if (intersectionObj.intersectionType == IntersectionType.TRAFFIC_LIGHT)
                    {
                        //Enable  traffic lights to show them  on the player screen
                        GameManager.instance.uIManager.playerScreenTrafficLights.SetActive(true);
                        dontCastRayOnTrafficSignal = true;
                    }
            }
        }

    }

    //If player Causing Damage to Cars By Wrong Lane Driving
    void PlayerCausingDamage()
    {
        if (Physics.Raycast(raycastPoint.position, transform.forward, out playerCauseDamageHit, damageCauserayLength, LayerMask.GetMask(StringConstants.aiCarLayer)))
        {
            if (playerCauseDamageHit.collider.tag == StringConstants.aiCarLayer)
            {
                if (playerCauseDamageHit.collider != null)
                {
                    float angle = Vector3.Angle(transform.forward, playerCauseDamageHit.collider.transform.forward);
                    float distance = Vector3.Distance(transform.position, playerCauseDamageHit.collider.transform.position);
                    VehicleAI aiCar = playerCauseDamageHit.collider.GetComponent<VehicleAI>();

                    if (aiCar != null)
                        if (damageCauseDistance == (int)(distance) && aiCar.RIGID.velocity.magnitude >= damageCauseAiCarSpeed &&
                           (angle > 160) && RCC_SceneManager.Instance.activePlayerVehicle.speed >= damageCauseSpeed)
                        {

                            CreateNotification(StringConstants.causingDamage, 150, false, StringConstants.causingDamage_ID);
                        }
                }
            }
        }
    }

    //Code By Ashish
    IEnumerator ISetEnteredBlinkersBooleansFalse()
    {
        yield return new WaitForSeconds(0.6f);
        isEnteredBlinkers = false;
    }

    //NotificationPopUps
    void CreateNotification(string message, int points, bool isBool, string id)
    {
        //Make id is equal to (""), if you want to call the notification once
        NotificationContentView.instance.CreateNotification(message, points, isBool, id);
    }
}
//Code By Ashish
