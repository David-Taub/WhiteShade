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
        get { return (Path.Count == 0) ? Position: Path.First(); }
    }

    public void Halt()
    {
        if (Path.Count <= 1) return;
        Vector3 nextStep = new Vector3(NextStep.x, NextStep.y, NextStep.z);
        Path.Clear();
        Path.AddLast(nextStep);
    }

    public Vector3 Destination
    {
        get { return (Path.Count == 0) ? Position: Path.Last(); }
    }
    public Vector3 Position
    {
        get { return transform.position; }
    }
    public bool IsMoving
    {
        get { return Path.Count > 0; }
    }

    public LinkedList<Vector3> Path;

    public void Start()
    {
        base.StartCoroutine()
        Path = new LinkedList<Vector3>();
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
            Vector3 oldPosition = NextStep;
            Path.RemoveFirst();
            if (IsMoving)
            {
                GetComponent<MapController>().UpdateObjectPos(gameObject, oldPosition);
            }
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

    public void Reach(Vector3 wantedDestination, Action action = null)
    {
        _navigationController.TryReachDest(this, wantedDestination, newPath =>
        {
            //doint that in case a bad click on unwalkable spot midway
            Path = (newPath.Count > 0) ? newPath : Path;
            if (action != null)
                action();
        });
    }
}