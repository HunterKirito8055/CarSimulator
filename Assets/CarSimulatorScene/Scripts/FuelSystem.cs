using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
//Bhargav Code
public class FuelSystem : MonoBehaviour
{
    bool isVehicleMoving;
    bool isFuelEmpty;
    bool indicatorOn;
    [SerializeField] float fuel = 100f;
    float milage = 1f;
    float reducer;

    public bool IsVehicleMoving
    {
        get
        {
            return isVehicleMoving;
        }
        set
        {
            isVehicleMoving = value;
        }
    }
   
    void Start()
    {
        isFuelEmpty = false;
        reducer = milage;   
    }


    void Update()
    {
        if (SceneManager.GetActiveScene().buildIndex != 1)
            return;
        if (!GameManager.instance.uIManager.fuelBar.gameObject.activeSelf || isFuelEmpty)
        {
            return;
        }
        if (isVehicleMoving)
        {
            if (reducer > 0)
            {
                reducer -= Time.deltaTime;
                
            }
            else
            {
                reducer = milage;
                fuel -= 1f;
                //if (indicatorOn)
                //{
                //    GameManager.instance.uIManager.fuelIndictor.color = new Color(255, 255, 255, 255);
                //    indicatorOn = false;
                //}
            }
        }
        GameManager.instance.uIManager.fuelBar.value = fuel/100;

        if (fuel < 90)
        {
            // Active Low fuel Indicator
            indicatorOn = true;
            //GameManager.instance.uIManager.fuelIndictor.color = new Color(255, 255, 255, 0);
            //fuelIndicator.color = RCC_SceneManager.Instance.activePlayerVehicle.fuelTank < 10f ? Color.red : new Color(.1f, 0f, 0f);
        }
        if (fuel <= 0)
        {
            isFuelEmpty = true;
            RCC_SceneManager.Instance.activePlayerVehicle.KillEngine();
            GameManager.instance.uIManager.gameStatus = false;
            GameManager.instance.uIManager.CompletePanel(true);
        }
    }
    public void VehicleMovingStatus(bool status)
    {
        //isVehicleMoving = status;

        //print("in here " + status + " " + isVehicleMoving);

    }
    private void OnTriggerStay(Collider other)
    {
        //for refill fuel 
        if (other.tag == StringConstants.fuelStation)
        {
            if (fuel < 100)
            {
                fuel += Time.deltaTime;
                if (isFuelEmpty) isFuelEmpty = false;
            }
        }
    }
}
