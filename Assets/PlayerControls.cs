using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class PlayerControls : MonoBehaviour {

    private float speed = 5.0f;
    private float step = 1.0f;
    private Vector3 target;
    Animator amin;
 
    void Start()
    {
        target = transform.position;
        amin = GetComponent<Animator>();
    }
    void Update()
    {

        if (transform.position == target)
        {
            float x = 0, y = 0;
            x = Math.Sign(Input.GetAxis("Horizontal"));
            y = Math.Sign(Input.GetAxis("Vertical"));
            
            target += new Vector3(x, y, 0);
            if (transform.position == target)
                amin.SetInteger("State", 0);
           
        }
        if (transform.position.x < target.x)
        {
            amin.SetInteger("State", 1);
            Vector3 localScale = transform.localScale;
            localScale.x = -1;
            transform.localScale = localScale;
        }
        if (transform.position.x > target.x)
        {
            amin.SetInteger("State", 4);
            Vector3 localScale = transform.localScale;
            localScale.x = 1;
            transform.localScale = localScale;
        }
        if (transform.position.y < target.y)
            amin.SetInteger("State", 2);
        if (transform.position.y > target.y)
            amin.SetInteger("State", 3);
        transform.position = Vector3.MoveTowards(transform.position, target, Time.deltaTime * speed);

    }

}
