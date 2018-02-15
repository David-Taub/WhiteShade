﻿using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class UnitSelector : MonoBehaviour
{
    public GameObject SelectionCirclePrefab;
    public List<SelectableUnit> Selected;
    private bool _isSelecting;
    private Vector3 _selectionStart;

    // Use this for initialization
    void Start()
    {
        Debug.Log("Unit Selector Started");
        Selected = new List<SelectableUnit>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            UnselectAllUnits();
            _isSelecting = true;
            _selectionStart = Input.mousePosition;
        }
        // If we let go of the left mouse button, end selection
        if (Input.GetMouseButtonUp(0))
        {
            Selected = SelectUnits(SelectionBox);
            _isSelecting = false;
            //Debug.Log("Selected" + Selected.Count);
        }
        if (_isSelecting)
        {
            SelectBoxHover(SelectionBox);
        }
    }

    private Rect SelectionBox
    {
        get
        {
            var point1 = Camera.main.ScreenToWorldPoint(_selectionStart);
            var point2 = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            var topLeft = Vector3.Min(point1, point2);
            var bottomRight = Vector3.Max(point1, point2);
            // Create Rect
            return Rect.MinMaxRect(topLeft.x, topLeft.y, bottomRight.x, bottomRight.y);
        }   
    }

    private List<SelectableUnit> SelectUnits(Rect selectionBox)
    {
        var selectables = FindObjectsOfType<SelectableUnit>();
        return selectables.Where(x => selectionBox.Contains(x.transform.position)).ToList();
    }

    private void SelectBoxHover(Rect selectionBox)
    {
        foreach (var selectableObject in FindObjectsOfType<SelectableUnit>())
        {
            if (selectionBox.Contains(selectableObject.transform.position))
            {
                //in box
                if (selectableObject.SelectionCircle != null)
                {
                    selectableObject.SelectionCircle.SetActive(true);
                    //selectableObject.selectionCircle = Instantiate(selectionCirclePrefab, selectableObject.transform.position, Quaternion.identity);
                    //selectableObject.selectionCircle.transform.SetParent(selectableObject.transform);
                }
            }
            else
            {
                //out of box
                if (selectableObject.SelectionCircle != null)
                {
                    selectableObject.SelectionCircle.SetActive(false);
                    //Destroy(selectableObject.selectionCircle.gameObject);
                    //selectableObject.selectionCircle = null;
                }
            }
        }
    }

    private void UnselectAllUnits()
    {
        Selected.ForEach((x) => x.SelectionCircle.SetActive(false));
        //Debug.Log("Selected clear" + selected.Count);
        Selected.Clear();
    }
    private void OnGUI()
    {
        if (_isSelecting)
        {
            var rect = Utils.GetScreenRect(_selectionStart, Input.mousePosition);
            Utils.DrawScreenRect(rect, new Color(0.8f, 0.8f, 0.95f, 0.25f));
            Utils.DrawScreenRectBorder(rect, 2, new Color(0.8f, 0.8f, 0.95f));
        }
    }
}