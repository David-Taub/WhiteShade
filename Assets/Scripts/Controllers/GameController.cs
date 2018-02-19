using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class GameController : MonoBehaviour
{
    public int PlayerGroup = 0;

    void Start()
    {
        StartNavigation();
    }
    void Update()
    {
        updateInput();
        UpdateUnitSelection();
    }

    void updateInput()
    {
        if (Input.GetMouseButtonDown(0))
        {
            SelectDown();
        }
        if (Input.GetMouseButtonUp(0))
        {
            SelectUp();
        }
        if (Input.GetMouseButtonUp(1))
        {
            Vector3 click = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            click.z = 0.0f;

            Unit hostile = hasHostile(click);
            if (hostile != null)
            {
                OrderAttack(hostile);
            }
            else
            {
                OrderGo(click);
            }
        }
    }
    public Unit hasHostile(Vector3 pos)
    {
        var units = FindObjectsOfType<Unit>();
        for (int i = 0; i < units.Length; i++)
        {
            if ((units[i].transform.position - pos).magnitude < 1.0f)
                return units[i];
        }
        return null;
    }
}