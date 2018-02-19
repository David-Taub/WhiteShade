using UnityEngine;
using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using Pathfinding;

public partial class Unit: MonoBehaviour
{
    private const float MinimumHealth = 0.0f;
    private AnimatedWalker _animatedWalker;
    private const float WalkSpeed = 10.0f;
    private float _timeLeftToReload = 0.0f;
    private Unit _target;
    private float _currentHealth;
    private GameController _gameController;

    public LinkedList<Vector3> Path;
    public GameObject SelectionCircle;
    public int Group;
    public float MaxHealth = 100;
    public GameObject BulletPrefab;
    public float ReloadTime = 1.0f;
    public float AimRange = 10.0f;
    
    public bool CanReloadWhileWalking = false;
    public bool CanShootWhileWalking = false;
    public bool ShouldHoldFire = false;
    
    public bool IsInPlayingGroup
    {
        get { return _gameController.PlayerGroup == Group;}
    }

    public Vector3 NextStep
    {
        get { return (Path.Count == 0) ? Position: Path.First(); }
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
        get { return Path != null && Path.Count > 0; }
    }

    public void StartSelectionCircle()
    {
        SelectionCircle = Instantiate(SelectionCircle, transform.position, Quaternion.identity);
        SelectionCircle.transform.SetParent(transform);
    }

    public void Start()
    {

        _gameController = FindObjectOfType<GameController>();
        _currentHealth = MaxHealth;
        Path = new LinkedList<Vector3>();
        if (IsInPlayingGroup)
        {
            StartPathLine();
            StartSelectionCircle();
        }
        _animatedWalker = GetComponent<AnimatedWalker>();
    }

    public void Update()
    {
        UpdatePosition();
        UpdatePathLines();
        UpdateShooter();

    }

    private void UpdateShooter()
    {
        if (!(IsMoving && !CanReloadWhileWalking))
            AdvanceReload();
        if (_target == null)
        {
            if (IsReloaded() && !ShouldHoldFire)
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

        if (IsReloaded())
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

    public void Die()
    {
        _gameController.Selected.Remove(this);
        _gameController.ClearPos(transform.position);
        Destroy(gameObject);
    }

    public void Halt()
    {
        if (Path.Count <= 1) return;
        
        Vector3 closestPos = new Vector3(Mathf.Round(NextStep.x), Mathf.Round(NextStep.y), Mathf.Round(NextStep.z));
        Path.Clear();
        Path.AddLast(closestPos);
    }

    private void FinishedStep()
    {
        if (IsMoving)
        {
            _gameController.GetComponent<MapController>().UpdateObjectPos(gameObject, transform.position);
        }
        _animatedWalker.UpdateAnimation(IsMoving, NextStep);
        if (!_gameController.IsPositionWalkable(NextStep))
        {
            Vector3 originalDest = new Vector3(Destination.x, Destination.y, Destination.z);
            Halt();
            Debug.Log("Blocked, retrying");
            //blocked by unexpected object, retry
            Reach(originalDest);
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

    public void Reach(Vector3 wantedDestination, Action action = null)
    {
        _gameController.TryReachDest(this, wantedDestination, newPath =>
        {
            //doint that in case a bad click on unwalkable spot midway
            Path = (newPath.Count > 0) ? newPath : Path;
            if (action != null)
                action();
        });
    }
    
    private bool IsReloaded()
    {
        return _timeLeftToReload <= 0.0f;
    }

    private bool CanAimAt(Vector3 source, Unit targetUnit)
    {
        return HasLineOfSight(source, targetUnit) && IsUnitInRange(source, targetUnit);
    }

    public void AcquireTarget(Unit target)
    {
        _target = target;
    }
    
    public void ReceiveDamage(float impact)
    {
        _currentHealth -= impact;
        if (_currentHealth < MinimumHealth)
        {
            Destroy(gameObject);
        }
    }

    
    private Unit SearchAimableTarget()
    {
        // TODO: replace this foreach with MapController.map search might be faster
        var units = FindObjectsOfType<Unit>();
        foreach (var potentialTarget in units)
        {
            if (potentialTarget.Group != Group && CanAimAt(transform.position, potentialTarget))
            {
                return potentialTarget;
            }
        }
        return null;
    }

    private bool ShouldRepairRoute()
    {
        //TODO: check every few frames that movign toward the target
        return false;
    }

    private void GetInAimPosToTarget(Unit target)
    {
        Reach(target.transform.position, () =>
        {
            Debug.Log("" + Path.Count + target.Position + Path.Last() + CanAimAt(Path.Last(), target));
            while (Path.Count > 1 && CanAimAt(Path.Last(), target))
            {
                Debug.Log("" + Path.Count + target.Position + Path.Last() + CanAimAt(Path.Last(), target));
                Path.RemoveLast();
                Debug.Log("" + Path.Count + target.Position + Path.Last() + CanAimAt(Path.Last(), target));
            }
        });
    }

    void ShootAt(Unit unit)
    {

        Assert.NotNull(unit);
        Assert.IsTrue(IsReloaded());

        GameObject bullet = Instantiate(BulletPrefab, transform.position, Quaternion.identity);
        //bullet.transform.SetParent(transform);
        BulletControl bulletControl = bullet.GetComponentInChildren<BulletControl>();
        bulletControl.Owner = gameObject;
        bulletControl.Direction = new Ray(transform.position, (unit.transform.position - transform.position).normalized);
        _timeLeftToReload = ReloadTime;
    }

    private bool IsUnitInRange(Vector3 source, Unit targetUnit )
    {
        return (targetUnit.transform.position - source).magnitude <= AimRange;
    }

    private void AdvanceReload()
    {
        if (_timeLeftToReload > 0.0f)
            _timeLeftToReload = Math.Max(0.0f, _timeLeftToReload - Time.deltaTime);
    }

    private bool HasLineOfSight(Vector3 source, Unit targetUnit)
    {
        if ((source - targetUnit.Position).magnitude <= 1)
            return true;
        Vector3 direction = (targetUnit.transform.position - source).normalized;

        RaycastHit2D[] hits = new RaycastHit2D[2];
        int numOfHits = Physics2D.RaycastNonAlloc(source, direction, hits, AimRange, ~(1 << LayerMask.NameToLayer("Bullets")));
        return numOfHits > 1 && hits[1].collider.gameObject == targetUnit.gameObject;
    }

}