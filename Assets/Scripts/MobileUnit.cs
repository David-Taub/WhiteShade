using UnityEngine;
using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using Pathfinding;

public class MobileUnit: MonoBehaviour
{
    private NavigationController _navigationController;
    private AnimatedWalker _animatedWalker;
    private const float Speed = 10.0f;

    public Vector3 NextStep
    {
        get { return (_path.Count == 0) ? Position: _path.First(); }
    }

    public Vector3 Destination
    {
        get { return (_path.Count == 0) ? Position: _path.Last(); }
    }
    public Vector3 Position
    {
        get { return transform.position; }
    }
    public bool IsMoving
    {
        get { return _path.Count > 0; }
    }

    private Queue<Vector3> _path;

    public void Start()
    {
        _path = new Queue<Vector3>();
        _navigationController = GameObject.Find("GameController").GetComponent<NavigationController>();
        _animatedWalker = GetComponent<AnimatedWalker>();
    }

    public void Update()
    {
        UpdatePosition();
    }

    private void UpdatePosition()
    {
        if (!IsMoving)
        {
            return;
        }
        if (IsMoving && transform.position == NextStep)
        {
            _path.Dequeue();
            _animatedWalker.ChangeAnimation(IsMoving, NextStep);
            if (!_navigationController.IsPositionWalkable(NextStep))
            {
                Debug.Log("Blocked, retrying");
                //blocked by unexpected object, retry
                Reach(Destination);
            }
            return;
        }
        transform.position = Vector3.MoveTowards(transform.position, NextStep, Time.deltaTime * Speed);
    }

    public void Reach(Vector3 wantedDestination)
    {
        _navigationController.TryReachDest(this, wantedDestination, newPath => { _path = (newPath.Count > 0) ? newPath : _path; });
    }
}