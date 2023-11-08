﻿using Spine;
using Spine.Unity;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CSkeletonAnimator : MonoBehaviour
{
    //[ContextMenu("test")]
    //public void Test()
    //{
    //    skeletonGraphic.Skeleton.SetSkin("Crystal"); //Đổi skin
    //    skeletonGraphic.AnimationState.SetAnimation(trackIndex: 0, "AttackA", loop: true);
    //}
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
    }
    public void ShowAttacked(bool isDead, Action cb)
    {
        ShowAnimation(state: AnimationState.HURT_ANIM);
        if(isDead) {
            ShowDead(cb: cb);
        }
    }
    public void ShowAttackedCrit()
    {
        ShowAnimation(state: AnimationState.HURT_CRIT_ANIM);
    }
    public void ShowDead(Action cb)
    {
        CAnimation.Start += OnSpineAnimationStart;
        CAnimation.Complete += OnSpineAnimationEnd;

        CAnimation.AddAnimation(0, AnimationState.DIE_ANIM, false, delay:0);
        void OnSpineAnimationEnd(TrackEntry trackEntry) //, Spine.Event e
        {
            Debug.Log("Join Callback " + trackEntry.Animation.Name);

            //Spine.Animation dieAnimation = SkeletonGraphic.SkeletonData.FindAnimation(AnimationState.DIE_ANIM);
            if (trackEntry.Animation.Name.Equals(AnimationState.DIE_ANIM))
            {
                cb?.Invoke();
                Debug.Log("Complete Dead");
            }

        }
        void OnSpineAnimationStart(TrackEntry trackEntry)
        {
            Debug.Log("Start Dead");
        }
    }
}
public static class AnimationState
{
    public const string APPEAR_ANIM = "Appear";
    public const string IDLE_ANIM = "Idle";
    public const string ATTACK_ANIM = "AttackA";
    public const string HURT_ANIM = "HurtA";
    public const string HURT_CRIT_ANIM = "HurtCritical";
    public const string DIE_ANIM = "Die";
}
