using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractionableUnit : MonoBehaviour
{
    public int Group;
    public float CurrentHealth;
    public float MaxHealth = 100;
    private const float MinimumHealth = 0.0f;
    void Start()
    {
        CurrentHealth = MaxHealth;
    }

    public void ReceiveDamage(float impact)
    {
        CurrentHealth -= impact;
        if (CurrentHealth < MinimumHealth)
        {
            Destroy(gameObject);
        }
    }
}
