using NUnit.Framework;
using System;
using System.Linq;
using UnityEngine;

partial class Unit : MonoBehaviour
{
    public GameObject BulletPrefab;
    public float AimRange = 10.0f;
    public bool CanReloadWhileWalking = false;
    public bool CanShootWhileWalking = false;
    public bool ShouldHoldFire = false;
    public Progressable Reload;
    private Unit _target;

    void ShooterStart()
    {
        Reload = new Progressable(1, 1);
    }

    void ShootAt(Unit unit)
    {

        Assert.NotNull(unit);
        Assert.IsTrue(Reload.IsDone);

        GameObject bullet = Instantiate(BulletPrefab, transform.position, Quaternion.identity);
        //bullet.transform.SetParent(transform);
        BulletControl bulletControl = bullet.GetComponentInChildren<BulletControl>();
        bulletControl.Owner = gameObject;
        bulletControl.Direction = new Ray(transform.position, (unit.transform.position - transform.position).normalized);
        Reload.Reset();
    }

    private bool IsUnitInRange(Vector3 source, Unit targetUnit)
    {
        return (targetUnit.transform.position - source).magnitude <= AimRange;
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

    private bool CanAimAt(Vector3 source, Unit targetUnit)
    {
        return HasLineOfSight(source, targetUnit) && IsUnitInRange(source, targetUnit);
    }

    public void AcquireTarget(Unit target)
    {
        _target = target;
    }

}
