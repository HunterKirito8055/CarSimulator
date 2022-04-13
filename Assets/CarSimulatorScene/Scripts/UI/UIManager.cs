using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [SerializeField] Button engineButton;
    [SerializeField] Button seatBeltButton;
    [SerializeField] Button carWipers;
    [SerializeField] Button fuelButton;

    [SerializeField] Dropdown cameraViewChange;

    //Traffic Lights
    public Image redSignal;
    public Image yellowSignal;
    public Image greenSignal;
    public GameObject playerScreenTrafficLights;
    //Traffic Lights

    //
    [SerializeField] Text gameStatusText;
    [HideInInspector] public bool gameStatus;

    [Space]
    [Header("Fuel")]
    #region FuelManager
    public Text topFuelText;
    public Text panelFuelText;
    [SerializeField] Image fullLifeImage;

    [SerializeField] Button refillFuel;
    [SerializeField] Button refillOneFuel;
    [SerializeField] Button unlimitedFuel;
    public GameObject noFuelPanel;
    public Slider fuelBar;
    #endregion
    //Bhargav
    [SerializeField] private Transform parkingPlaceTransform; //this takes the transform of the parking slots
    [SerializeField] Material parkingMaterial; //the material of the parking material
    Color materialColor; //to change material alpha
    float timeOnArea = 0f;
    bool isCheckingParkingPrecession;
    bool isInParking;
    bool isEngineStarted = true;
    bool isFastenSeatbelt = true;

    int engineCounter;
    int seatBeltCounter;

    float shouldStopfor = 3f;
    [HideInInspector] public string vehicleSpeed;
    private bool parked;

    public float precisionAngle;//this value is updated in precision text to give rewards or grades in future

    //pause
    [SerializeField] GameObject pausebtn;
    [SerializeField] Text countDownText;
    //pause

    //Bhargav
    [Header("Panels")]
    [SerializeField] GameObject completePanel;
    [SerializeField] GameObject pausePanel;
    [SerializeField] GameObject countDownPanel;
    //
    [SerializeField] GameObject nextlevelBtn;
    #region UnityMethods
    private void Awake()
    {
        //Player 
        engineButton.onClick.AddListener(() => EngineStarts());
        seatBeltButton.onClick.AddListener(() => FastenSeatBelt());
        carWipers.onClick.AddListener(() =>
        {
            //  GameManager.instance.Wipers();
        });

        cameraViewChange.onValueChanged.AddListener(delegate
        {
            DropDownCamera(cameraViewChange);
        });

        //Fuel
        if (refillFuel != null)
            refillFuel.onClick.AddListener(() => AddFuel(10));
        if (refillOneFuel != null)
            refillOneFuel.onClick.AddListener(() => AddFuel_RewardedAD());
        if (unlimitedFuel != null)
            unlimitedFuel.onClick.AddListener(() => UnlimitedFuel());
        if (fuelBar != null)
            fuelBar.gameObject.SetActive(false);
        parkingSlider.SetActive(false);

    }
    private void Start()
    {
        parkingMaterial.color = Color.black;
        pausebtn.SetActive(false);
    }
    #endregion
    public void CheckPlayerStartPoints()
    {
        if (engineButton.gameObject.activeSelf)
        {
            engineCounter++;
            if (engineCounter == 1)
            {
                isEngineStarted = false;
                // NotificationContentView.instance.CreateNotification("First Turn the Engine On", 20, false, "");
                InstructionManager.instance.GiveInstruction(InstructionType.EngineStart);
                GameManager.instance.scoreManager.Score -= StringConstants.engine_seatbelt_Points;
            }
        }
        else if (seatBeltButton.gameObject.activeSelf)
        {
            seatBeltCounter++;
            if (seatBeltCounter == 1)
            {
                isFastenSeatbelt = false;
                // NotificationContentView.instance.CreateNotification("Belt Up before Moving", 20, false, "");
                InstructionManager.instance.GiveInstruction(InstructionType.Seatbelt);
                GameManager.instance.scoreManager.Score -= StringConstants.engine_seatbelt_Points;
            }
        }
    }

    void EngineStarts()
    {
        engineButton.gameObject.SetActive(false);
        RCC_SceneManager.Instance.activePlayerVehicle.StartEngine();
        seatBeltButton.gameObject.SetActive(true);
        if (isEngineStarted == true)
            NotificationContentView.instance.CreateNotification(StringConstants.startEngine, 20, true, "");

    }
    void FastenSeatBelt()
    {
        GameManager.instance.isCheckatPlayerStart = true;
        seatBeltButton.gameObject.SetActive(false);
        fuelBar.gameObject.SetActive(true);
        if (isFastenSeatbelt == true)
            NotificationContentView.instance.CreateNotification(StringConstants.seatBelt, 20, true, "");
        pausebtn.SetActive(true);
    }

    //Bhargav
    #region Parking
    [Header("Parking")]
    [SerializeField] GameObject parkingSlider; //this gameobject is to enable the parking progress
    [SerializeField] Image fillImageParkingSlot; //in this image we are filling parking progress\
    float percentage;
    float angle;
    private void CalculateParkingPrecesion()
    {
        angle = Vector3.Angle(parkingPlaceTransform.forward, RCC_SceneManager.Instance.activePlayerVehicle.transform.forward);
        percentage = Mathf.InverseLerp(30f, 2f, angle);
        if (angle >= 135 && angle <= 180)
        {
            parkingSlider.SetActive(false);
            materialColor = parkingMaterial.color;
            materialColor = Color.red;
            materialColor.a = 0.15f;
            parkingMaterial.color = materialColor;
            isInParking = false;
        }
        else
        {
            isInParking = true;
            precisionAngle = (percentage * 100);
            //if (precisionTxt.gameObject.activeSelf)
            // precisionTxt.text = "Precision :" + precisionAngle.ToString("00");
        }
    }

    public void InsideParkingSlot(bool _isInParking)
    {
        isInParking = _isInParking;

        if (isInParking && !isCheckingParkingPrecession)
        {
            CalculateParkingPrecesion();
            if (isInParking)
            {
                StartCoroutine("IStartCheckingParkingPrecession");
            }
        }
        else
        {
            materialColor = parkingMaterial.color;
            materialColor = Color.white;
            materialColor.a = 0.15f;
            parkingMaterial.color = materialColor;
            //parkingMaterial.color = Color.white;
            parkingSlider.SetActive(false);

            isCheckingParkingPrecession = false;
            timeOnArea = 0;
            StopCoroutine("IStartCheckingParkingPrecession");

        }

    }
    IEnumerator IStartCheckingParkingPrecession()
    {
        isCheckingParkingPrecession = true;
        yield return null;

        while (isInParking && !parked)
        {
            parkingSlider.SetActive(true);
            CalculateParkingPrecesion();
            while (vehicleSpeed == "0")
            {
                timeOnArea += Time.deltaTime;
                materialColor = parkingMaterial.color;
                materialColor = Color.blue;
                materialColor.a = 0.15f;
                parkingMaterial.color = materialColor;
                fillImageParkingSlot.fillAmount = timeOnArea / shouldStopfor;
                if (timeOnArea >= shouldStopfor)
                {
                    fillImageParkingSlot.fillAmount = 0f;
                    timeOnArea = 0f;
                    parked = true;

                    materialColor = parkingMaterial.color;
                    materialColor = Color.green;
                    materialColor.a = 0.15f;
                    parkingMaterial.color = materialColor;
                    RCC_SceneManager.Instance.activePlayerVehicle.KillEngine();
                    RCC_SceneManager.Instance.activePlayerVehicle.SetCanControl(false);
                    GameManager.instance.uIManager.carWipers.gameObject.SetActive(false);//Ashish Code

                    //camera.cameraMode = RCC_Camera.CameraMode.CINEMATIC;
                    parkingSlider.SetActive(false);
                    gameStatus = true;
                    CompletePanel(true);


                    isCheckingParkingPrecession = false;
                    StopCoroutine("IStartCheckingParkingPrecession");
                }
                yield return new WaitForSeconds(Time.deltaTime);
            }
            yield return new WaitForSeconds(Time.deltaTime);
        }
        parked = false;
        parkingSlider.SetActive(false);
        isCheckingParkingPrecession = false;
        timeOnArea = 0;
    }

    #endregion
    //Bhargav

    #region Camera
    public void DropDownCamera(Dropdown change)
    {
        switch (change.value)
        {
            case 0:
                RCC_SceneManager.Instance.activePlayerCamera.cameraMode = RCC_Camera.CameraMode.TPS;
                break;
            case 1:
                RCC_SceneManager.Instance.activePlayerCamera.cameraMode = RCC_Camera.CameraMode.WHEEL;
                break;
            case 2:
                RCC_SceneManager.Instance.activePlayerCamera.cameraMode = RCC_Camera.CameraMode.FPS;
                break;
            case 3:
                RCC_SceneManager.Instance.activePlayerCamera.cameraMode = RCC_Camera.CameraMode.BACK;
                break;
            default:
                break;
        }

    }
    #endregion

    #region Fuel
    public void IsFullFuel(bool isTrue)
    {
        if (isTrue)
        {
            if (fullLifeImage != null)
                fullLifeImage.gameObject.SetActive(true);
            if (panelFuelText != null)
                panelFuelText.gameObject.transform.parent.gameObject.SetActive(false);
        }
        else
        {
            if (fullLifeImage != null)
                fullLifeImage.gameObject.SetActive(false);
            if (panelFuelText != null)
                panelFuelText.gameObject.transform.parent.gameObject.SetActive(true);
        }
    }

    void AddFuel(int _val)
    {
        GameManager.instance.fuelManager.AddFuel(_val);
    }
    void AddFuel_RewardedAD()
    {
        if (GameManager.instance.fuelManager.Fuels < 10)
        {
            AdsManager.instance.ShowRewardedAd();
            AdsManager.instance.rewardBtnType = RewardBtnType.Fuel;

        }
    }
    void UnlimitedFuel()
    {
        AddFuel(10);
        GameManager.instance.fuelManager.UnlimitedFuel();
    }
    #endregion
    #region Pause/Resume
    //Bhargav
    public void OnPause()
    {
        AdsManager.instance.ShowBanner();
        pausePanel.SetActive(true);
        Time.timeScale = 0;
        RCC_SceneManager.Instance.activePlayerVehicle.GetComponent<SimulationManager>().StopAllAudioSources();
    }
    public void OnResume()
    {
        AdsManager.instance.HideBanner();
        pausePanel.SetActive(false);
        countDownPanel.SetActive(true);
        StartCoroutine(IResume());
    }
    IEnumerator IResume()
    {
        int countDown = 3;
        while (countDown > 0)
        {
            countDownText.text = countDown.ToString();
            countDown--;
            yield return new WaitForSecondsRealtime(1f);
        }
        countDownPanel.SetActive(false);
        Time.timeScale = 1;
    }
    #endregion
    public void CompletePanel(bool onOrOff)
    {
        completePanel.SetActive(onOrOff);
        if (gameStatus)
        {
            gameStatusText.text = "Game Win";
            nextlevelBtn.SetActive(true);
            LevelTargetSystem currentVehicleChapter = GameManager.instance.levelTargetSystem;
            if (GameMaster.instance.selected_LevelNumber >= currentVehicleChapter.currentLevel[currentVehicleChapter.GetVehicleTypeChapter()].levelTargets.Count - 1)
            {
                nextlevelBtn.SetActive(false);
            }
            GameMaster.instance.ShowInterstAd_AtWin();
        }
        else
        {
            gameStatusText.text = "Game Lost";
            nextlevelBtn.SetActive(false);
            GameMaster.instance.ShowInterstAd_AtFail();
        }
    }
    //Bhargav
    #region VehicleUnlocking
    int selectedVehicleIndex;
    [Space(10)]
    [Header("VehicleUnlocking")]

    [SerializeField] RCC_CarSelectionExample rCC_CarSelectionExample;

    [SerializeField] Button select;
    [SerializeField] Button purchase;
    [SerializeField] GameObject lockImage;

    //vehicleData
    [SerializeField] Text carNameText;
    [SerializeField] Text purchaseText;
    [SerializeField] Text powertext;
    [SerializeField] Text torqueText;
    [SerializeField] Text tractionText;
    [SerializeField] Text weightText;

    public void UpdateCarLockingStatus(int _selectedvehicleIndex)
    {
        selectedVehicleIndex = _selectedvehicleIndex;
        if (_selectedvehicleIndex > PlayerPrefs.GetInt(StringConstants.unlockedCar + _selectedvehicleIndex, 0))
        {
            //lock on
            select.gameObject.SetActive(false);
            purchase.gameObject.SetActive(true);
            lockImage.SetActive(true);

            purchaseText.text = "Purchase : " + rCC_CarSelectionExample.vehiclesData[selectedVehicleIndex].vehiclePrice;

        }
        else
        {
            select.gameObject.SetActive(true);
            purchase.gameObject.SetActive(false);
            lockImage.SetActive(false);
        }
        UpdateVehicleData();
    }

    public void OnCarUnlocking()
    {
        //check coins is enough or not
        select.gameObject.SetActive(true);
        purchase.gameObject.SetActive(false);
        lockImage.SetActive(false);

        //reduce coins here

        PlayerPrefs.SetInt(StringConstants.unlockedCar + selectedVehicleIndex, selectedVehicleIndex);
    }

    void UpdateVehicleData()
    {
        carNameText.text = rCC_CarSelectionExample.vehiclesData[selectedVehicleIndex].vehicleName;
        torqueText.text = "torque : \n" + rCC_CarSelectionExample.vehiclesData[selectedVehicleIndex].torque.ToString();
        tractionText.text = "traction : \n" + rCC_CarSelectionExample.vehiclesData[selectedVehicleIndex].traction;
        powertext.text = "power : \n" + rCC_CarSelectionExample.vehiclesData[selectedVehicleIndex].power.ToString();
        weightText.text = "weight : \n" + rCC_CarSelectionExample.vehiclesData[selectedVehicleIndex].weight.ToString();
    }
    #endregion

}