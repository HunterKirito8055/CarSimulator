//----------------------------------------------
// Realistic Car Controller
//
// Copyright © 2014 - 2020 BoneCracker Games
// http://www.bonecrackergames.com
// Buğra Özdoğanlar
//
//----------------------------------------------

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// A simple example manager for how the car selection scene should work.
/// </summary>
public class RCC_CarSelectionExample : MonoBehaviour
{
    [HideInInspector]
    //public List<RCC_CarControllerV3> _spawnedVehicles = new List<RCC_CarControllerV3> (); // Our spawned vehicle list. No need to instantiate same vehicles over and over again.
    public List<VehicleData> vehiclesData = new List<VehicleData>();
    public List<VehicleData> carData = new List<VehicleData>();
    public List<VehicleData> busData = new List<VehicleData>();
    public List<VehicleData> towData = new List<VehicleData>();

    public Transform spawnPosition; // Spawn transform.
    private int selectedIndex; // Selected vehicle index. Next and previous buttons are affecting this value.

    public RCC_Camera RCCCamera; // Enabling / disabling camera selection script on RCC Camera if choosen.
    public string nextScene;

    public int SelectedIndex
    {
        get { return selectedIndex; }
        set
        {
            selectedIndex = value;
            GameManager.instance.uIManager.UpdateCarLockingStatus(value);
        }
    }

    void Start()
    {

        // Getting RCC Camera.
        if (!RCCCamera)
            RCCCamera = GameObject.FindObjectOfType<RCC_Camera>();

        // First, we are instantiating all vehicles and store them in _spawnedVehicles list.
        CreateVehicles();
        //SelectedIndex = 0;
    }
    [SerializeField] Transform carSpawn;
    [SerializeField] Transform busSpawn;
    [SerializeField] Transform towSpawn;

