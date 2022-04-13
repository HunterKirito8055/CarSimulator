using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NavigationDirection : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == StringConstants.laneOne || other.tag == StringConstants.laneTwo)
        {
            navigationDir = other.GetComponent<WorldSpaceWaypointDirection>().navigationDir;
        }
        if(other.tag == StringConstants.intersection || other.tag == StringConstants.navigationTrigger)
        {
            isnavigationDetection = false;
        }
        if (other.tag == StringConstants.navigationGPS_Off)
        {
            isGPS_ON = false;
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.tag == StringConstants.intersection || other.tag == StringConstants.navigationTrigger)
        {
            isnavigationDetection = true;
        }
    }
    [HideInInspector] public int navigationDir = 0;
    [HideInInspector] public bool isnavigationDetection = true;
    [HideInInspector] public bool isGPS_ON = true;
}
