using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using UnityEngine;

public class ShooterUnit : MonoBehaviour
{

    private Vector3 _targetLastPos;
    public GameObject BulletPrefab;
    public float ReloadTime = 10.0f;
    private float _timeLeftToReload = 0.0f;
    public float Range = 10.0f;
    public bool CanReloadWhileWalking = false;
    public bool CanShootWhileWalking = false;
    public bool ShouldHoldFire = false;
    private InteractionableUnit Target;
    //private bool didTargetMove
    //{
    //    get
    //    {
    //        if (Target.transform.position == _targetLastPos)
    //            return true;
    //        _targetLastPos = Target.transform.position;
    //        return false;
    //    }
    //}

    private bool IsReloaded()
    {
        return _timeLeftToReload <= 0.0f;
    }

    private bool CanAimAt(Vector3 pos, InteractionableUnit unit)
    {
        return HasLineOfSight(pos, unit) && IsUnitInRage(pos, unit);
    }

    public void AquireTarget(InteractionableUnit target)
    {
        Target = target;
        _targetLastPos = target.transform.position;
    }

    private InteractionableUnit SearchAimableTarget()
    {
        var self = GetComponent<InteractionableUnit>();
        // TODO: replace this foreach with MapController.map search might be faster
        foreach (var potentialTarget in GetComponents<InteractionableUnit>())
        {
            if (potentialTarget.Group != self.Group && CanAimAt(transform.position, potentialTarget))
            {
                return potentialTarget;
            }
        }
        return null;
    }
    void Update()
    {
        MobileUnit mobileComponent = GetComponent<MobileUnit>();
        bool isMoving = (mobileComponent != null) && mobileComponent.IsMoving;
        if (!(isMoving && !CanReloadWhileWalking))
            AdvanceReload();
        if (Target == null)
        {
            if(IsReloaded() && !ShouldHoldFire)
            {
                var passingHostile  = SearchAimableTarget();
                if (passingHostile != null)
                    ShootAt(passingHostile);
            }
            return;
        }

        if (isMoving && CanAimAt(transform.position, Target))
        {
            //halt if moving and target in sight unexpectedly
            mobileComponent.Halt();
            return;
        }

        if (IsReloaded())
        {
            if (CanAimAt(transform.position, Target))
            {
                //shoot when reloaded and target in sight
                ShootAt(Target);
            }
            else if (mobileComponent != null &&(!mobileComponent.IsMoving || (mobileComponent.IsMoving && ShouldRepairRoute())))
            {
                //chase target if can't shoot
                GetInAimPosToTarget(mobileComponent);
            }
        }

    }

    private bool ShouldRepairRoute()
    {
        //TODO: check every few frames that movign toward the target
        return false;
    }

    private void GetInAimPosToTarget(MobileUnit mobileUnit)
    {
        mobileUnit.Reach(Target.transform.position, () =>
        {
            while (mobileUnit.Path.Count>1)
            {
                if (CanAimAt(mobileUnit.Path.Last(), Target))
                    mobileUnit.Path.RemoveLast();
            }
        });
    }

    void ShootAt(InteractionableUnit unit)
	{

	    Assert.NotNull(unit);
        Assert.IsTrue(IsReloaded());

	    GameObject bullet = Instantiate(BulletPrefab);
	    BulletControl bulletControl = bullet.GetComponent<BulletControl>();
	    bulletControl.Direction = new Ray(transform.position, Vector3.Normalize(unit.transform.position - transform.position));
	    _timeLeftToReload = ReloadTime;
    }

    private bool IsUnitInRage(Vector3 pos, InteractionableUnit unit)
    {
        return (unit.transform.position - pos).magnitude > Range;
    }

    private void AdvanceReload()
    {
        if (_timeLeftToReload > 0.0f)
            _timeLeftToReload = Math.Max(0.0f, _timeLeftToReload - Time.deltaTime);
    }
    private bool HasLineOfSight(Vector3 pos, InteractionableUnit unit)
    {
        Vector3 rayDirection = unit.transform.position - pos;
        RaycastHit hit;
        if (Physics.Raycast(pos, rayDirection, out hit))
        {
            return hit.transform == unit.transform;
        }
        return false;
    }
}
