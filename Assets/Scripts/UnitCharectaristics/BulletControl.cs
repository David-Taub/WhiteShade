using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking.NetworkSystem;

public class BulletControl : MonoBehaviour
{
    public float DamageImpact = 1.0f;
    public Ray Direction;
    public float Speed = 2.0f;
    public float DisappearRange = 20.0f;
	
    
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
        if (transform.parent == other.transform)
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
}