    private void CreateVehicles()
    {
        //if (StringConstants.selectedVehicleMode == VehicleMode.Normalvehicle)
        {
            for (int i = 0; i < RCC_DemoVehicles.Instance.vehiclesNormal.Length; i++)
            {
                spawnPosition = carSpawn;
                // Spawning the vehicle with no controllable, no player, and engine off. We don't want to let player control the vehicle while in selection menu.

                RCC_CarControllerV3 spawnedVehicle = RCC.SpawnRCC(RCC_DemoVehicles.Instance.vehiclesNormal[i].RCC_carController, spawnPosition.position, spawnPosition.rotation, false, false, false);

                VehicleData presentVehicleData = new VehicleData();

                if (spawnedVehicle != null)
                {
                    presentVehicleData.RCC_carController = spawnedVehicle;
                    presentVehicleData.vehicleName = RCC_DemoVehicles.Instance.vehiclesNormal[i].vehicleName;
                    presentVehicleData.vehiclePrice = RCC_DemoVehicles.Instance.vehiclesNormal[i].vehiclePrice;
                    presentVehicleData.torque = spawnedVehicle.maxEngineTorque;
                    presentVehicleData.traction = spawnedVehicle.wheelTypeChoise.ToString();
                    presentVehicleData.power = spawnedVehicle.maxspeed;
                    presentVehicleData.weight = spawnedVehicle.GetComponent<Rigidbody>().mass;
                }

                //vehiclesData.Add(presentVehicleData);
                carData.Add(presentVehicleData);

                // Disabling spawned vehicle.
                spawnedVehicle.gameObject.SetActive(false);

                // Adding and storing it in _spawnedVehicles list.
                //_spawnedVehicles.Add (spawnedVehicle);
            }
        }
        //else if (StringConstants.selectedVehicleMode == VehicleMode.TowVehicle)
        {
            for (int i = 0; i < RCC_DemoVehicles.Instance.towVehicles.Length; i++)
            {
                spawnPosition = towSpawn;
                //spawnPosition.position = towSpawn.position;
                //spawnPosition.rotation = towSpawn.rotation;
                // Spawning the vehicle with no controllable, no player, and engine off. We don't want to let player control the vehicle while in selection menu.

                RCC_CarControllerV3 spawnedVehicle = RCC.SpawnRCC(RCC_DemoVehicles.Instance.towVehicles[i].RCC_carController, spawnPosition.position, spawnPosition.rotation, false, false, false);

                VehicleData presentVehicleData = new VehicleData();

                if (spawnedVehicle != null)
                {
                    presentVehicleData.RCC_carController = spawnedVehicle;
                    presentVehicleData.vehicleName = RCC_DemoVehicles.Instance.vehiclesNormal[i].vehicleName;
                    presentVehicleData.vehiclePrice = RCC_DemoVehicles.Instance.vehiclesNormal[i].vehiclePrice;
                    presentVehicleData.torque = spawnedVehicle.maxEngineTorque;
                    presentVehicleData.traction = spawnedVehicle.wheelTypeChoise.ToString();
                    presentVehicleData.power = spawnedVehicle.maxspeed;
                    presentVehicleData.weight = spawnedVehicle.GetComponent<Rigidbody>().mass;
                }

                //vehiclesData.Add(presentVehicleData);
                towData.Add(presentVehicleData);

                // Disabling spawned vehicle.
                spawnedVehicle.gameObject.SetActive(false);

                // Adding and storing it in _spawnedVehicles list.
                //_spawnedVehicles.Add (spawnedVehicle);
            }
        }
        //else if (StringConstants.selectedVehicleMode == VehicleMode.Bus)
        {
            for (int i = 0; i < RCC_DemoVehicles.Instance.busVehicles.Length; i++)
            {
                spawnPosition = busSpawn;
                // Spawning the vehicle with no controllable, no player, and engine off. We don't want to let player control the vehicle while in selection menu.

                RCC_CarControllerV3 spawnedVehicle = RCC.SpawnRCC(RCC_DemoVehicles.Instance.busVehicles[i].RCC_carController, spawnPosition.position, spawnPosition.rotation, false, false, false);

                VehicleData presentVehicleData = new VehicleData();

                if (spawnedVehicle != null)
                {
                    presentVehicleData.RCC_carController = spawnedVehicle;
                    presentVehicleData.vehicleName = RCC_DemoVehicles.Instance.vehiclesNormal[i].vehicleName;
                    presentVehicleData.vehiclePrice = RCC_DemoVehicles.Instance.vehiclesNormal[i].vehiclePrice;
                    presentVehicleData.torque = spawnedVehicle.maxEngineTorque;
                    presentVehicleData.traction = spawnedVehicle.wheelTypeChoise.ToString();
                    presentVehicleData.power = spawnedVehicle.maxspeed;
                    presentVehicleData.weight = spawnedVehicle.GetComponent<Rigidbody>().mass;
                }

                //vehiclesData.Add(presentVehicleData);
                busData.Add(presentVehicleData);
                // Disabling spawned vehicle.
                spawnedVehicle.gameObject.SetActive(false);

                // Adding and storing it in _spawnedVehicles list.
                //_spawnedVehicles.Add (spawnedVehicle);
            }

        }




        //SpawnVehicle();

        // If RCC Camera is choosen, it wil enable RCC_CameraCarSelection script. This script was used for orbiting camera.
        if (RCCCamera)
        {

            if (RCCCamera.GetComponent<RCC_CameraCarSelection>())
                RCCCamera.GetComponent<RCC_CameraCarSelection>().enabled = true;

        }

    }

    public void Onselect()
    {
        //index store
        switch (StringConstants.selectedVehicleMode)
        {
            case VehicleMode.Normalvehicle:
                PlayerPrefs.SetInt(StringConstants.carSelectedIndex, SelectedIndex);
                break;
            case VehicleMode.Bus:
                PlayerPrefs.SetInt(StringConstants.busSelectedIndex, SelectedIndex);
                break;
            case VehicleMode.TowVehicle:
                PlayerPrefs.SetInt(StringConstants.towSelectedIndex, SelectedIndex);
                break;
            case VehicleMode.Tutorial:
                break;
            default:
                break;
        }
        //vehicle store
    }
    public void OnGarageClick()
    {

        switch (StringConstants.selectedVehicleMode)
        {
            case VehicleMode.Normalvehicle:
                vehiclesData = carData;
                SelectedIndex = PlayerPrefs.GetInt(StringConstants.carSelectedIndex, 0);
                break;
            case VehicleMode.Bus:
                vehiclesData = busData;
                SelectedIndex = PlayerPrefs.GetInt(StringConstants.busSelectedIndex, 0);
                break;
            case VehicleMode.TowVehicle:
                vehiclesData = towData;
                SelectedIndex = PlayerPrefs.GetInt(StringConstants.towSelectedIndex, 0);
                break;
            case VehicleMode.Tutorial:
                break;
            default:
                break;
        }
        //SelectedIndex = 0;
        SpawnVehicle();
    }
    // Increasing selected index, disabling all other vehicles, enabling current selected vehicle.
    public void NextVehicle()
    {

        // If index exceeds maximum, return to 0.
        if (SelectedIndex == vehiclesData.Count - 1)
            SelectedIndex = 0;
        else
            SelectedIndex++;

        SpawnVehicle();
    }

