using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace TrafficSimulation
{

    /*
        [-] Check prefab #6 issue
        [-] Deaccelerate when see stop in front
        [-] Smooth sharp turns when two segments are linked
    */
    public struct Target
    {
        public int segment;
        public int waypoint;
    }
    public enum Status
    {
        GO,
        STOP,
        SLOW_DOWN
    }

    public class VehicleAI : MonoBehaviour
    {
        public AILightManager aILightManager;
        public Rigidbody RIGID;
        [Header("Traffic System")]
        [Tooltip("Current active traffic system")]
        public TrafficSystem trafficSystem;

        [Tooltip("Determine when the vehicle has reached its target. Can be used to \"anticipate\" earlier the next waypoint (the higher this number his, the earlier it will anticipate the next waypoint)")]
        public float waypointThresh = 6;

        [Header("Radar")]

        [Tooltip("Empty gameobject from where the rays will be casted")]
        public Transform raycastAnchor;

        [Tooltip("Length of the casted rays")]
        public float raycastLength = 5;

        [Tooltip("Spacing between each rays")]
        public int raySpacing = 8;

        [Tooltip("Number of rays to be casted")]
        public int raysNumber = 18;

        [Tooltip("If detected vehicle is below this distance, ego vehicle will stop")]
        public float emergencyBrakeThresh = 3f;

        [Tooltip("If detected vehicle is below this distance (and above, above distance), ego vehicle will slow down")]
        public float slowDownThresh = 5f;

        [HideInInspector] public Status vehicleStatus = Status.GO;

        public WheelDrive wheelDrive;
        private float initMaxSpeed = 0;
        private int pastTargetSegment = -1;
        public Target currentTarget;
        private Target futureTarget;

        //AShish Code
        int waypointReceiver = 0;
        float playerEmergencyThreshold = 4.5f;
        float playerSlowDownThreshold = 6f;

        public float playerRayLength = 15f;
        public int playerRaysNumbers = 18;
        public int playerRaySpacing = 3;
        bool isHorn = false;
        public AudioSource hornSound;
        //AShish Code

        //Bhargav
        float turningBlinkerAngle;
        bool turning = false;
        bool lightOn = false;
        public bool isIntersectionEntered;
         float steering = 0;

        //Bhargav

        private void Awake()
        {
            RIGID = GetComponent<Rigidbody>();
            aILightManager = GetComponent<AILightManager>();
        }
        void Start()
        {
            wheelDrive = this.GetComponent<WheelDrive>();
            if (trafficSystem == null)
                return;

            initMaxSpeed = wheelDrive.maxSpeed;
            SetWaypointVehicleIsOn();
        }

        void Update()
        {
            if (trafficSystem == null)
                return;

            WaypointChecker();
            if (isPlayerDashed == false)
            {
                MoveVehicle();
            }
            //If car has been hit by player
            if (isReachToPoint_AfterDash)
            {
                //  Vehicle_GoToSegmentLinePoint();
                steering_DashValue = Mathf.Clamp(this.transform.InverseTransformDirection((pointInSegmentsLine - this.transform.position).normalized).x, -1f, 1f);
                pointDistance = Vector3.Distance(this.transform.position, pointInSegmentsLine);

                if (pointDistance > 1.8f)
                {
                    RIGID.freezeRotation = false;
                    this.transform.localEulerAngles = new Vector3(0, this.transform.localEulerAngles.y, 0);

                    vehicleStatus = Status.GO;
                    transform.localRotation = Quaternion.Slerp(this.transform.localRotation, Quaternion.LookRotation((pointInSegmentsLine - this.transform.position)), Time.deltaTime * 0.25f);
                    wheelDrive.Move(1f, steering_DashValue, 0);
                    wheelDrive.maxSpeed = 300;
                }
                else
                {
                    isReachToPoint_AfterDash = false;
                    isPlayerDashed = false;
                    vehicleStatus = Status.GO;
                }
            }
        }

        #region Car Dash
        //Car to have link with segments after hit by player
        Vector3 previousSegmentPos;
        Vector3 pointInSegmentsLine;
        [SerializeField] float dotVal;
        float pointDistance;
        float steering_DashValue;

        Vector3 prevToNext_LineDirection;
        Vector3 prevToCar_LineDirection;

        bool isPlayerDashed = false;
        bool isReachToPoint_AfterDash = false;
        [HideInInspector] public bool isWaitingForSignal = false;

        private void OnCollisionEnter(Collision collision)
        {
            if (collision.transform.tag == StringConstants.player)
            {
                if (isWaitingForSignal == false)
                {
                    vehicleStatus = Status.STOP;
                    wheelDrive.Move(0, 0, 1);
                    isPlayerDashed = true;
                    StartCoroutine(TakeNextDirection());
                }
            }
        }
        IEnumerator TakeNextDirection()
        {
            yield return new WaitForSeconds(5f);
            prevToNext_LineDirection = (trafficSystem.segments[currentTarget.segment].waypoints[currentTarget.waypoint].transform.position - previousSegmentPos).normalized;
            prevToCar_LineDirection = transform.position - previousSegmentPos;
            dotVal = Vector3.Dot(prevToCar_LineDirection, prevToNext_LineDirection);
            pointInSegmentsLine = previousSegmentPos + prevToNext_LineDirection * (Mathf.Abs(dotVal) * 1.5f); //Add 1.5f value to take the nearest point for future waypoint
            isReachToPoint_AfterDash = true;
        }
        //private void OnDrawGizmos()
        //{
        //    Gizmos.color = Color.red;
        //    Gizmos.DrawLine(this.transform.position, pointInSegmentsLine);
        //}
        #endregion
        void WaypointChecker()
        {
            GameObject waypoint = trafficSystem.segments[currentTarget.segment].waypoints[currentTarget.waypoint /*- trafficSystem.segments[currentTarget.segment].segmentlastWaypoint[0]*/].gameObject;

            //Position of next waypoint relative to the car
            Vector3 wpDist = this.transform.InverseTransformPoint(new Vector3(waypoint.transform.position.x, this.transform.position.y, waypoint.transform.position.z));

            //Go to next waypoint if arrived to current
            if (wpDist.magnitude < waypointThresh)
            {

                previousSegmentPos = trafficSystem.segments[currentTarget.segment].waypoints[currentTarget.waypoint].transform.position;
                //Get next target
                currentTarget.waypoint++;
                //if current waypoint ID value exceeds the count,then the segment needs to change

                if (currentTarget.waypoint >= trafficSystem.segments[currentTarget.segment].waypoints.Count)
                {

                    pastTargetSegment = currentTarget.segment;
                    currentTarget.segment = futureTarget.segment;

                    if (trafficSystem.segments[pastTargetSegment].waypointNextSegment.Length > 0) //Code by Ashish
                        currentTarget.waypoint = trafficSystem.segments[pastTargetSegment].waypointNextSegment[waypointReceiver];//0
                    else
                        currentTarget.waypoint = 0;//0

                }
                //if future waypoint ID value exceeds the count,then the segment needs to change
                //Get future target
                futureTarget.waypoint = currentTarget.waypoint + 1;
                if (futureTarget.waypoint >= trafficSystem.segments[currentTarget.segment].waypoints.Count)
                {
                    futureTarget.waypoint = 0;
                    futureTarget.segment = GetNextSegmentId();
                }
            }
        }
        

        void MoveVehicle()
        {
            //Default, full acceleration, no break and no steering
            float acc = 1;
            float brake = 0;
            wheelDrive.maxSpeed = initMaxSpeed;

            //Calculate if there is a planned turn
            Transform targetTransform = trafficSystem.segments[currentTarget.segment].waypoints[currentTarget.waypoint].transform;
            Transform futureTargetTransform = trafficSystem.segments[futureTarget.segment].waypoints[futureTarget.waypoint].transform;
            Vector3 futureVel = futureTargetTransform.position - targetTransform.position;
            float futureSteering = Mathf.Clamp(this.transform.InverseTransformDirection(futureVel.normalized).x, -1, 1);

            //Check if the car has to stop
            if (vehicleStatus == Status.STOP)
            {
                acc = 0;
                brake = 1;
                if (!isPlayerDashed)
                    wheelDrive.maxSpeed = Mathf.Min(wheelDrive.maxSpeed / 2f, 5f);
                RIGID.freezeRotation = true;
            }
            else
            {

                RIGID.freezeRotation = false;
                RIGID.constraints = RigidbodyConstraints.FreezeRotationZ;

                //Not full acceleration if have to slow down
                if (vehicleStatus == Status.SLOW_DOWN)
                {
                    acc = .3f;
                    brake = 0f;
                }

                //If planned to steer, decrease the speed
                if (futureSteering > .3f || futureSteering < -.3f)
                {
                    wheelDrive.maxSpeed = Mathf.Min(wheelDrive.maxSpeed, wheelDrive.steeringSpeedMax);
                }
                if (steering > .2f || steering < -.2f)
                {
                    raySpacing = 8;
                }
                else
                {
                    raySpacing = 4;
                }

                //2. Check if there are obstacles which are detected by the radar
                float hitDist;
                float playerhitDist;
                float intersectionhitDist;
                GameObject obstacle = GetDetectedObstacles(out hitDist, StringConstants.aiCarLayer);
                GameObject playerobstacle = GetDetectedObstacles(out playerhitDist, StringConstants.playerLayer);
                GameObject intersectionObstacle = GetDetectedObstacles(out intersectionhitDist, "Intersection");

                //Check if we hit something
                if (obstacle != null)
                {
                    WheelDrive otherVehicle = null;
                    otherVehicle = obstacle.GetComponent<WheelDrive>();

                    ///////////////////////////////////////////////////////////////
                    //Differenciate between other vehicles AI and generic obstacles (including controlled vehicle, if any)
                    if (otherVehicle != null)
                    {
                        //Check if it's front vehicle
                        float dotFront = Vector3.Dot(this.transform.forward, otherVehicle.transform.forward);

                        //If detected front vehicle max speed is lower than ego vehicle, then decrease ego vehicle max speed
                        if (otherVehicle.maxSpeed < wheelDrive.maxSpeed && dotFront > .8f)
                        {
                            float ms = Mathf.Max(wheelDrive.GetSpeedMS(otherVehicle.maxSpeed) - .5f, .1f);
                            wheelDrive.maxSpeed = wheelDrive.GetSpeedUnit(ms);
                        }

                        //If the two vehicles are too close, and facing the same direction, brake the ego vehicle
                        if (hitDist < emergencyBrakeThresh && dotFront > .8f)
                        {
                            acc = 0;
                            brake = 1;
                            RIGID.velocity = Vector3.zero;
                            wheelDrive.maxSpeed = Mathf.Max(wheelDrive.maxSpeed / 2f, wheelDrive.minSpeed);
                        }
                        //If the two vehicles are too close, and not facing same direction, slight make the ego vehicle go backward
                        else if (hitDist < emergencyBrakeThresh && dotFront <= .8f)
                        {
                            acc = -0.1f;
                            brake = 0f;
                            RIGID.velocity = Vector3.zero;
                            wheelDrive.maxSpeed = 0;// Mathf.Max(wheelDrive.maxSpeed / 2f, wheelDrive.minSpeed); //Code Commented By ashish

                            //Check if the vehicle we are close to is located on the right or left then apply according steering to try to make it move
                            float dotRight = Vector3.Dot(this.transform.forward, otherVehicle.transform.right);
                            //Right
                            if (dotRight > 0.1f) steering = -.3f;
                            //Left
                            else if (dotRight < -0.1f) steering = .3f;
                            //Middle
                            else steering = -.7f;
                        }
                        //If the two vehicles are getting close, slow down their speed
                        else if (hitDist < slowDownThresh)
                        {
                            acc = .2f;
                            brake = 0f;
                            //wheelDrive.maxSpeed = Mathf.Max(wheelDrive.maxSpeed / 1.5f, wheelDrive.minSpeed);
                        }
                    }
                    // Generic obstacles
                    else
                    {
                        //Emergency brake if getting too close
                        if (hitDist < emergencyBrakeThresh)
                        {
                            acc = 0;
                            brake = 1;
                            wheelDrive.maxSpeed = Mathf.Max(wheelDrive.maxSpeed / 2f, wheelDrive.minSpeed);
                        }
                        //Otherwise if getting relatively close decrease speed
                        else if (hitDist < slowDownThresh)
                        {
                            acc = .5f;
                            brake = 0f;
                        }
                    }
                }
                //Check if we need to steer to follow path
                if (acc > 0f)
                {
                    Vector3 desiredVel = trafficSystem.segments[currentTarget.segment].waypoints[currentTarget.waypoint].transform.position - this.transform.position;
                    steering = Mathf.Clamp(this.transform.InverseTransformDirection(desiredVel.normalized).x, -1f, 1f);
                }
                //Check if we hit Player //Ashish Code
                if (playerobstacle != null)
                {
                    //If we are too close to player, brake the vehicle
                    if (playerhitDist < playerEmergencyThreshold)
                    {
                        acc = 0;
                        brake = 1;
                        RIGID.velocity = Vector3.zero;
                        wheelDrive.maxSpeed = Mathf.Max(wheelDrive.maxSpeed / 2f, wheelDrive.minSpeed);
                        if (!isHorn)
                        {
                            isHorn = true;
                            StartCoroutine(Horn(acc));
                        }
                    }
                    else if (playerhitDist < 9f)
                    {
                        isHorn = false;
                        acc = 0.2f;
                        brake = 0.5f;
                        //wheelDrive.maxSpeed = Mathf.Max(wheelDrive.maxSpeed / 1.5f, wheelDrive.minSpeed);
                    }
                    //If this and player, both vehicles are getting close, slow down this vehicle speed
                    else if (playerhitDist < playerSlowDownThreshold)
                    {
                        isHorn = false; //Dont horn when vehicle is in slow speed 
                        acc = 0.5f;
                        brake = 0f;
                        //wheelDrive.maxSpeed = Mathf.Max(wheelDrive.maxSpeed / 1.5f, wheelDrive.minSpeed);
                    }
                }
                else if (playerobstacle == null && hornSound.isPlaying)
                {
                    isHorn = false;
                }

                if (intersectionObstacle != null)
                {
                    SignalLightBlinkers();
                    if (intersectionhitDist < slowDownThresh && vehicleStatus == Status.GO &&
                        isIntersectionEntered == false)
                    {
                        acc = 0.2f;
                        brake = 0f;
                    }
                }
            }

            //Move the car
            wheelDrive.Move(acc, steering, brake);
        }
        IEnumerator Horn(float _accl)
        {
            yield return new WaitForSeconds(3f);
            if (!hornSound.isPlaying && _accl == 0 && isHorn == true)
            {
                hornSound.Play();
                isHorn = false;
            }
            else if (isHorn == false)
            {
                hornSound.Stop();
            }
        }
        GameObject GetDetectedObstacles(out float _hitDist, string layerName)
        {
            GameObject detectedObstacle = null;
            float minDist = 1000f;

            int raysNumb = raysNumber;
            int raysSpace = raySpacing;
            float rayLength = raycastLength;
            //Ashish Code 01/09
            if (layerName == StringConstants.playerLayer)
            {
                raysNumb = playerRaysNumbers;
                raysSpace = playerRaySpacing;
                rayLength = playerRayLength;
            }
            else if (layerName == StringConstants.aiCarLayer)
            {
                raysNumb = raysNumber;
                raysSpace = raySpacing;
                rayLength = raycastLength;
            }
            else if (layerName == StringConstants.intersection)
            {
                raysNumb = 1;
                raysSpace = 1;
                //if (UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex == 1)
                //{
                //    rayLength = 30;
                //}
                //else if (UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex == 2)
                //{
                rayLength = 8;
                // }
            }

            float initRay = (raysNumb / 2f) * raysSpace;
            float hitDist = -1f;

            if (steering > 0.2)
                for (float a = -2; a <= initRay + 3; a += raysSpace)
                {
                    CastRay(raycastAnchor.transform.position, a, this.transform.forward, rayLength, out detectedObstacle, out hitDist, layerName);
                    if (detectedObstacle == null) continue;
                    float dist = Vector3.Distance(this.transform.position, detectedObstacle.transform.position);
                    if (dist < minDist)
                    {
                        minDist = dist;
                        break;
                    }
                }
            else if (steering < -0.2)
                for (float a = -initRay - 2; a <= 0 + 3; a += raysSpace)
                {
                    CastRay(raycastAnchor.transform.position, a, this.transform.forward, rayLength, out detectedObstacle, out hitDist, layerName);
                    if (detectedObstacle == null) continue;
                    float dist = Vector3.Distance(this.transform.position, detectedObstacle.transform.position);
                    if (dist < minDist)
                    {
                        minDist = dist;
                        break;
                    }
                }
            else
                for (float a = -initRay; a <= initRay; a += raysSpace)
                {
                    CastRay(raycastAnchor.transform.position, a, this.transform.forward, rayLength, out detectedObstacle, out hitDist, layerName);
                    if (detectedObstacle == null) continue;
                    float dist = Vector3.Distance(this.transform.position, detectedObstacle.transform.position);
                    if (dist < minDist)
                    {
                        minDist = dist;
                        break;
                    }
                }
            _hitDist = hitDist;
            return detectedObstacle;
        }
        void CastRaysToDetect(GameObject detectedObstacle,int initRay,int raysSpace)
        {
        
        }
        void CastRay(Vector3 _anchor, float _angle, Vector3 _dir, float _length, out GameObject _outObstacle, out float _outHitDistance, string _vehiclelayerName)
        {
            _outObstacle = null;
            _outHitDistance = -1f;

            //Draw raycast
            Debug.DrawRay(_anchor, Quaternion.Euler(0, _angle, 0) * _dir * _length, new Color(1, 0, 0, 0.5f));

            //Detect hit only on the autonomous vehicle layer
            int layer = 1 << LayerMask.NameToLayer(_vehiclelayerName);
            int finalMask = layer;

            foreach (string layerName in trafficSystem.collisionLayers)
            {
                int id = 1 << LayerMask.NameToLayer(layerName);
                finalMask = finalMask | id;
            }

            RaycastHit hit;
            if (Physics.Raycast(_anchor, Quaternion.Euler(0, _angle, 0) * _dir, out hit, _length, finalMask))
            {
                _outObstacle = hit.collider.gameObject;
                _outHitDistance = hit.distance;
            }
        }

        int GetNextSegmentId()
        {
            if (trafficSystem.segments[currentTarget.segment].nextSegments.Count == 0)
            {
                return 0;
            }

            int c = Random.Range(0, trafficSystem.segments[currentTarget.segment].nextSegments.Count);
            //Code by Ashish
            waypointReceiver = c;  //Receiving waypoint for next Segment 
                                   //now vehicles traverses to any direction from current lane of road
            return trafficSystem.segments[currentTarget.segment].nextSegments[c].id;
        }
        void SetWaypointVehicleIsOn()
        {
            //Find current target
            foreach (Segment segment in trafficSystem.segments)
            {
                if (segment.IsOnSegment(this.transform.position))
                {
                    currentTarget.segment = segment.id;

                    //Find nearest waypoint to start within the segment
                    float minDist = float.MaxValue;
                    for (int j = 0; j < trafficSystem.segments[currentTarget.segment].waypoints.Count; j++)
                    {
                        float d = Vector3.Distance(this.transform.position, trafficSystem.segments[currentTarget.segment].waypoints[j].transform.position);

                        //Only take in front points
                        Vector3 lSpace = this.transform.InverseTransformPoint(trafficSystem.segments[currentTarget.segment].waypoints[j].transform.position);
                        if (d < minDist && lSpace.z > 0)
                        {
                            minDist = d;
                            currentTarget.waypoint = j;
                        }
                    }
                    break;
                }
            }

            //Get future target
            futureTarget.waypoint = currentTarget.waypoint + 1; //c=3 ,f =4, 
            futureTarget.segment = currentTarget.segment;  //cs=0=fs

            //s_wp = 3
            //f =4 >= s_wp=3
            if (futureTarget.waypoint >= trafficSystem.segments[currentTarget.segment].waypoints.Count)
            {
                futureTarget.waypoint = 0;//0
                futureTarget.segment = GetNextSegmentId();
            }
        }

        public int GetSegmentVehicleIsIn()
        {
            int vehicleSegment = currentTarget.segment;
            bool isOnSegment = trafficSystem.segments[vehicleSegment].IsOnSegment(this.transform.position);
            if (!isOnSegment)
            {
                bool isOnPSegement = trafficSystem.segments[pastTargetSegment].IsOnSegment(this.transform.position);
                if (isOnPSegement)
                    vehicleSegment = pastTargetSegment;
            }
            return vehicleSegment;
        }

        void SignalLightBlinkers()
        {
            if (turning) return;
            turning = true;

            Transform t = trafficSystem.segments[futureTarget.segment].waypoints[futureTarget.waypoint].transform;
            Vector3 car = transform.right;
            Vector3 wayPoint = t.position - this.transform.position;
            Debug.DrawRay(transform.position, wayPoint, Color.green);
            turningBlinkerAngle = Vector3.Dot(car.normalized, wayPoint.normalized);

            if (Vector3.Dot(car, wayPoint) < -0.1)
            {
                aILightManager.Indicators(IndicatorType.Left);
            }
            else if (Vector3.Dot(car, wayPoint) > 0.1)
            {
                aILightManager.Indicators((IndicatorType.Right));
            }
            //turn on the lights
            lightOn = true;
        }
        public void TurnOfftheSignalLights()
        {
            //turn off the lights
            aILightManager.Indicators((IndicatorType.Off));
            lightOn = false;
            turning = false;
        }
    }
}