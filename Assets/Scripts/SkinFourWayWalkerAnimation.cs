using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class SkinFourWayWalkerAnimation : MonoBehaviour
{
 
    //TODO: this should be a FourWayAnimatedWalker that inherits AnimatedWalker with ChangeAnimation method
    public Sprite[] Up;
    public Sprite[] Down;
    public Sprite[] Left;
    public Sprite[] Right;
    public Sprite[] Idle;
    public RuntimeAnimatorController Controller;

    // Update is called once per frame
    void Start()
    {
        Dictionary<string, Sprite[]> myDict = new Dictionary<string, Sprite[]>();
        myDict["Up"] = Up;
        myDict["Down"] = Down;
        myDict["Left"] = Left;
        myDict["Right"] = Right;
        myDict["Idle"] = Idle;
        foreach (var clip in Controller.animationClips)
	    {
            Debug.Log("clip" + clip.GetInstanceID());
            ReplaceClipFrames(clip, myDict[clip.name]);
	    }
	}

    private void ReplaceClipFrames(AnimationClip clip, Sprite[] replacements)
    {
        EditorCurveBinding binding = AnimationUtility.GetObjectReferenceCurveBindings(clip)[0];
        ObjectReferenceKeyframe[] keyFrames = AnimationUtility.GetObjectReferenceCurve(clip, binding);
        for (int i = 0; i < keyFrames.Length; i++)
        {
            keyFrames[i].value = replacements[i];
        }
        AnimationUtility.SetObjectReferenceCurve(clip, binding, keyFrames);
    }

 

}
