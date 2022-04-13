using System.Collections.Generic;
using System.Collections;
using UnityEngine;

namespace TrafficSimulation
{
    public enum IntersectionType
    {
        STOP,
        TRAFFIC_LIGHT
    }
    public enum TrafficLightSignal
    {
        GREEN,
        YELLOW,
        RED,
        NONE
    }

    public class Intersection : MonoBehaviour
    {
        #region variables
        public IntersectionType intersectionType;
        public TrafficLightSignal currentSignal1;
        public TrafficLightSignal currentSignal2;
        public int id;  //
        //For stop enum only
        public List<Segment> prioritySegments;

        //For traffic lights enum only
        public float lightsDuration = 8; //Red and Green Lights
        public float orangeLightDuration = 2;

        //segments 
        public List<Segment> lightsNbr1;
        public List<Segment> lightsNbr2;

        public List<GameObject> vehiclesQueue;
        public List<GameObject> vehiclesInIntersection;

        [HideInInspector] public int currentRedLightsGroup = 1;
        #endregion

        //Traffic Lights    //code by ashish
        public MeshRenderer[] firstRedTrafficLights_RedMat;  //First Red
        public MeshRenderer[] firstRedTrafficLights_YellowMat;  //First Red
        public MeshRenderer[] firstRedTrafficLights_GreenMat;  //First Red

        public MeshRenderer[] firstGreenTrafficLights_RedMat;  //First Green
        public MeshRenderer[] firstGreenTrafficLights_YellowMat;//First Green
        public MeshRenderer[] firstGreenTrafficLights_GreenMat; //First Green

        //Traffic Lights
        public bool isRightSideRed;
        public bool isLeftSideRed;
        public bool isFrontSideRed;
        public bool isBackSideRed;

        float vehicleMassMax = 2200f;
        float vehicleMassMin = 1700f;
        void Start()
        {
            vehiclesQueue = new List<GameObject>();
            vehiclesInIntersection = new List<GameObject>();
            if (intersectionType == IntersectionType.TRAFFIC_LIGHT)
                InvokeRepeating("SwitchLights", lightsDuration, lightsDuration);

            //Duplicate traffic light textures for every junction to separate all city traffic lights
            for (int i = 0; i < firstRedTrafficLights_RedMat.Length; i++)
            {
                Instantiate(firstRedTrafficLights_RedMat[i].material);
                Instantiate(firstRedTrafficLights_YellowMat[i].material);
                Instantiate(firstRedTrafficLights_GreenMat[i].material);
            }
            for (int i = 0; i < firstGreenTrafficLights_RedMat.Length; i++)
            {
                Instantiate(firstGreenTrafficLights_RedMat[i].material);
                Instantiate(firstGreenTrafficLights_YellowMat[i].material);
                Instantiate(firstGreenTrafficLights_GreenMat[i].material);
            }
        }

        void ChangeFirstRedTrafficLights(Color color)
        {
            //if the color is white or their respective colors then change them
            for (int i = 0; i < firstRedTrafficLights_RedMat.Length; i++)
            {
                if (color == Color.red || color == Color.white)
                    firstRedTrafficLights_RedMat[i].material.color = color;
                if (color == Color.yellow || color == Color.white)
                    firstRedTrafficLights_YellowMat[i].material.color = color;
                if (color == Color.green || color == Color.white)
                    firstRedTrafficLights_GreenMat[i].material.color = color;
            }
        }
        void ChangeFirstGreenTrafficLights(Color color)
        {
            //if the color is white or their respective colors then change them
            for (int i = 0; i < firstGreenTrafficLights_RedMat.Length; i++)
            {
                if (color == Color.red || color == Color.white)
                    firstGreenTrafficLights_RedMat[i].material.color = color;
                if (color == Color.yellow || color == Color.white)
                    firstGreenTrafficLights_YellowMat[i].material.color = color;
                if (color == Color.green || color == Color.white)
                    firstGreenTrafficLights_GreenMat[i].material.color = color;
            }
        }
        //Ashish Code
        void SwitchAlltheLights(string _lightName, Color changeColor)
        {
            //Traffic Signal Check for player
            if (_lightName == StringConstants.lights1)
            {
                ChangeFirstRedTrafficLights(Color.white);
                if (changeColor == Color.red)
                {
                    currentSignal1 = TrafficLightSignal.RED;
                    ChangeFirstRedTrafficLights(Color.red);
                }
                if (changeColor == Color.yellow)
                {
                    currentSignal1 = TrafficLightSignal.YELLOW;
                    ChangeFirstRedTrafficLights(Color.yellow);
                }
                if (changeColor == Color.green)
                {
                    currentSignal1 = TrafficLightSignal.GREEN;
                    ChangeFirstRedTrafficLights(Color.green);
                }
            }
            if (_lightName == StringConstants.lights2)
            {
                ChangeFirstGreenTrafficLights(Color.white);
                if (changeColor == Color.red)
                {
                    currentSignal2 = TrafficLightSignal.RED;
                    ChangeFirstGreenTrafficLights(Color.red);
                }
                if (changeColor == Color.yellow)
                {
                    currentSignal2 = TrafficLightSignal.YELLOW;
                    ChangeFirstGreenTrafficLights(Color.yellow);
                }
                if (changeColor == Color.green)
                {
                    currentSignal2 = TrafficLightSignal.GREEN;
                    ChangeFirstGreenTrafficLights(Color.green);
                }
            }
            //Traffic Signal Check for player
        }
        IEnumerator Lights(string lightName, Color start, Color End)
        {
            SwitchAlltheLights(lightName, start);
            yield return new WaitForSeconds(2f);
            SwitchAlltheLights(lightName, End);
        }
        //Ashish Code