    // Decreasing selected index, disabling all other vehicles, enabling current selected vehicle.
    public void PreviousVehicle()
    {
        // If index is below 0, return to maximum.
        if (SelectedIndex == 0)
            SelectedIndex = vehiclesData.Count - 1;
        else
            SelectedIndex--;

        SpawnVehicle();
    }

    // Spawns the current selected vehicle.
    public void SpawnVehicle()
    {

        // Disabling all vehicles.
        for (int i = 0; i < vehiclesData.Count; i++)
            vehiclesData[i].RCC_carController.gameObject.SetActive(false);

        // And enabling only selected vehicle.
        vehiclesData[SelectedIndex].RCC_carController.gameObject.SetActive(true);
        //vehiclesData[10].RCC_carController.gameObject.SetActive(true);
        //vehiclesData[11].RCC_carController.gameObject.SetActive(true);

        // RCC_SceneManager.Instance.RegisterPlayer (_spawnedVehicles [selectedIndex], false, false);
        RCC_SceneManager.Instance.activePlayerVehicle = vehiclesData[SelectedIndex].RCC_carController;

    }

    // Registering the spawned vehicle as player vehicle, enabling controllable.
    public void SelectVehicle()
    {

        // Registers the vehicle as player vehicle.
        RCC.RegisterPlayerVehicle(vehiclesData[SelectedIndex].RCC_carController);
        // Starts engine and enabling controllable when selected.
        //Ashish Code
        //_spawnedVehicles[selectedIndex].StartEngine ();
        vehiclesData[SelectedIndex].RCC_carController.SetCanControl(true);
        //Ashish Code
        // Save the selected vehicle for instantianting it on next scene.
        PlayerPrefs.SetInt("SelectedRCCVehicle", SelectedIndex);

        // If RCC Camera is choosen, it will disable RCC_CameraCarSelection script. This script was used for orbiting camera.
        if (RCCCamera)
        {

            if (RCCCamera.GetComponent<RCC_CameraCarSelection>())
                RCCCamera.GetComponent<RCC_CameraCarSelection>().enabled = false;

        }

        if (nextScene != "")
            OpenScene();

    }

    // Deactivates selected vehicle and returns to the car selection.
    public void DeSelectVehicle()
    {

        // De-registers the vehicle.
        RCC.DeRegisterPlayerVehicle();

        // Resets position and rotation.
        vehiclesData[SelectedIndex].RCC_carController.transform.position = spawnPosition.position;
        vehiclesData[SelectedIndex].RCC_carController.transform.rotation = spawnPosition.rotation;

        // Kills engine and disables controllable.
        vehiclesData[SelectedIndex].RCC_carController.KillEngine();
        vehiclesData[SelectedIndex].RCC_carController.SetCanControl(false);

        // Resets the velocity of the vehicle.
        vehiclesData[SelectedIndex].RCC_carController.GetComponent<Rigidbody>().ResetInertiaTensor();
        vehiclesData[SelectedIndex].RCC_carController.GetComponent<Rigidbody>().velocity = Vector3.zero;
        vehiclesData[SelectedIndex].RCC_carController.GetComponent<Rigidbody>().angularVelocity = Vector3.zero;

        // If RCC Camera is choosen, it wil enable RCC_CameraCarSelection script. This script was used for orbiting camera.
        //if (RCCCamera)
        //{

        //    if (RCCCamera.GetComponent<RCC_CameraCarSelection>())
        //        RCCCamera.GetComponent<RCC_CameraCarSelection>().enabled = true;

        //}

    }
    public void OpenScene()
    {

        // Loads next scene.
        SceneManager.LoadScene(nextScene);

    }
}

public class VehicleData
{
    public string vehicleName;
    public RCC_CarControllerV3 RCC_carController;
    public int vehiclePrice;

    public float power; //horse Power (HP)
    public float torque; //newton meter (NM)
    public float weight; //kilograms (KG)
    public string traction; //traction
}