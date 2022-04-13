using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParkingIndicator : MonoBehaviour
{
    public Transform ParkingSlot;
    public Transform camPos;
    public Transform arrow;

    public Vector3 screenPoint;
    [SerializeField] NavigationDirection navigationDirection;


    public Vector3 viewPos;
    void Update()
    {

        //if Engine Doesnt Started Turn off arrow parking indicator
        if (GameManager.instance.uIManager.fuelBar.gameObject.activeSelf == false)
        {
            return;
        }
         viewPos = Camera.main.WorldToViewportPoint(ParkingSlot.position);
        if (viewPos.x >= -0.1f && viewPos.x <= 1.3f && viewPos.y >= 0 && viewPos.y <= 1 && viewPos.z > 0)
        {
            arrow.gameObject.SetActive(false);
        }
        else if(!navigationDirection.isGPS_ON)
        {
            arrow.gameObject.SetActive(true);
        }

        camPos = RCC_SceneManager.Instance.activePlayerCamera.transform;
        if (camPos != null && ParkingSlot != null)
        {
            screenPoint = camPos.InverseTransformPoint(ParkingSlot.position);
            float a = Mathf.Atan2(screenPoint.x, screenPoint.y) * Mathf.Rad2Deg;
            a += 180;
            arrow.transform.localEulerAngles = new Vector3(0, 180, a);
        }

    }
}