        void SwitchLights()
        {
            if (currentRedLightsGroup == 1)
            {
                currentRedLightsGroup = 2;
                //Ashish Code
                StartCoroutine(Lights(StringConstants.lights1, Color.yellow, Color.green));
                StartCoroutine(Lights(StringConstants.lights2, Color.yellow, Color.red));
            }
            else if (currentRedLightsGroup == 2)
            {
                currentRedLightsGroup = 1;
                //Ashish Code
                StartCoroutine(Lights(StringConstants.lights1, Color.yellow, Color.red));
                StartCoroutine(Lights(StringConstants.lights2, Color.yellow, Color.green));
            }
            //Wait few seconds after light transition before making the other car move (= orange light)
            Invoke("MoveVehiclesQueue", orangeLightDuration);
        }
        void TurnOfftheSignalLights(GameObject _vehicle)
        {
            VehicleAI vehicleAI = _vehicle.GetComponent<VehicleAI>();
            if (vehicleAI != null)
            {
                vehicleAI.TurnOfftheSignalLights();
                vehicleAI.isIntersectionEntered = false;
            }
        }

        void MoveVehiclesQueue()
        {
            //Move all vehicles in queue
            List<GameObject> nVehiclesQueue = new List<GameObject>(vehiclesQueue);
            foreach (GameObject vehicle in vehiclesQueue)
            {
                int vehicleSegment = vehicle.GetComponent<VehicleAI>().GetSegmentVehicleIsIn();
                //If the signal is green,remove from queue and drive
                if (!IsRedLightSegment(vehicleSegment))
                {
                    vehicle.GetComponent<VehicleAI>().vehicleStatus = Status.GO;
                    vehicle.GetComponent<VehicleAI>().isWaitingForSignal = false;       //Ashish 27/10
                    vehicle.GetComponent<VehicleAI>().RIGID.mass = vehicleMassMin;   //Ashish 27/10
                    nVehiclesQueue.Remove(vehicle);
                }
            }
            vehiclesQueue = nVehiclesQueue;
        }

        #region OnTrigger Methods
        void OnTriggerEnter(Collider _other)
        {
            //Check if vehicle is already in the list if yes abort
            //Also abort if we just started the scene (if vehicles inside colliders at start)
            if (IsAlreadyInIntersection(_other.gameObject) || Time.timeSinceLevelLoad < .5f) return;

            if (_other.tag == "AutonomousVehicle" && intersectionType == IntersectionType.STOP)
                TriggerForStop(_other.gameObject);
            else if (_other.tag == "AutonomousVehicle" && intersectionType == IntersectionType.TRAFFIC_LIGHT)
                TriggerForTrafficLights(_other.gameObject);
        }

        void OnTriggerExit(Collider _other)
        {
            if (_other.tag == "AutonomousVehicle" && intersectionType == IntersectionType.STOP)
                ExitForStop(_other.gameObject);
            else if (_other.tag == "AutonomousVehicle" && intersectionType == IntersectionType.TRAFFIC_LIGHT)
                ExitForTrafficLight(_other.gameObject);

            TurnOfftheSignalLights(_other.gameObject);
        }

