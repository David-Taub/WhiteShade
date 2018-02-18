using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class AnimatedWalker : MonoBehaviour
{
    enum State
    {
        IDLE=0, RIGHT=1, UP=2, DOWN=3, LEFT=4
    };

    //TODO: this should be a FourWayAnimatedWalker that inherits AnimatedWalker with ChangeAnimation method
    public bool FlipRight;
    

    public void ChangeAnimation(bool isMoving, Vector3 nextStep)
    {
        transform.localScale = new Vector3(1,1,1);
        if (!isMoving)
            GetComponent<Animator>().SetInteger("State", (int)State.IDLE);
        else if (transform.position.x < nextStep.x)
        {
            GetComponent<Animator>().SetInteger("State", (int)State.RIGHT);
            if (FlipRight)
                transform.localScale = new Vector3(-1, 1, 1);
        }
        else if (transform.position.x > nextStep.x)
            GetComponent<Animator>().SetInteger("State", (int)State.LEFT);
        else if (transform.position.y < nextStep.y)
            GetComponent<Animator>().SetInteger("State", (int)State.UP);
        else if (transform.position.y > nextStep.y)
            GetComponent<Animator>().SetInteger("State", (int)State.DOWN);
    }

}
