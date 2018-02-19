using Pathfinding;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public partial class GameController : MonoBehaviour {

    private readonly HashSet<GridNode> _occupiedDestinations = new HashSet<GridNode>();
    private readonly object _destCalculation = new object();
    public const int MaxNearMoveRadius = 500;

    private static GridGraph Graph
    {
        get { return AstarPath.active.data.gridGraph; }
    }

    public void StartNavigation()
    {
        AstarPath.active.logPathResults = PathLog.OnlyErrors;
    }

    // Update is called once per frame
    void OrderGo(Vector3 pos)
    {
        foreach (var selectedUnit in Selected)
        {
            selectedUnit.Reach(pos);
        }
    }
    
    void OrderAttack(Unit target)
    {
        foreach (var selectedUnit in Selected)
        {
            selectedUnit.AcquireTarget(target);
        }
    }
    
    //bfs search for a new destination
    private GridNode GetNearestFreeDest(Unit unit, Vector3 wantedDestination)
    {
        
        Queue<GridNode> toCheck = new Queue<GridNode>();
        GridNode currentUnitNode = Vector3ToNode(unit.Position);
        toCheck.Enqueue(Vector3ToNode(wantedDestination));
        HashSet<int> visited = new HashSet<int>();
        while (toCheck.Count > 0 && toCheck.Count < MaxNearMoveRadius)
        {
            GridNode checkedDestnation = toCheck.Dequeue();
            if (checkedDestnation == null || !checkedDestnation.Walkable)
                continue;
            if (IsGoodDest(currentUnitNode, checkedDestnation))
            {
                return checkedDestnation;
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
        return null;
    }


    public bool IsPositionWalkable(Vector3 position)
    {
        return Vector3ToNode(position).Walkable;
    }

    public void ClearPos(Vector3 pos)
    {
        _occupiedDestinations.Remove(Vector3ToNode(pos));
    }

    public void TryReachDest(Unit unit, Vector3 wantedDestination, Action<LinkedList<Vector3>> onPathCalculated)
    {
        lock (_destCalculation)
        {

            _occupiedDestinations.Remove(Vector3ToNode(unit.Destination));
            GridNode destination = GetNearestFreeDest(unit, wantedDestination);
            if (destination == null)
            {
                Debug.Log("Can't reach dest " + wantedDestination);
                onPathCalculated(new LinkedList<Vector3>());
                return;
            }
            _occupiedDestinations.Add(destination);
            var abPath = ABPath.Construct(unit.transform.position, Int3ToVector3(destination.position), (foundPath) =>
            {
                var convertedPath = new LinkedList<Vector3>();
                if (foundPath.error)
                {
                    Debug.LogError("Can't reach destination " + destination.position + " from " + transform.position + foundPath.errorLog);
                }
                else
                {
                    foundPath.path.ForEach(node => convertedPath.AddLast(Int3ToVector3(node.position)));
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
