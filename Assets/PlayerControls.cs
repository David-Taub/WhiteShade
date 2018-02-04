using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class PlayerControls : MonoBehaviour {

    private float speed = 5.0f;
    private float step = 2.0f;
    private Vector3 target;

 
    void Start()
    {
        target = transform.position;
    }
    void Update()
    {

        if (transform.position == target)
        {
            float x = Math.Sign(Input.GetAxis("Horizontal"));
            float y = Math.Sign(Input.GetAxis("Vertical"));
            
            target += new Vector3(x, y, 0);
        }
        transform.position = Vector3.MoveTowards(transform.position, target, Time.deltaTime * speed);

    }

}
