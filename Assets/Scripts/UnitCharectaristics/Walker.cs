﻿using System;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

partial class Unit : MonoBehaviour
{
    private const float WalkSpeed = 10.0f;
    private AnimatedWalker _animatedWalker;
    public GameObject DestinationPlaceholder;
    public LinkedList<Vector3> Path;

    public Vector3 NextStep
    {
        get { return (Path.Count == 0) ? Position : Path.First(); }
    }

    public Vector3 Destination
    {
        get { return (Path.Count == 0) ? Position : Path.Last(); }
    }

    public bool IsMoving
    {
        get { return Path != null && Path.Count > 0; }
    }

    public void Halt()
    {
        if (Path.Count <= 1) return;

        Vector3 closestPos = new Vector3(Mathf.Round(NextStep.x), Mathf.Round(NextStep.y), Mathf.Round(NextStep.z));
        Path.Clear();
        Path.AddLast(closestPos);
        _animatedWalker.UpdateAnimation(IsMoving, NextStep);
    }

    private void FinishedStep()
    {
        if (!_gameController.IsPositionWalkable(NextStep))
        {
            Vector3 originalDest = new Vector3(Destination.x, Destination.y, Destination.z);
            Halt();
            Debug.Log("Blocked, retrying");
            //blocked by unexpected object, retry
            Reach(originalDest);
            return;
        }
        if (IsMoving)
        {
            _gameController.UpdateObjectPos(gameObject, transform.position, NextStep);
        }
        else
        {
            _gameController.RemoveObjectPos(DestinationPlaceholder, transform.position);
        }
        _animatedWalker.UpdateAnimation(IsMoving, NextStep);
    }

    private bool ShouldRepairRoute()
    {
        //TODO: check every few frames that movign toward the target
        return false;
    }


    public void Reach(Vector3 wantedDestination, Action action = null)
    {
        _gameController.TryReachDest(this, wantedDestination, newPath =>
        {
            //doint that in case a bad click on unwalkable spot midway
            _gameController.RemoveObjectPos(DestinationPlaceholder, Destination);
            Path = (newPath.Count > 0) ? newPath : Path;
            _gameController.AddObjectPos(DestinationPlaceholder, Destination);
            if (action != null)
                action();
        });
    }

    private void UpdateShooter()
    {
        if (_target == null)
        {
            if (Reload.IsDone && !ShouldHoldFire)
            {
                var passingHostile = SearchAimableTarget();
                if (passingHostile != null)
                    ShootAt(passingHostile);
            }
            return;
        }

        if (IsMoving && CanAimAt(transform.position, _target))
        {
            //halt if moving and target in sight unexpectedly
            Halt();
            return;
        }

        if (Reload.IsDone)
        {
            if (CanAimAt(transform.position, _target))
            {
                //shoot when reloaded and target in sight
                ShootAt(_target);
            }
            else if (!IsMoving || (IsMoving && ShouldRepairRoute()))
            {
                //chase target if can't shoot
                GetInAimPosToTarget(_target);
            }
        }
    }

    private void UpdatePosition()
    {
        if (!IsMoving)
        {
            return;
        }
        if (IsMoving && transform.position == NextStep)
        {
            Path.RemoveFirst();
            FinishedStep();
            return;
        }
        transform.position = Vector3.MoveTowards(transform.position, NextStep, Time.deltaTime * WalkSpeed);
    }

}
