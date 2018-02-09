using Pathfinding;
using System.Collections;
using System.Threading;
using System.Collections.Generic;
using UnityEngine;
using System;

public class PlayerControls : MonoBehaviour
{
    static object DestCalculation = new object();
    private float speed = 10.0f;
    private Vector3 stepTarget;
    private GridNode destination;
    static private HashSet<GridNode> occupiedDestinations = new HashSet<GridNode>();
    Queue<GraphNode> currentPath = new Queue<GraphNode>();
    Animator amin;
    private GridGraph graph;
    public const int MAX_NEAR_MOVE_RADIUS = 3;

    private GridNode currentNode 
    {
        get{ return (GridNode)graph.GetNearest(transform.position).node;}
    }

    void Start()
    {
        stepTarget = transform.position;
        amin = GetComponent<Animator>();
        graph = (GridGraph)AstarPath.active.data.gridGraph;
    }

    void UpdateState()
    {
        if (transform.position == stepTarget)
            amin.SetInteger("State", 0);
        if (transform.position.x < stepTarget.x)
        {
            amin.SetInteger("State", 1);
            Vector3 localScale = transform.localScale;
            localScale.x = -1;
            transform.localScale = localScale;
        }
        if (transform.position.x > stepTarget.x)
        {
            amin.SetInteger("State", 4);
            Vector3 localScale = transform.localScale;
            localScale.x = 1;
            transform.localScale = localScale;
        }
        if (transform.position.y < stepTarget.y)
            amin.SetInteger("State", 2);
        if (transform.position.y > stepTarget.y)
            amin.SetInteger("State", 3);
    }
    
    void OnPathCalculated(Path p)
    {
        if (p.error)
        {
            Debug.Log("Can't reach destination " + destination.position + " from " + transform.position);
            Debug.Log(p.errorLog);
            currentPath.Clear();
            //todo: round that position, might be midway
            return;
        }
        currentPath = new Queue<GraphNode>(p.path);
    }
    void Update()
    {
        if (Input.GetMouseButtonDown(1))
        {
            //right click
            Vector3 click = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            click.z = 0;
            GridNode wantedDestination = (GridNode)graph.GetNearest(click).node;
            //Debug.Log("clicked" + click + "dest" + destination.position + "current" + transform.position);
            TryReachDest(wantedDestination);
        }
        stepOnPath();
        UpdateState();
    }

    private Boolean TryReachDest(GridNode wantedDestination)
    {
        lock (DestCalculation)
        {
            occupiedDestinations.Remove(destination);
            currentPath.Clear();
            wantedDestination = GetNearestFreeDest(wantedDestination);
            if (wantedDestination == null)
            {
                Debug.Log("Can't reach dest " + wantedDestination);
                return false;
            }
            occupiedDestinations.Add(wantedDestination);
            destination = wantedDestination;
            QueuePathToDest();
            return true;
    }
    }

    private GridNode GetNearestFreeDest(GridNode node, HashSet<GridNode> visited = null, int depth = MAX_NEAR_MOVE_RADIUS)
    {
        //Debug.Log("Looking at " + node.position + "depth " + depth + node.Walkable);
        if (currentNode.Area != node.Area)
        {
            Debug.Log("Not the same connectivity component");
            return null;
        }
        if (node.Walkable && !occupiedDestinations.Contains(node))
        {
            return node;
        }
        var connections = new List<GraphNode>();
        node.GetConnections(connections.Add);
        Debug.Log("not free, trying neighbours..." + connections.Count);
        if (depth == 0 || connections.Count == 0)
            return null;
        visited = visited == null ? new HashSet<GridNode>() : visited;
        visited.Add(node);
        foreach (GraphNode connection in connections)
        {
            Debug.Log("inside recursive");
            if (!visited.Contains((GridNode)connection))
            {
                GridNode res = GetNearestFreeDest((GridNode)connection, visited, depth - 1);
                if (res != null)
                    return res;
            }

        }
        return null;
    }


    static Vector3 Int3ToVector3(Int3 vec)
    {
        return new Vector3(vec.x, vec.y, vec.z);
    }

    private void QueuePathToDest()
    {
        AstarPath.StartPath(ABPath.Construct(transform.position, Int3ToVector3(destination.position), OnPathCalculated));
    }

    private void stepOnPath()
    {
        if (transform.position == stepTarget)
        {
            if (currentPath.Count > 0)
            {
                GraphNode nextNode = currentPath.Dequeue();
                AstarPath.active.AddWorkItem(ctx => {
                    if (nextNode.Walkable)
                    {
                        //take the step
                        stepTarget = Int3ToVector3(nextNode.position);
                    }
                    else
                    {
                        Debug.Log("Blocked, retrying");
                        //blocked by unexpected object, retry
                        TryReachDest(destination);
                    }
                });
            }
        }
        else
        {
            transform.position = Vector3.MoveTowards(transform.position, stepTarget, Time.deltaTime * speed);
        }
    }
    
    //private void SetPositionWalkable(Boolean walkableState, Vector3 pos, IWorkItemContext ctx)
    //{
    //    //AstarPath.active.AddWorkItem(new AstarWorkItem(ctx =>
    //    //{
    //    GraphNode currentNode = graph.GetNearest(pos).node;
    //    currentNode.Walkable = walkableState;
    //    graph.GetNodes(node => graph.CalculateConnections((GridNodeBase)node));
    //    ctx.QueueFloodFill();
    //    //}));
    //}
    private Vector3 RoundVector3(Vector3 vec)
    {
        return new Vector3(Mathf.Round(vec.x), Mathf.Round(vec.y), Mathf.Round(vec.z));
    }
}
