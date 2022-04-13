using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DrivingSide : MonoBehaviour
{
    [SerializeField] GameObject aiCar_Parent;
    [SerializeField] GameObject outSide_aiCarParent;
    [SerializeField] Scrollbar drivingSide_Slider;

    GameObject[] vehicles;
    [SerializeField] Button changeRoadButton;

    [SerializeField] NavigationPathFinding navigationPathFinding;
    //Temp Variable
    [SerializeField] Transform tempVar_TerrainGround;


    void Start()
    {
        if (aiCar_Parent != null)
        {
            vehicles = new GameObject[aiCar_Parent.transform.childCount];
            for (int i = 0; i < vehicles.Length; i++)
            {
                vehicles[i] = aiCar_Parent.transform.GetChild(i).gameObject;
            }
        }
        IsLeftSideDriving = 0;
        StartCoroutine(SetPlayerVehicle());
    }
    IEnumerator SetPlayerVehicle()
    {
        yield return new WaitForSeconds(0.2f);
        RCC_SceneManager.Instance.activePlayerVehicle.transform.SetParent(this.transform);
    }


    int IsLeftSideDriving
    {
        get
        {
            return PlayerPrefs.GetInt(StringConstants.drivingSide, 0);
        }
        set
        {
            if (RCC_SceneManager.Instance.activePlayerVehicle != null)
            {
                RCC_CarControllerV3 player = RCC_SceneManager.Instance.activePlayerVehicle;

                player.transform.parent = this.transform;//Make this gameobject, parent to player car
                player.rigid.velocity = new Vector3(-player.rigid.velocity.x, player.rigid.velocity.y, player.rigid.velocity.z);//Player should move previous movement direction 

                //Keep them order to change the cars parent to work, if we rotate the environment  
                if (value == 0)
                {
                    ChangeCarsParent(aiCar_Parent);
                    player.transform.localScale = transform.localScale = new Vector3(1, 1, 1);
                    tempVar_TerrainGround.localPosition = new Vector3(-477.2553f, -338.3573f, -37.01208f);
                }
                else if (value == 1)
                {
                    player.transform.localScale = transform.localScale = new Vector3(-1, 1, 1);
                    player.speed = 0;

                    ChangeCarsParent(outSide_aiCarParent);
                    tempVar_TerrainGround.localPosition = new Vector3(13.1f, -338.3573f, -37.01208f);
                }

            }
            PlayerPrefs.SetInt(StringConstants.drivingSide, value);
        }

    }

    void ChangeCarsParent(GameObject Parent)
    {
        foreach (var item in vehicles)
        {
            item.transform.parent = Parent.transform;
            item.GetComponent<Rigidbody>().Sleep();
            item.transform.localScale = new Vector3(1, 1, 1);
        }
    }

    public void ChangeDrivingSide()
    {
        StartCoroutine(IEChangeDrivingSide());
    }

    IEnumerator IEChangeDrivingSide()
    {
        changeRoadButton.interactable = false; //Disable the button


        if (IsLeftSideDriving == 0)
        {
            IsLeftSideDriving = 1;
        }
        else
        {
            IsLeftSideDriving = 0;
        }
        yield return new WaitForSecondsRealtime(1f);
        drivingSide_Slider.value = PlayerPrefs.GetInt(StringConstants.drivingSide, 0);
        changeRoadButton.interactable = true; //Enable the button
        navigationPathFinding.DrawPath_OnRoadChange();
    }
}
