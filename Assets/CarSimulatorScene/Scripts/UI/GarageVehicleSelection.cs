using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class GarageVehicleSelection : MonoBehaviour
{
    [SerializeField] GameObject carsPosition;
    [SerializeField] GameObject busesPosition;
    [SerializeField] GameObject towTrucksPosition;
    [SerializeField] ChapterController chapterController;
    Transform vehicleTransform;


    public void CarSelection()
    {
        switch (chapterController.SelectIndex)
        {
            case 0:
                vehicleTransform = carsPosition.transform;
                break;
            case 1:
                vehicleTransform = busesPosition.transform;
                break;
            case 2:
                vehicleTransform = towTrucksPosition.transform;
                break;
            default:
                break;
        }
        RotateCamera(vehicleTransform, 1);
    }

    void RotateCamera(Transform t, float time)
    {
        transform.DOMove(t.position, time).SetEase(Ease.InOutFlash);
        transform.DORotateQuaternion(t.rotation, time);
    }
}
