using Spine.Unity;
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
    public Spine.AnimationState Animation => this.SkeletonGraphic?.AnimationState;

    public void ShowAnimation(string state, bool loop = false)
    {
        this.Animation?.SetAnimation(trackIndex: 0, animationName: state, loop: loop);
    }
    public void ShowIdle()
    {
        ShowAnimation(state: AnimationState.IDLE_ANIM, loop: true);
    }
    public void ShowAppear()
    {
        ShowAnimation(state: AnimationState.APPEAR_ANIM);
        Animation?.AddAnimation(trackIndex: 0, animationName: AnimationState.IDLE_ANIM, loop: true, delay: 0);
    }
    public void ShowAttack()
    {
        ShowAnimation(state: AnimationState.ATTACK_ANIM);
    }
    public void ShowAttacked()
    {
        ShowAnimation(state: AnimationState.HURT_ANIM);
    }
    public void ShowAttackedCrit()
    {
        ShowAnimation(state: AnimationState.HURT_CRIT_ANIM);
    }
    public void ShowDead()
    {
        ShowAnimation(state: AnimationState.DIE_ANIM);
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
