//----------------------------------------------
//            Realistic Car Controller
//
// Copyright © 2014 - 2020 BoneCracker Games
// http://www.bonecrackergames.com
// Buğra Özdoğanlar
//
//----------------------------------------------

using UnityEngine;
using UnityEngine.UI;
using System.Collections;

/// <summary>
/// Receiving inputs from UI buttons, and feeds active vehicles on your scene.
/// </summary>
[AddComponentMenu("BoneCracker Games/Realistic Car Controller/UI/Mobile/RCC UI Mobile Buttons")]
public class RCC_MobileButtons : RCC_Core
{

    #region Singleton
    private static RCC_MobileButtons instance;
    public static RCC_MobileButtons Instance { get { if (instance == null) instance = GameObject.FindObjectOfType<RCC_MobileButtons>(); return instance; } }
    #endregion

    // Getting an Instance of Main Shared RCC Settings.
    #region RCC Settings Instance

    private RCC_Settings RCCSettingsInstance;
    private RCC_Settings RCCSettings
    {
        get
        {
            if (RCCSettingsInstance == null)
            {
                RCCSettingsInstance = RCC_Settings.Instance;
                return RCCSettingsInstance;
            }
            return RCCSettingsInstance;
        }
    }

    #endregion


    public ActiveUI_Buttons currentActiveUIButtons;
    public ActiveUI_Buttons left_UI_buttons;
    public ActiveUI_Buttons right_UI_buttons;


    //public RCC_UIController gasButton;
    //public RCC_UIController gradualGasButton;
    //public RCC_UIController brakeButton;
    //public RCC_UIController leftButton;
    //public RCC_UIController rightButton;
    //public RCC_UISteeringWheelController steeringWheel;
    //public RCC_UIController handbrakeButton;
    //public RCC_UIController NOSButton;
    //public RCC_UIController NOSButtonSteeringWheel;
    //public GameObject gearButton;
    //public RCC_UIJoystick joystick;

    public RCC_Inputs inputs = new RCC_Inputs();

    private float throttleInput = 0f;
    private float brakeInput = 0f;
    private float leftInput = 0f;
    private float rightInput = 0f;
    private float steeringWheelInput = 0f;
    private float handbrakeInput = 0f;
    private float boostInput = 1f;
    private float gyroInput = 0f;
    private float joystickInput = 0f;
    private float horizontalInput;
    private float verticalInput;
    private bool canUseNos = false;

    private Vector3 orgBrakeButtonPos;

    void Start()
    {

        if (currentActiveUIButtons.brakeButton)
            orgBrakeButtonPos = currentActiveUIButtons.brakeButton.transform.position;

        CheckController();

        if (RCCSettings.streeringSide == RCC_Settings.StreeringSide.Left)
            currentActiveUIButtons = left_UI_buttons;
        else
            currentActiveUIButtons = right_UI_buttons;

        if (RCCSettings.mobileController == RCC_Settings.MobileController.SteeringWheel)
            currentActiveUIButtons.indicators.SetActive(false);
        else
            currentActiveUIButtons.indicators.SetActive(true);
    }

    void OnEnable()
    {

        RCC_SceneManager.OnControllerChanged += CheckController;
        RCC_SceneManager.OnVehicleChanged += CheckController;

    }

    private void CheckController()
    {

        if (!RCC_SceneManager.Instance.activePlayerVehicle)
            return;

        if (RCCSettings.selectedControllerType == RCC_Settings.ControllerType.Mobile)
        {

            EnableButtons();
            return;

        }
        else
        {

            DisableButtons();
            return;

        }

    }

    void DisableButtons()
    {

        if (currentActiveUIButtons.gasButton)
            currentActiveUIButtons.gasButton.gameObject.SetActive(false);
        if (currentActiveUIButtons.gradualGasButton)
            currentActiveUIButtons.gradualGasButton.gameObject.SetActive(false);
        if (currentActiveUIButtons.leftButton)
            currentActiveUIButtons.leftButton.gameObject.SetActive(false);
        if (currentActiveUIButtons.rightButton)
            currentActiveUIButtons.rightButton.gameObject.SetActive(false);
        if (currentActiveUIButtons.brakeButton)
            currentActiveUIButtons.brakeButton.gameObject.SetActive(false);
        if (currentActiveUIButtons.steeringWheel)
            currentActiveUIButtons.steeringWheel.gameObject.SetActive(false);
        if (currentActiveUIButtons.handbrakeButton)
            currentActiveUIButtons.handbrakeButton.gameObject.SetActive(false);
        if (currentActiveUIButtons.NOSButton)
            currentActiveUIButtons.NOSButton.gameObject.SetActive(false);
        if (currentActiveUIButtons.NOSButtonSteeringWheel)
            currentActiveUIButtons.NOSButtonSteeringWheel.gameObject.SetActive(false);
        if (currentActiveUIButtons.gearButton)
            currentActiveUIButtons.gearButton.gameObject.SetActive(false);
        if (currentActiveUIButtons.joystick)
            currentActiveUIButtons.joystick.gameObject.SetActive(false);

    }

