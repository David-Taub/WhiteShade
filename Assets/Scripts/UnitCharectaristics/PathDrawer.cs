using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEditor;

public partial class Unit
{
    public void StartPathLine()
    {
        LineRenderer pathLines = GetComponent<LineRenderer>();
        pathLines.material = new Material(Shader.Find("Particles/Additive (Soft)"));
        pathLines.startColor = new Color(1, 1 , 1, 0.5f);
        pathLines.endColor = new Color(1, 1, 1, 0.5f);

    }

    public void UpdatePathLines()
    {
        if (Path == null)
            return;
        LineRenderer pathLines = GetComponent<LineRenderer>();
        if (pathLines == null)
            return;
        //pathLines.material.SetColor("_TintColor", );
        if (pathLines.positionCount != Path.Count)
        {
            pathLines.positionCount = Path.Count;
            var pathArray = Path.ToArray();
            for (int i = 0; i < pathArray.Length; i++)
            {
                pathArray[i] -= Vector3.forward;
            }
            pathLines.SetPositions(pathArray);
        }

        if (Path.Count > 0)
            pathLines.SetPosition(0, transform.position);
        
    }
}