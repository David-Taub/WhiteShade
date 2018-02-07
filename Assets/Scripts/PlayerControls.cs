using Pathfinding;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class PlayerControls : MonoBehaviour {

    private float speed = 10.0f;
    private Vector3 stepTarget;
    private Vector3 destination;
    Queue<GraphNode> currentPath = new Queue<GraphNode>();
    Animator amin;
 
    void Start()
    {
        destination = transform.position;
        stepTarget = transform.position;
        amin = GetComponent<Animator>();
    }
    void UpdateState()
    {
        if (transform.position == stepTarget)
            amin.SetInteger("State", 0);
        if (transform.position.x < stepTarget.x)
        {
            amin.SetInteger("State", 1);
            Vector3 localScale = transform.localScale;
            localScale.x = -1;
            transform.localScale = localScale;
        }
        if (transform.position.x > stepTarget.x)
        {
            amin.SetInteger("State", 4);
            Vector3 localScale = transform.localScale;
            localScale.x = 1;
            transform.localScale = localScale;
        }
        if (transform.position.y < stepTarget.y)
            amin.SetInteger("State", 2);
        if (transform.position.y > stepTarget.y)
            amin.SetInteger("State", 3);
    }
    
    void OnPathComplete(Path p)
    {
        if (!p.error)
        {
            currentPath = new Queue<GraphNode>(p.path);
            Debug.Log("path found");
            Debug.Log(p.path[0].position);
        }
            
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(1))
        {
            //right click
            destination = RoundVector3(Camera.main.ScreenToWorldPoint(Input.mousePosition));
            destination.z = 0;
            AstarPath.StartPath(ABPath.Construct(transform.position, destination, OnPathComplete));
            Debug.Log("click at");
            Debug.Log(destination);
            Debug.Log("current");
            Debug.Log(transform.position);
        }
        stepOnPath();
        UpdateState();
    }
    private void stepOnPath()
    {
        if (transform.position == stepTarget)
        {
            if (currentPath.Count > 0)
            {
                GraphNode nextNode = currentPath.Dequeue();
                GridGraph graph = (GridGraph) AstarPath.active.data.graphs[0];
                stepTarget = new Vector3(nextNode.position.x, nextNode.position.y, nextNode.position.z);
                Debug.Log(stepTarget);
                Debug.Log(currentPath.Count);
            }
        }
        else
        {
            transform.position = Vector3.MoveTowards(transform.position, stepTarget, Time.deltaTime * speed);
        }
    }

    private Vector3 RoundVector3(Vector3 vec)
    {
        return new Vector3(Mathf.Round(vec.x), Mathf.Round(vec.y), Mathf.Round(vec.z));
    }
}
