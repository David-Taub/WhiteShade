using UnityEngine;
using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using Pathfinding;

public class MobileUnit: MonoBehaviour
{
    NavigationController navigationController;
    float speed = 10.0f;

    public Vector3 nextStep
    {
        get { return (path.Count == 0) ? position: path.First(); }
    }

    public Vector3 destination
    {
        get { return (path.Count == 0) ? position: path.Last(); }
    }
    public Vector3 position
    {
        get { return transform.position; }
    }
    private Boolean isMoving
    {
        get { return path.Count > 0; }
    }
    Queue<Vector3> path;

    public void Start()
    {
        path = new Queue<Vector3>();
        navigationController = GameObject.Find("GameController").GetComponent<NavigationController>();
    }

    public void Update()
    {
        UpdatePosition();
    }

    private void UpdatePosition()
    {
        if (!isMoving)
        {
            return;
        }
        if (isMoving && transform.position == nextStep)
        {
            path.Dequeue();
            if (!navigationController.IsPositionWalkable(nextStep))
            {
                Debug.Log("Blocked, retrying");
                //blocked by unexpected object, retry
                Reach(destination);
            }
            return;
        }
        transform.position = Vector3.MoveTowards(transform.position, nextStep, Time.deltaTime * speed);
    }

    public void Reach(Vector3 wantedDestination)
    {
        navigationController.TryReachDest(this, wantedDestination, newPath => { path = newPath; });
    }
}