    void EnableButtons()
    {

        if (currentActiveUIButtons.gasButton)
            currentActiveUIButtons.gasButton.gameObject.SetActive(true);
        //			if (gradualGasButton)
        //				gradualGasButton.gameObject.SetActive (true);
        if (currentActiveUIButtons.leftButton)
            currentActiveUIButtons.leftButton.gameObject.SetActive(true);
        if (currentActiveUIButtons.rightButton)
            currentActiveUIButtons.rightButton.gameObject.SetActive(true);
        if (currentActiveUIButtons.brakeButton)
            //brakeButton.gameObject.SetActive (true);
            if (currentActiveUIButtons.steeringWheel)
                currentActiveUIButtons.steeringWheel.gameObject.SetActive(true);
        if (currentActiveUIButtons.handbrakeButton)
            currentActiveUIButtons.handbrakeButton.gameObject.SetActive(true);

        if (canUseNos)
        {

            if (currentActiveUIButtons.NOSButton)
                currentActiveUIButtons.NOSButton.gameObject.SetActive(true);
            if (currentActiveUIButtons.NOSButtonSteeringWheel)
                currentActiveUIButtons.NOSButtonSteeringWheel.gameObject.SetActive(true);

        }

        if (currentActiveUIButtons.joystick)
            currentActiveUIButtons.joystick.gameObject.SetActive(false); //true

    }

