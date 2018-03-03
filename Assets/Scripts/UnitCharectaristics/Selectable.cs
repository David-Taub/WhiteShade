using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

partial class Unit : MonoBehaviour
{

    public GameObject SelectionCircle;

    public void StartSelectionCircle()
    {
        SelectionCircle = Instantiate(SelectionCircle, transform.position, Quaternion.identity);
        SelectionCircle.transform.SetParent(transform);
    }

}
