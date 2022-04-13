using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TrafficSimulation;
using System.Linq;


public class MinimapController : MonoBehaviour
{
    [SerializeField] GameObject parking_MinimapObj;
    [SerializeField] Transform parkingSlotTransform;

    [SerializeField] Transform playerPointer;
    [SerializeField] Transform childNavigator;


    void Start()
    {
        parking_MinimapObj.transform.position = new Vector3(parkingSlotTransform.position.x, 1025f, parkingSlotTransform.position.z);
        //Invoke("Makechild", 2f);
    }
    public void MakeChildtoVehicle()
    {
        childNavigator.parent = RCC_SceneManager.Instance.activePlayerVehicle.transform;
    }

    void Update()
    {
        //Update player Mark in Map
        if (RCC_SceneManager.Instance.activePlayerVehicle != null)
        {
            Transform playerTransform = RCC_SceneManager.Instance.activePlayerVehicle.transform;
            playerPointer.position = new Vector3(playerTransform.position.x, 0, playerTransform.position.z);
            playerPointer.eulerAngles = new Vector3(0, playerTransform.eulerAngles.y, 0);
        }
    }

}





