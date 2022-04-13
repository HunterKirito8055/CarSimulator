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
using UnityEngine.EventSystems;

/// <summary>
/// UI input (float) receiver from UI Button. 
/// </summary>
[AddComponentMenu("BoneCracker Games/Realistic Car Controller/UI/Mobile/RCC UI Controller Button")]
public class RCC_UIController : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{

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

    private Button button;
    private Slider slider;

    internal float input;
    private float sensitivity { get { return RCCSettings.UIButtonSensitivity; } }
    private float gravity { get { return RCCSettings.UIButtonGravity; } }
    public bool pressing;

    void Awake()
    {
        button = GetComponent<Button>();
        slider = GetComponent<Slider>();
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        //Ashish Code
        if (!GameManager.instance.isCheckatPlayerStart)
        {
            if (gameObject.name == "Gas")
                GameManager.instance.uIManager.CheckPlayerStartPoints();
        }
        else
        {

            //FuelSystem.instance.VehicleMovingStatus(true);
            pressing = true;
        }
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        pressing = false;
        //FuelSystem.instance.VehicleMovingStatus(false);
    }

    void OnPress(bool isPressed)
    {

        if (isPressed)
            pressing = true;
        else
            pressing = false;

    }

    void Update()
    {

        if (button && !button.interactable)
        {

            pressing = false;
            input = 0f;
            return;

        }

        //There is no slider attached to this object,so this condition doesnt work Ashish Code 12/08
        if (slider && !slider.interactable)
        {

            pressing = false;
            input = 0f;
            slider.value = 0f;
            return;

        }

        if (slider)
        {

            if (pressing)
                input = slider.value;
            else
                input = 0f;

            slider.value = input;

        }
        else
        {

            if (pressing)
                input += Time.deltaTime * sensitivity;
            else
                input -= Time.deltaTime * gravity;

        }

        if (input < 0f)
            input = 0f;

        if (input > 1f)
            input = 1f;

    }

    void OnDisable()
    {

        input = 0f;
        pressing = false;

    }

}
