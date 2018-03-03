using System;
using UnityEngine;

public class Progressable
{
    public float Value;
    public float ProgressRate;

    public Progressable(float value, float progressRate)
    {
        Value = value;
        ProgressRate = progressRate;
    }

    public bool IsDone
    {
        get { return Value >= 1; }
    }

    public void Reset()
    {
        Value = 0;
    }

    public void Advance()
    {
        Value = Math.Min(1.0f, Value + Time.deltaTime);
    }
}
