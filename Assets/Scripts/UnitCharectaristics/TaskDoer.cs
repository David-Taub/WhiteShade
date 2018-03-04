using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

partial class Unit
{
    public Progressable CurrentTask = null;
    public Slider ProgressSlider;

    void UpdateTask()
    {
        if (Reload != null && !Reload.IsDone && ((IsMoving && CanReloadWhileWalking) || !IsMoving))
            CurrentTask = Reload;
        if (CurrentTask == null)
            return;
        CurrentTask.Advance();
        if (CurrentTask.IsDone)
        {
            CurrentTask = null;
        }
        if (ProgressSlider != null)
            ProgressSlider.value = CurrentTask==null ? 0 : CurrentTask.Value;
    }
}
