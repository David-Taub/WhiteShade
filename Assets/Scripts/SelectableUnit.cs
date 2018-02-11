using UnityEngine;
using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;


public class SelectableUnit : MonoBehaviour
{
    public GameObject SelectionCircle;

    void Start()
    {
        //selectableObject.selectionCircle = Instantiate(selectionCirclePrefab, selectableObject.transform.position, Quaternion.identity);
        //selectableObject.selectionCircle.transform.SetParent(selectableObject.transform);
        SelectionCircle = Instantiate(SelectionCircle, transform.position, Quaternion.identity);
        SelectionCircle.transform.SetParent(transform);
    }
}