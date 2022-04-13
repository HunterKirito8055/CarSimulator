using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Waypoint : MonoBehaviour
{
    public Waypoint previousWaypoint;
    public Waypoint nextWaypoint;

    [Range(0f, 5f)] public float width = 1f;

    public Vector3 GetPosition()
    {
        Vector3 minBoundary = transform.position + transform.right * width / 2f;
        Vector3 maxBoundary = transform.position - transform.right * width / 2f;
        return Vector3.Lerp(minBoundary, maxBoundary, Random.Range(0f, 1f));
    }
}
