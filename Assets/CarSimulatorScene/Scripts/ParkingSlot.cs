using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// this script is attached to each wheel collider
/// </summary>
public class ParkingSlot : MonoBehaviour
{
    public bool isOnParking;
  
    bool fl, fr, rl, rr;
  
    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "FL")
        {
            fl = true;
        }
        if (other.tag == "FR")
        {
            fr = true;
        }
        if (other.tag == "RL")
        {
            rl = true;
        }
        if (other.tag == "RR")
        {
            rr = true;
        }

        if (fl && fr && rl && rr)
        {
            //print("parkinglsot parked");
            isOnParking = true;
            GameManager.instance.uIManager.InsideParkingSlot(isOnParking);
        }
    }

    
    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "FL")
        {
            fl = false;
            AnyOneWheelsOut();
        }
        if (other.tag == "FR")
        {
            fr = false;
            AnyOneWheelsOut();
        }
        if (other.tag == "RL")
        {
            rl = false;
            AnyOneWheelsOut();
        }
        if (other.tag == "RR")
        {
            rr = false;
            AnyOneWheelsOut();
        }
    }

    void AnyOneWheelsOut()
    {
        isOnParking = false;
        GameManager.instance.uIManager.InsideParkingSlot(isOnParking);
    }


}



