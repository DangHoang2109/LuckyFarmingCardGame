using Spine;
using Spine.Unity;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CSkeletonAnimatorBase : MonoBehaviour
{
    protected SkeletonGraphic skeletonGraphic;
    protected SkeletonGraphic SkeletonGraphic
    {
        get
        {
            if (skeletonGraphic == null)
                skeletonGraphic = GetComponentInChildren<SkeletonGraphic>();
            return skeletonGraphic;
        }
    }


    public Spine.Skeleton Skin => this.SkeletonGraphic?.Skeleton;
    public Spine.AnimationState CAnimation => this.SkeletonGraphic?.AnimationState;

    public void ShowAnimation(string state, bool loop = false)
    {
        this.CAnimation?.SetAnimation(trackIndex: 0, animationName: state, loop: loop);
    }
    public void ShowAnimationWithCallback(string state, Action cb)
    {
        CAnimation.Complete += OnSpineAnimationEnd;
        CAnimation.AddAnimation(0, state, false, delay: 0);

        void OnSpineAnimationEnd(TrackEntry trackEntry) //, Spine.Event e
        {
            //Spine.Animation dieAnimation = SkeletonGraphic.SkeletonData.FindAnimation(AnimationState.DIE_ANIM);
            if (trackEntry.Animation.Name.Equals(state))
            {
                cb?.Invoke();
                Debug.Log("Complete Dead");
            }
        }
    }
}


