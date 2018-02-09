using Pathfinding;
using System.Collections;
using System.Threading;
using System.Collections.Generic;
using UnityEngine;
using System;

public class PlayerControls : MonoBehaviour
{
    void UpdateState()
    {
        Animator anim = GetComponent<Animator>();
        Vector3 nextStep = GetComponent<MobileUnit>().nextStep;
        if (transform.position == nextStep)
            anim.SetInteger("State", 0);
        if (transform.position.x < nextStep.x)
        {
            anim.SetInteger("State", 1);
            Vector3 localScale = transform.localScale;
            localScale.x = -1;
            transform.localScale = localScale;
        }
        if (transform.position.x > nextStep.x)
        {
            anim.SetInteger("State", 4);
            Vector3 localScale = transform.localScale;
            localScale.x = 1;
            transform.localScale = localScale;
        }
        if (transform.position.y < nextStep.y)
            anim.SetInteger("State", 2);
        if (transform.position.y > nextStep.y)
            anim.SetInteger("State", 3);
    }

    void Update()
    {
        UpdateState();
    }
}