using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NavigationPathFinding : MonoBehaviour
{

    [SerializeField] Transform StartPosition;
    [SerializeField] Transform TargetPosition;

    [SerializeField] Transform WaypointsParentTransform;

    //All WayPoints
    LinkNode[] waypoints;

    List<LinkNode> DestinationPath = new List<LinkNode>();
    List<LinkNode> previousDestinationPath = new List<LinkNode>();

    [SerializeField] GameObject linerenderObj;
    [SerializeField] NavigationDirection navigationDirection;
    LinkNode startNode;
    LinkNode currentLinkNode;

    bool isMissedGPS;


    private void Awake()
    {
        waypoints = WaypointsParentTransform.GetComponentsInChildren<LinkNode>();
    }

    public void DrawPath_OnRoadChange()
    {
        LineRenderer lineRenderer = linerenderObj.GetComponent<LineRenderer>();
        for (int i = 0; i < lineRenderer.positionCount; i++)
        {
            Vector3 currentLine = lineRenderer.GetPosition(i);
            lineRenderer.SetPosition(i, new Vector3(-currentLine.x, currentLine.y, currentLine.z));
        }
    }

    public void DrawLine(float width, Color colour, bool _clearLineRenderer)
    {
        LineRenderer lineRenderer = linerenderObj.GetComponent<LineRenderer>();
        lineRenderer.material = new Material(Shader.Find("Unlit/Color"));
        lineRenderer.material.color = colour;
        if (_clearLineRenderer)
            lineRenderer.positionCount = 0; //clear line renderer to disable GPS
        if (DestinationPath.Count > 1)
        {
            lineRenderer.positionCount = DestinationPath.Count + 1;

            //StartPoint
            if (navigationDirection.navigationDir == 0)
            {
                lineRenderer.SetPosition(0, new Vector3(RCC_SceneManager.Instance.activePlayerVehicle.transform.position.x, 1010f, startNode.transform.position.z));
            }
            else
            {
                lineRenderer.SetPosition(0, new Vector3(startNode.transform.position.x, 1010f, RCC_SceneManager.Instance.activePlayerVehicle.transform.position.z));
            }
            //Loop from startNode + 1
            for (int i = 0; i < DestinationPath.Count - 1; i++)
            {
                lineRenderer.SetPosition(i + 1, new Vector3(DestinationPath[i].transform.position.x, 1010f, DestinationPath[i].transform.position.z));
            }

            //TargetNode
            lineRenderer.SetPosition(DestinationPath.Count, new Vector3(DestinationPath[DestinationPath.Count - 1].transform.position.x, 1010f, TargetPosition.position.z));
            linerenderObj.layer = LayerMask.NameToLayer(StringConstants.miniMap);
            lineRenderer.startWidth = width;
            lineRenderer.endWidth = width;
        }
    }
    //Gets the closest node to the given world position.
    public LinkNode NodePositionFromWorldPoint(Vector3 pos, bool dir)
    {
        //LinkNode currentLinkNode = waypoints[0];
        float minVal = Mathf.Infinity;

        foreach (var node in waypoints)
        {
            float distance = Vector3.Distance(pos, node.transform.position);


            if (dir == true)
            {
                float playerAngle = RCC_SceneManager.Instance.activePlayerVehicle.transform.eulerAngles.y;

                //Check player world transform forward with waypoint world forward axis
                if ((playerAngle > 180 && playerAngle < 360 && node.transform.eulerAngles.y == 270 && distance < minVal) ||
                    (playerAngle > 0 && playerAngle < 180 && node.transform.eulerAngles.y == 90 && distance < minVal) ||
                    (playerAngle > 90 && playerAngle < 270 && node.transform.eulerAngles.y == 180 && distance < minVal) ||
                    (((playerAngle > 270 && playerAngle <= 360) || (playerAngle >= 0 && playerAngle < 90)) && distance < minVal && node.transform.eulerAngles.y == 0)
                        )
                {

                    minVal = distance;
                    currentLinkNode = node;
                }
            }
            else
            {
                if (distance < minVal)
                {
                    minVal = distance;
                    currentLinkNode = node;
                }
            }
        }
        return currentLinkNode;
    }
    private void Start()
    {
        StartCoroutine(IFindPath(StartPosition.position, TargetPosition.position));
    }
    IEnumerator IEMissedGPS(int _destinationPathIndex)
    {
        yield return null;
        if (RCC_SceneManager.Instance.activePlayerVehicle.direction >= 0)
        {
            if (!previousDestinationPath.Contains(DestinationPath[_destinationPathIndex]))
                if (isMissedGPS == false)
                {
                    NotificationContentView.instance.CreateNotification(StringConstants.missedGps, 50, false, "");
                    isMissedGPS = true;
                    yield return new WaitForSecondsRealtime(1f);
                    previousDestinationPath = DestinationPath;
                    isMissedGPS = false;
                }
        }
    }

    IEnumerator IFindPath(Vector3 startPos, Vector3 TargetPos)
    {
        yield return new WaitUntil(()=> RCC_SceneManager.Instance.activePlayerVehicle != null);
        startNode = NodePositionFromWorldPoint(startPos, true); //Get StartPoint
        LinkNode targetNode = NodePositionFromWorldPoint(TargetPos, false); //Get Targeted Parking Point

        //check if navigation is on to search for the path
        yield return new WaitUntil(() => navigationDirection.isnavigationDetection);
        if (DestinationPath.Count > 0)//Check if player missed the drawn path - with first two waypoints
        {
            StartCoroutine(IEMissedGPS(0));
            if (DestinationPath.Count > 1)
            {
                StartCoroutine(IEMissedGPS(1));
            }
        }
        yield return new WaitForSecondsRealtime(0.2f);
        List<LinkNode> OpenList = new List<LinkNode>();
        HashSet<LinkNode> ClosedList = new HashSet<LinkNode>(); //Closed list to show the path

        OpenList.Add(startNode);

        while (OpenList.Count > 0)
        {
            LinkNode currentNode = OpenList[0]; //Create a node and set it to the first item
            for (int i = 1; i < OpenList.Count; i++)
            {
                //If the f cost of that object is less than or equal to the f cost of the current node,Set the current node to that GameObject
                if (OpenList[i].currentDir.FCost < currentNode.currentDir.FCost || OpenList[i].currentDir.FCost == currentNode.currentDir.FCost && OpenList[i].currentDir.hCost < currentNode.currentDir.hCost)
                {
                    currentNode = OpenList[i];
                }
            }
            OpenList.Remove(currentNode);//Remove that from the open list
            ClosedList.Add(currentNode);//And add it to the closed list

            if (currentNode == targetNode)
            {
                GetFinalPath(startNode, targetNode);
                if (navigationDirection.isGPS_ON)
                {
                    if (navigationDirection.isnavigationDetection)
                        DrawLine(6, Color.blue, false);
                }
                else
                {
                    DestinationPath.Clear();
                    DrawLine(6, Color.blue, true);
                    yield break;
                }
            }

            if (currentNode != targetNode)
            {
                foreach (LinkNode NeighborNode in GetNeighbourNodes(currentNode))
                {
                    if (ClosedList.Contains(NeighborNode))
                    {
                        continue;//Skip it
                    }
                    int MoveCost = currentNode.currentDir.gCost + GetManhattenDistance(currentNode, NeighborNode);

                    if (MoveCost < NeighborNode.currentDir.gCost || !OpenList.Contains(NeighborNode))
                    {
                        NeighborNode.currentDir.gCost = MoveCost;
                        NeighborNode.currentDir.hCost = GetManhattenDistance(NeighborNode, targetNode);
                        NeighborNode.prevNode = currentNode;

                        if (!OpenList.Contains(NeighborNode))
                        {
                            OpenList.Add(NeighborNode);
                        }
                    }
                }
            }
        }
        yield return new WaitForSecondsRealtime(0.5f);
        StartCoroutine(IFindPath(StartPosition.position, TargetPosition.position));
    }

    int GetManhattenDistance(LinkNode nodeA, LinkNode nodeB)
    {
        int ix = (int)Mathf.Abs(nodeA.transform.position.x - nodeB.transform.position.x);
        int iy = (int)Mathf.Abs(nodeA.transform.position.z - nodeB.transform.position.z);

        return ix + iy;//Return the sum
    }
    public List<LinkNode> GetNeighbourNodes(LinkNode neighborNode)
    {
        List<LinkNode> NeighborList = new List<LinkNode>();

        //Check the Left side of the current node.
        if (neighborNode.currentDir.Left != null)
            NeighborList.Add(neighborNode.currentDir.Left.gameObject.GetComponent<LinkNode>());

        //Check the right side of the current node.
        if (neighborNode.currentDir.Right != null)
            NeighborList.Add(neighborNode.currentDir.Right.gameObject.GetComponent<LinkNode>());


        //Check the Top side of the current node.
        if (neighborNode.currentDir.Top != null)
            NeighborList.Add(neighborNode.currentDir.Top.gameObject.GetComponent<LinkNode>());

        //Check the Bottom side of the current node.
        if (neighborNode.currentDir.Bottom != null)
            NeighborList.Add(neighborNode.currentDir.Bottom.gameObject.GetComponent<LinkNode>());


        return NeighborList;//Return the neighbors list.
    }

    void GetFinalPath(LinkNode startingNode, LinkNode endNode)
    {
        List<LinkNode> FinalPath = new List<LinkNode>();//List to hold the path sequentially 
        LinkNode CurrentNode = endNode;//Node to store the current node being checked

        while (CurrentNode != startingNode)
        {
            FinalPath.Add(CurrentNode);//Add that node to the final path
            CurrentNode = CurrentNode.prevNode;//Move onto its previous node
        }

        FinalPath.Reverse();//Reverse the path to get the correct order
        DestinationPath = FinalPath;//Set the final path

        if (previousDestinationPath.Count <= 0)
        {
            previousDestinationPath = FinalPath;
        }
    }

}
[System.Serializable]
public class NavigationWayPoints
{
    public Transform thisNode;
    public Transform prevNode;

    int gCost;
    int hCost;
    public int FCost { get { return gCost + hCost; } }

    public Transform Left;
    public Transform Right;
    public Transform Top;
}