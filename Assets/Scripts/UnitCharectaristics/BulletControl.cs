using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking.NetworkSystem;

public class BulletControl : MonoBehaviour
{
    private float DamageImpact = 10.0f;
    private Ray Direction;
    private float Speed = 2.0f;
    private float DisappearRange = 20.0f;
    private GameObject Owner;
	
    
	// Update is called once per frame
	void Update ()
	{
	    transform.position += Direction.direction * Time.deltaTime * Speed;
	    if ((transform.position - Direction.origin).magnitude > DisappearRange)
	    {
            Destroy(gameObject);
	    }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (Owner == other.gameObject)
        {
            //bullet collider with own shooter
            return;
        }
        var interactionable = other.gameObject.GetComponentInChildren<Unit>();
        if (interactionable != null)
        {
            interactionable.ReceiveDamage(DamageImpact);
        }
        Destroy(gameObject);
    }

    internal void init(GameObject owner, Ray ray)
    {
        Owner = owner;
        Direction = ray;

    }
}
