using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WayPointNavigator : MonoBehaviour
{
    AIpedestrian walker;
    [SerializeField] Waypoint currentWaypoint;
    [Range(0f, 1f)] [SerializeField] int direction;

    private void Awake()
    {
        walker = GetComponent<AIpedestrian>();
    }
    void Start()
    {
        walker.SetDestination(currentWaypoint.GetPosition());

    }

    void Update()
    {
        if (walker.reachedDestination)
        {
            if (direction == 0)
            {
                currentWaypoint = currentWaypoint.nextWaypoint;
            }
            else if (direction == 1)
            {
                currentWaypoint = currentWaypoint.previousWaypoint;
            }
            walker.SetDestination(currentWaypoint.GetPosition());
        }
    }
}
