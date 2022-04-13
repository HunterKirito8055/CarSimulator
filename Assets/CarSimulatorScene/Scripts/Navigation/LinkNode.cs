using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LinkNode : MonoBehaviour
{
    public playerCurrentDirection waypoint;

    public LinkNode prevNode;
    [HideInInspector]public playerCurrentDirection currentDir;

    void Start()
    {
        waypoint.thisNode = this.transform;
        currentDir = waypoint;
    }

    // Update is called once per frame

    //Gizmos
    private void OnDrawGizmos()
    {
        waypoint.thisNode = this.transform;
    }
}

[System.Serializable]
public class playerCurrentDirection
{
    public Transform thisNode;
    public int gCost;
    public int hCost;
    public int FCost { get { return gCost + hCost; } }

    public Transform Left;
    public Transform Right;
    public Transform Top;
    public Transform Bottom;
}
