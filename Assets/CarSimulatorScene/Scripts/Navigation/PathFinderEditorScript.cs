using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathFinderEditorScript : MonoBehaviour
{
    public bool HideGizmosFront;

    public Transform WaypointsParentTransform;
    public LinkNode[] waypoints;

    private void Awake()
    {
        waypoints = WaypointsParentTransform.GetComponentsInChildren<LinkNode>();
    }
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
    private void OnDrawGizmos()
    {
        waypoints = WaypointsParentTransform.GetComponentsInChildren<LinkNode>();
        if (HideGizmosFront == false)
        {
            foreach (var item in waypoints)
            {
                CurrentDirection(item.waypoint, Color.blue);
            }
        }
    }

    void CurrentDirection(playerCurrentDirection currentDir, Color lineColor)
    {
        waypoints = WaypointsParentTransform.GetComponentsInChildren<LinkNode>();

        //Left
        if (currentDir.Left != null)
        {
            Gizmos.color = lineColor;
            Gizmos.DrawLine(currentDir.thisNode.position, currentDir.Left.position);
            DrawArrow((currentDir.thisNode.position + currentDir.Left.position) / 2, currentDir.thisNode.position - currentDir.Left.position, 1f);
        }
        //Right
        if (currentDir.Right != null)
        {
            Gizmos.color = lineColor;
            Gizmos.DrawLine(currentDir.thisNode.position, currentDir.Right.position);
            DrawArrow((currentDir.thisNode.position + currentDir.Right.position) / 2, currentDir.thisNode.position - currentDir.Right.position, 1f);
        }
        //Top
        if (currentDir.Top != null)
        {

            Gizmos.color = lineColor;
            Gizmos.DrawLine(currentDir.thisNode.position, currentDir.Top.position);
            DrawArrow((currentDir.thisNode.position + currentDir.Top.position) / 2, currentDir.thisNode.position - currentDir.Top.position, 1f);

        }
        //Bottom
        if (currentDir.Bottom != null)
        {
            Gizmos.color = lineColor;
            Gizmos.DrawLine(currentDir.thisNode.position, currentDir.Bottom.position);
            DrawArrow((currentDir.thisNode.position + currentDir.Bottom.position) / 2, currentDir.thisNode.position - currentDir.Bottom.position, 1f);
        }

    }

    void DrawArrow(Vector3 point, Vector3 forward, float size)
    {
        Gizmos.color = Color.red;
        forward = forward.normalized * size;
        Vector3 left = Quaternion.Euler(0, 45, 0) * forward;
        Vector3 right = Quaternion.Euler(0, -45, 0) * forward;
        Gizmos.DrawLine(point, point + left);
        Gizmos.DrawLine(point, point + right);
    }
}
