using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialFaceHighLightNoTap : TutorialFaceImage
{
    public override bool IsRaycastLocationValid(Vector2 sp, Camera eventCamera)
    {
        return true;
    }
    public override void Play(RectTransform target)
    {

        base.Play(target);
    }
}