        void TriggerForStop(GameObject _vehicle)
        {
            VehicleAI vehicleAI = _vehicle.GetComponent<VehicleAI>();

            //Depending on the waypoint threshold, the car can be either on the target segment or on the past segment
            int vehicleSegment = vehicleAI.GetSegmentVehicleIsIn();
            vehicleAI.isIntersectionEntered = true;

            if (!IsPrioritySegment(vehicleSegment))
            {
                if (vehicleAI.aILightManager.presentIndicator != IndicatorType.Left) //AShish Code
                    if (vehiclesQueue.Count > 0 || vehiclesInIntersection.Count > 0)
                    {
                        vehicleAI.vehicleStatus = Status.STOP;
                        vehicleAI.isWaitingForSignal = true;                    //Ashish 27/10
                        vehicleAI.RIGID.mass = vehicleMassMax;                       //Ashish 27/10
                        vehiclesQueue.Add(_vehicle);
                    }
                    else
                    {
                        vehiclesInIntersection.Add(_vehicle);
                        vehicleAI.isWaitingForSignal = false;                    //Ashish 27/10
                        vehicleAI.RIGID.mass = vehicleMassMin;                       //Ashish 27/10
                        vehicleAI.vehicleStatus = Status.SLOW_DOWN;
                    }
            }
            else
            {
                vehicleAI.vehicleStatus = Status.SLOW_DOWN;
                vehicleAI.isWaitingForSignal = false;                    //Ashish 27/10
                vehicleAI.RIGID.mass = vehicleMassMin;                       //Ashish 27/10
                vehiclesInIntersection.Add(_vehicle);
            }
        }
        void ExitForStop(GameObject _vehicle)
        {
            _vehicle.GetComponent<VehicleAI>().vehicleStatus = Status.GO;
            _vehicle.GetComponent<VehicleAI>().isWaitingForSignal = false;  //Ashish 27/10
            _vehicle.GetComponent<VehicleAI>().RIGID.mass = vehicleMassMin;                       //Ashish 27/10

            vehiclesInIntersection.Remove(_vehicle);
            vehiclesQueue.Remove(_vehicle);

            if (vehiclesQueue.Count > 0 && vehiclesInIntersection.Count == 0)
            {
                vehiclesQueue[0].GetComponent<VehicleAI>().vehicleStatus = Status.GO;
                vehiclesQueue[0].GetComponent<VehicleAI>().isWaitingForSignal = false;  //Ashish 27/10
                vehiclesQueue[0].GetComponent<VehicleAI>().RIGID.mass = vehicleMassMin;           //Ashish 27/10
            }
        }
        void TriggerForTrafficLights(GameObject _vehicle)
        {
            VehicleAI vehicleAI = _vehicle.GetComponent<VehicleAI>();
            int vehicleSegment = vehicleAI.GetSegmentVehicleIsIn();
            vehicleAI.isIntersectionEntered = true;
            if (vehicleAI.aILightManager.presentIndicator != IndicatorType.Left)  //AShish Code
                if (IsRedLightSegment(vehicleSegment))
                {

                    vehicleAI.vehicleStatus = Status.STOP;
                    vehiclesQueue.Add(_vehicle);
                    vehicleAI.isWaitingForSignal = true;                    //Ashish 27/10
                    vehicleAI.RIGID.mass = vehicleMassMax;                       //Ashish 27/10
                }
                else
                {
                    vehicleAI.vehicleStatus = Status.GO;
                    vehicleAI.isWaitingForSignal = false;                    //Ashish 27/10
                    vehicleAI.RIGID.mass = vehicleMassMin;                       //Ashish 27/10
                }
        }
        void ExitForTrafficLight(GameObject _vehicle)
        {
            _vehicle.GetComponent<VehicleAI>().vehicleStatus = Status.GO;
            _vehicle.GetComponent<VehicleAI>().isWaitingForSignal = false;                    //Ashish 27/10
            _vehicle.GetComponent<VehicleAI>().RIGID.mass = vehicleMassMin;                       //Ashish 27/10

        }
        #endregion


        //Check if vehicle is already triggered
        bool IsAlreadyInIntersection(GameObject _target)
        {
            foreach (GameObject vehicle in vehiclesInIntersection)
            {
                if (vehicle.GetInstanceID() == _target.GetInstanceID()) return true;
            }
            foreach (GameObject vehicle in vehiclesQueue)
            {
                if (vehicle.GetInstanceID() == _target.GetInstanceID()) return true;
            }
            return false;
        }

        bool IsRedLightSegment(int _vehicleSegment)
        {
            if (currentRedLightsGroup == 1)
            {
                foreach (Segment segment in lightsNbr1)
                {
                    if (segment.id == _vehicleSegment)
                        return true;
                }
            }
            else
            {
                foreach (Segment segment in lightsNbr2)
                {
                    if (segment.id == _vehicleSegment)
                        return true;
                }
            }
            return false;
        }

        bool IsPrioritySegment(int _vehicleSegment)
        {
            foreach (Segment s in prioritySegments)
            {
                if (_vehicleSegment == s.id)
                    return true;
            }
            return false;
        }

        #region DeactivateScript
        private List<GameObject> memVehiclesQueue = new List<GameObject>();
        private List<GameObject> memVehiclesInIntersection = new List<GameObject>();

        public void SaveIntersectionStatus()
        {
            memVehiclesQueue = vehiclesQueue;
            memVehiclesInIntersection = vehiclesInIntersection;
        }

        public void ResumeIntersectionStatus()
        {
            foreach (GameObject v in vehiclesInIntersection)
            {
                foreach (GameObject v2 in memVehiclesInIntersection)
                {
                    if (v.GetInstanceID() == v2.GetInstanceID())
                    {
                        v.GetComponent<VehicleAI>().vehicleStatus = v2.GetComponent<VehicleAI>().vehicleStatus;
                        break;
                    }
                }
            }
            foreach (GameObject v in vehiclesQueue)
            {
                foreach (GameObject v2 in memVehiclesQueue)
                {
                    if (v.GetInstanceID() == v2.GetInstanceID())
                    {
                        v.GetComponent<VehicleAI>().vehicleStatus = v2.GetComponent<VehicleAI>().vehicleStatus;
                        break;
                    }
                }
            }
        }
        #endregion
    }
}