    void Update()
    {

        if (RCCSettings.selectedControllerType != RCC_Settings.ControllerType.Mobile)
            return;

        switch (RCCSettings.mobileController)
        {

            case RCC_Settings.MobileController.TouchScreen:

                gyroInput = 0f;

                if (currentActiveUIButtons.steeringWheel && currentActiveUIButtons.steeringWheel.gameObject.activeInHierarchy)
                    currentActiveUIButtons.steeringWheel.gameObject.SetActive(false);

                //if (currentActiveUIButtons.NOSButton && currentActiveUIButtons.NOSButton.gameObject.activeInHierarchy != canUseNos)
                    //NOSButton.gameObject.SetActive(canUseNos);

                    if (currentActiveUIButtons.joystick && currentActiveUIButtons.joystick.gameObject.activeInHierarchy)
                        currentActiveUIButtons.joystick.gameObject.SetActive(false);

                if (!currentActiveUIButtons.leftButton.gameObject.activeInHierarchy)
                {

                    currentActiveUIButtons.brakeButton.transform.position = orgBrakeButtonPos;
                    currentActiveUIButtons.leftButton.gameObject.SetActive(true);

                }

                if (!currentActiveUIButtons.rightButton.gameObject.activeInHierarchy)
                    currentActiveUIButtons.rightButton.gameObject.SetActive(true);

                break;

            case RCC_Settings.MobileController.Gyro:
                gyroInput = Mathf.Lerp(gyroInput, (Input.GetAxis("Horizontal") + Input.acceleration.x) * RCCSettings.gyroSensitivity, Time.deltaTime * 5f);
                currentActiveUIButtons.brakeButton.transform.position = currentActiveUIButtons.leftButton.transform.position;

                if (currentActiveUIButtons.steeringWheel && currentActiveUIButtons.steeringWheel.gameObject.activeInHierarchy)
                    currentActiveUIButtons.steeringWheel.gameObject.SetActive(false);

                if (currentActiveUIButtons.NOSButton && currentActiveUIButtons.NOSButton.gameObject.activeInHierarchy != canUseNos)
                    currentActiveUIButtons.NOSButton.gameObject.SetActive(canUseNos);

                if (currentActiveUIButtons.joystick && currentActiveUIButtons.joystick.gameObject.activeInHierarchy)
                    currentActiveUIButtons.joystick.gameObject.SetActive(false);

                if (currentActiveUIButtons.leftButton.gameObject.activeInHierarchy)
                    currentActiveUIButtons.leftButton.gameObject.SetActive(false);

                if (currentActiveUIButtons.rightButton.gameObject.activeInHierarchy)
                    currentActiveUIButtons.rightButton.gameObject.SetActive(false);

                break;

            case RCC_Settings.MobileController.SteeringWheel:

                gyroInput = 0f;

                if (currentActiveUIButtons.steeringWheel && !currentActiveUIButtons.steeringWheel.gameObject.activeInHierarchy)
                {
                    currentActiveUIButtons.steeringWheel.gameObject.SetActive(true);
                    currentActiveUIButtons.brakeButton.transform.position = orgBrakeButtonPos;
                }

                if (currentActiveUIButtons.NOSButton && currentActiveUIButtons.NOSButton.gameObject.activeInHierarchy)
                    currentActiveUIButtons.NOSButton.gameObject.SetActive(false);

                if (currentActiveUIButtons.NOSButtonSteeringWheel && currentActiveUIButtons.NOSButtonSteeringWheel.gameObject.activeInHierarchy != canUseNos)
                    currentActiveUIButtons.NOSButtonSteeringWheel.gameObject.SetActive(canUseNos);

                if (currentActiveUIButtons.joystick && currentActiveUIButtons.joystick.gameObject.activeInHierarchy)
                    currentActiveUIButtons.joystick.gameObject.SetActive(false);

                if (currentActiveUIButtons.leftButton.gameObject.activeInHierarchy)
                    currentActiveUIButtons.leftButton.gameObject.SetActive(false);
                if (currentActiveUIButtons.rightButton.gameObject.activeInHierarchy)
                    currentActiveUIButtons.rightButton.gameObject.SetActive(false);

                break;

            case RCC_Settings.MobileController.Joystick:

                gyroInput = 0f;

                if (currentActiveUIButtons.steeringWheel && currentActiveUIButtons.steeringWheel.gameObject.activeInHierarchy)
                    currentActiveUIButtons.steeringWheel.gameObject.SetActive(false);

                if (currentActiveUIButtons.NOSButton && currentActiveUIButtons.NOSButton.gameObject.activeInHierarchy != canUseNos)
                    currentActiveUIButtons.NOSButton.gameObject.SetActive(canUseNos);

                if (currentActiveUIButtons.joystick && !currentActiveUIButtons.joystick.gameObject.activeInHierarchy)
                {
                    print("afahjafhajhfajhfajhfjhfa");
                    currentActiveUIButtons.joystick.gameObject.SetActive(true); //true
                    currentActiveUIButtons.brakeButton.transform.position = orgBrakeButtonPos;
                }

                if (currentActiveUIButtons.leftButton.gameObject.activeInHierarchy)
                    currentActiveUIButtons.leftButton.gameObject.SetActive(false);

                if (currentActiveUIButtons.rightButton.gameObject.activeInHierarchy)
                    currentActiveUIButtons.rightButton.gameObject.SetActive(false);

                break;

        }


        //if (!reverseGear)
        //{
            throttleInput = GetInput(currentActiveUIButtons.gasButton) + GetInput(currentActiveUIButtons.gradualGasButton);
        
        //}
        //else
        //{
        //    brakeInput = GetInput(gasButton) + GetInput(gradualGasButton);
        //}
        brakeInput = GetInput (currentActiveUIButtons.brakeButton);
        leftInput = GetInput(currentActiveUIButtons.leftButton);
        rightInput = GetInput(currentActiveUIButtons.rightButton);
        handbrakeInput = GetInput(currentActiveUIButtons.handbrakeButton);
        boostInput = Mathf.Clamp((GetInput(currentActiveUIButtons.NOSButton) + GetInput(currentActiveUIButtons.NOSButtonSteeringWheel)), 0f, 1f);

        if (currentActiveUIButtons.steeringWheel)
            steeringWheelInput = currentActiveUIButtons.steeringWheel.input;

        if (currentActiveUIButtons.joystick)
            joystickInput = currentActiveUIButtons.joystick.inputHorizontal;

        FeedRCC();

    }

    private void FeedRCC()
    {

        if (!RCC_SceneManager.Instance.activePlayerVehicle)
            return;

        canUseNos = RCC_SceneManager.Instance.activePlayerVehicle.useNOS;

        inputs.throttleInput = throttleInput;
        inputs.brakeInput = brakeInput;
        inputs.steerInput = -leftInput + rightInput + steeringWheelInput + gyroInput + joystickInput;
        inputs.handbrakeInput = handbrakeInput;
        inputs.boostInput = boostInput;
    }

    // Gets input from button.
    float GetInput(RCC_UIController button)
    {

        if (button == null)
            return 0f;
        return (button.input);
    }

    // Gets input from joystick.
    Vector2 GetInput(RCC_UIJoystick joystick)
    {

        if (joystick == null)
            return Vector2.zero;

        return (joystick.inputVector);

    }

    void OnDisable()
    {

        RCC_SceneManager.OnControllerChanged -= CheckController;
        RCC_SceneManager.OnVehicleChanged -= CheckController;
    }

}
