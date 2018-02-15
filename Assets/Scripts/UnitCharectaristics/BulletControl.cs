using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking.NetworkSystem;

public class BulletControl : MonoBehaviour
{
    public float DamageImpact = 10.0f;
    public Ray Direction;
    public float Speed = 1.0f;
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

    void OnTriggerEnter(Collider other)
    {
        var interactionable = other.GetComponentInChildren<InteractionableUnit>();
        if (interactionable != null)
        {
            interactionable.ReceiveDamage(DamageImpact);
        }
        Destroy(gameObject);
    }
}
