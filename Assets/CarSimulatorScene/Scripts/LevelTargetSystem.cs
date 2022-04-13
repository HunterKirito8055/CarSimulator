using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelTargetSystem : MonoBehaviour
{
    public VehicleLevelTargets[] californiaLevelTargets;
    public VehicleLevelTargets[] newYorkLevelTargets;

    [SerializeField] Transform playerStartPosition;
    [SerializeField] Transform parkingSlotPosition;

    public VehicleLevelTargets[] currentLevel;
    [SerializeField] Camera miniCamera_LongView;

    [Space(10)]
    [Header("Lessons")]
    [SerializeField] GameObject lessons_Object;
    [SerializeField] GameObject[] lessonsCities;

    [Space(10)]
    [Header("Free Ride")]
    [SerializeField] GameObject freeRide_Object;
    [SerializeField] GameObject[] freeRideCities;
    [SerializeField] Transform[] freeRide_StartPoints;
    [SerializeField] Transform[] freeRide_MinimapPositions;
    [SerializeField] float[] freeRide_MinimapOrthoSize;



    private void Awake()
    {
        int freeRideChapter = GameMaster.instance.freeRide_Chapter;
        if (freeRideChapter != 0)//Open Free Ride
        {
            freeRide_Object.SetActive(true);
            freeRideCities[freeRideChapter - 1].SetActive(true); //Enable the selected city Object
            miniCamera_LongView.orthographicSize = freeRide_MinimapOrthoSize[freeRideChapter - 1];
            miniCamera_LongView.transform.position = freeRide_MinimapPositions[freeRideChapter - 1].position;

            //Set the startPosition for Free Ride Mode
            playerStartPosition.position = freeRide_StartPoints[freeRideChapter - 1].position;
            playerStartPosition.localEulerAngles = freeRide_StartPoints[freeRideChapter - 1].localEulerAngles;
            GameMaster.instance.freeRide_Chapter = 0;//Reset
        }
        else
        {
            int chapterNumber = GameMaster.instance.selected_ChapterId;
            lessons_Object.SetActive(true);
            int vehicleIndex = 0;
            switch (chapterNumber)
            {
                case 0:
                    currentLevel = californiaLevelTargets;
                    break;
                case 1:
                    //Currently using california instead of newyork
                    currentLevel = californiaLevelTargets;
                    break;
                case 2:
                    //Currently using california instead of Germany
                    currentLevel = californiaLevelTargets;
                    break;
                case 3:
                    break;
                default:
                    break;
            }
            vehicleIndex = GetVehicleTypeChapter();
            //Current City
            lessonsCities[GameMaster.instance.selected_ChapterId].SetActive(true);
            LevelTarget currentLevelTarget = currentLevel[vehicleIndex].levelTargets[GameMaster.instance.selected_LevelNumber];
            playerStartPosition.position = currentLevelTarget.startPosition.position;
            playerStartPosition.localEulerAngles = currentLevelTarget.startPosition.localEulerAngles;
            parkingSlotPosition.position = currentLevelTarget.parkingPosition.position;
            parkingSlotPosition.localEulerAngles = currentLevelTarget.parkingPosition.localEulerAngles;

            //Enable Triggers for particular level to disable navigation,once we reach the point
            foreach (var item in currentLevelTarget.enableNavigationGPSTriggers)
            {
                item.SetActive(true);
            }
        }
    }
    public int GetVehicleTypeChapter()
    {
        int vehicleIndex = 0;
        switch (GameMaster.instance.selected_vehicle)
        {
            case "Car":
                vehicleIndex = 0;
                selectedIndex = PlayerPrefs.GetInt(StringConstants.carSelectedIndex);
                break;
            case "Bus":
                //Currently using car points instead of bus
                vehicleIndex = 0;
                selectedIndex = PlayerPrefs.GetInt(StringConstants.busSelectedIndex);
                parkingSlotPosition.localScale = new Vector3(1, 1, 2);
                break;
            case "Tow Truck":
                //Currently using car points instead of tow truck
                vehicleIndex = 0;
                selectedIndex = PlayerPrefs.GetInt(StringConstants.towSelectedIndex);
                break;
            default:
                break;
        }
        return vehicleIndex;
    }

    [Space(5)]
    [Header("Car Spawn")]

    #region CarSpawn
    [SerializeField] Transform spawnPosition;
    RCC_CarControllerV3 spawnedVehicle;
    [SerializeField] RCC_Camera RCCCamera; // Enabling / disabling camera selection script on RCC Camera if choosen.
    int selectedIndex;
    int SelectedIndex
    {
        get
        {
            return selectedIndex;//selectedIndex;
        }
        set
        {
            selectedIndex = value;
        }
    }
    void Start()
    {

        switch (GameMaster.instance.selected_vehicle)
        {
            case "Car":
                spawnedVehicle = RCC.SpawnRCC(RCC_DemoVehicles.Instance.vehiclesNormal[SelectedIndex].RCC_carController, spawnPosition.position, spawnPosition.rotation, false, false, false);
                break;
            case "Bus":
                spawnedVehicle = RCC.SpawnRCC(RCC_DemoVehicles.Instance.busVehicles[SelectedIndex].RCC_carController, spawnPosition.position, spawnPosition.rotation, false, false, false);
                break;
            case "Tow Truck":
                spawnedVehicle = RCC.SpawnRCC(RCC_DemoVehicles.Instance.towVehicles[SelectedIndex].RCC_carController, spawnPosition.position, spawnPosition.rotation, false, false, false);
                break;
            default:
                break;
        }
        if(GameMaster.instance.selected_vehicle == "")
        spawnedVehicle = RCC.SpawnRCC(RCC_DemoVehicles.Instance.vehiclesNormal[SelectedIndex].RCC_carController, spawnPosition.position, spawnPosition.rotation, false, false, false);
      
        spawnedVehicle.gameObject.SetActive(true);
        SelectVehicle();
    }
   
    public void SelectVehicle()
    {
        spawnedVehicle.SetCanControl(true);

        // If RCC Camera is choosen, it will disable RCC_CameraCarSelection script. This script was used for orbiting camera.
        if (RCCCamera)
        {
            if (RCCCamera.GetComponent<RCC_CameraCarSelection>())
                RCCCamera.GetComponent<RCC_CameraCarSelection>().enabled = false;
        }
        RCC.RegisterPlayerVehicle(spawnedVehicle);
    }
    #endregion
}

[System.Serializable]
public class LevelTarget
{
    public string LevelNumber;
    public Transform startPosition;
    public Transform parkingPosition;
    public List<GameObject> enableNavigationGPSTriggers = new List<GameObject>();
}

[System.Serializable]
public class VehicleLevelTargets
{
    public string vehicleType;
    public List<LevelTarget> levelTargets = new List<LevelTarget>();
}

