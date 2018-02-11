using Pathfinding;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class NavigationController : MonoBehaviour {

    private readonly HashSet<GridNode> _occupiedDestinations = new HashSet<GridNode>();
    private readonly object _destCalculation = new object();
    private UnitSelector _unitSelector;
    public const int MaxNearMoveRadius = 500;

    private static GridGraph Graph
    {
        get { return AstarPath.active.data.gridGraph; }
    }

    public void Start()
    {
        AstarPath.active.logPathResults = PathLog.OnlyErrors;
        _unitSelector = GameObject.Find("GameController").GetComponent<UnitSelector>();
        Debug.Log("Navigation Controller Started");
    }

    // Update is called once per frame
    void Update () {

        if (Input.GetMouseButtonDown(1))
        {
            //right click
            Vector3 click = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            click.z = 0.0f;
            foreach (var selectedUnit in _unitSelector.Selected)
            {
                MobileUnit mobileUnit = selectedUnit.GetComponentInParent<MobileUnit>();
                if (mobileUnit != null)
                    mobileUnit.Reach(click);
            }
        }
    }
    
    //bfs search for a new destination
    private GridNode GetNearestFreeDest(MobileUnit unit, Vector3 wantedDestination)
    {
        
        Queue<GridNode> toCheck = new Queue<GridNode>();
        GridNode currentUnitNode = Vector3ToNode(unit.Position);
        toCheck.Enqueue(Vector3ToNode(wantedDestination));
        HashSet<int> visited = new HashSet<int>();
        while (toCheck.Count > 0 && toCheck.Count < MaxNearMoveRadius)
        {
            GridNode checkedDestnation = toCheck.Dequeue();
            if (IsGoodDest(currentUnitNode, checkedDestnation))
            {
                return checkedDestnation;
            }
            if (!checkedDestnation.Walkable)
            {
                return null;
            }
            visited.Add(checkedDestnation.NodeIndex);
            //Debug.Log("Visited: " +visited.Count +  currentNode.position + currentNode.NodeIndex);
            checkedDestnation.GetConnections(connectedNode =>
            {
                if (!visited.Contains(connectedNode.NodeIndex) )
                {
                    visited.Add(connectedNode.NodeIndex);
                    toCheck.Enqueue((GridNode)connectedNode);
                }
            });
        }
        Debug.LogError("max reached?" + toCheck.Count);
        return null;
    }


    public Boolean IsPositionWalkable(Vector3 position)
    {
        return Vector3ToNode(position).Walkable;
    }


    public void TryReachDest(MobileUnit unit, Vector3 wantedDestination, Action<Queue<Vector3>> onPathCalculated)
    {
        lock (_destCalculation)
        {

            _occupiedDestinations.Remove(Vector3ToNode(unit.Destination));
            GridNode destination = GetNearestFreeDest(unit, wantedDestination);
            if (destination == null)
            {
                Debug.Log("Can't reach dest " + wantedDestination);
                onPathCalculated(new Queue<Vector3>());
                return;
            }
            _occupiedDestinations.Add(destination);
            var abPath = ABPath.Construct(unit.transform.position, Int3ToVector3(destination.position), (foundPath) =>
            {
                var convertedPath = new Queue<Vector3>();
                if (foundPath.error)
                {
                    Debug.LogError("Can't reach destination " + destination.position + " from " + transform.position + foundPath.errorLog);
                }
                else
                {
                    foundPath.path.ForEach(node => convertedPath.Enqueue(Int3ToVector3(node.position)));
                }
                onPathCalculated(convertedPath);
            });
            AstarPath.StartPath(abPath);
        }
    }
    static Vector3 Int3ToVector3(Int3 vec)
    {
        return new Vector3(vec.x, vec.y, vec.z);
    }
    
    private GridNode Vector3ToNode(Vector3 position)
    {
        return (GridNode) Graph.GetNode((int)Math.Round(position.x), (int)Math.Round(position.y));
    }

    private Boolean IsGoodDest(GridNode source, GridNode destination)
    {
        //connected, walkable and unoccupied
        return ((source.Area == destination.Area) && destination.Walkable && !_occupiedDestinations.Contains(destination));
    }


}
