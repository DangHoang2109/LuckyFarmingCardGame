using Spine;
using Spine.Unity;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CSkeletonAnimator : MonoBehaviour
{
    protected SkeletonGraphic skeletonGraphic;
    protected SkeletonGraphic SkeletonGraphic
    {
        get
        {
            if(skeletonGraphic == null)
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
    public void ShowIdle()
    {
        ShowAnimation(state: AnimationState.IDLE_ANIM, loop: true);
    }
    public void ShowAppear()
    {
        ShowAnimation(state: AnimationState.APPEAR_ANIM);
        CAnimation?.AddAnimation(trackIndex: 0, animationName: AnimationState.IDLE_ANIM, loop: true, delay: 0);
    }
    public void ShowAttack()
    {
        ShowAnimation(state: AnimationState.ATTACK_ANIM);
        CAnimation?.AddAnimation(trackIndex: 0, animationName: AnimationState.IDLE_ANIM, loop: true, delay: 0);
    }
    public void ShowAttacked(bool isDead, Action cb)
    {
        ShowAnimation(state: AnimationState.HURT_ANIM);
        if(isDead) {
            ShowDead(cb: cb);
        }
        else
            CAnimation?.AddAnimation(trackIndex: 0, animationName: AnimationState.IDLE_ANIM, loop: true, delay: 0);
    }
    public void ShowAttackedCrit()
    {
        ShowAnimation(state: AnimationState.HURT_CRIT_ANIM);
    }
    public void ShowDead(Action cb)
    {
        CAnimation.Complete += OnSpineAnimationEnd;

        CAnimation.AddAnimation(0, AnimationState.DIE_ANIM, false, delay:0);
        void OnSpineAnimationEnd(TrackEntry trackEntry) //, Spine.Event e
        {
            //Spine.Animation dieAnimation = SkeletonGraphic.SkeletonData.FindAnimation(AnimationState.DIE_ANIM);
            if (trackEntry.Animation.Name.Equals(AnimationState.DIE_ANIM))
            {
                cb?.Invoke();
            }
        }
    }
    public void ShowAnimationWithCallback(string animKey, Action cb)
    {
        //Cler Appearing or Idle anim
        CAnimation.ClearTrack(0);

        CAnimation.Complete += OnSpineAnimationEnd;
        CAnimation?.AddAnimation(trackIndex: 0, animationName: animKey, loop: false, delay: 0);
        CAnimation?.AddAnimation(trackIndex: 0, animationName: AnimationState.IDLE_ANIM, loop: true, delay: 0);

        void OnSpineAnimationEnd(TrackEntry trackEntry) //, Spine.Event e
        {
            //Spine.Animation dieAnimation = SkeletonGraphic.SkeletonData.FindAnimation(AnimationState.DIE_ANIM);
            if (trackEntry.Animation.Name.Equals(animKey))
            {
                cb?.Invoke();
            }
        }
    }

}
