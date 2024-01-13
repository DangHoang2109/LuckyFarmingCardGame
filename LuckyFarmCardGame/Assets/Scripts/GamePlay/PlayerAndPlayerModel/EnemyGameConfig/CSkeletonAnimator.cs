using Spine;
using Spine.Unity;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Base tiêu chuẩn chung, tuy nhiên có vài model ko theo quy tắc này như Minotaur
/// </summary>
public static class AnimationState
{
    public const string APPEAR_ANIM = "Appear";
    public const string IDLE_ANIM = "Idle";
    public const string ATTACK_ANIM = "AttackA";
    public const string HURT_ANIM = "HurtA";
    public const string HURT_CRIT_ANIM = "HurtCritical";
    public const string DIE_ANIM = "Die";

    public const string DISAPPEAR = "Disappear";
}
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


    [SerializeField] private string _idleKey;
    [SerializeField] private string _appearKey;
    [SerializeField] private string _attackKey;
    [SerializeField] private string _hurtKey;
    [SerializeField] private string _hurtCritKey;
    [SerializeField] private string _dieKey;
    [SerializeField] private string _disAppearKey;

    public string APPEAR_ANIM => string.IsNullOrEmpty(this._appearKey) ? AnimationState.APPEAR_ANIM : _appearKey;
    public string IDLE_ANIM => string.IsNullOrEmpty(this._idleKey) ? AnimationState.IDLE_ANIM : _idleKey;
    public string ATTACK_ANIM => string.IsNullOrEmpty(this._attackKey) ? AnimationState.ATTACK_ANIM : _attackKey;
    public string HURT_ANIM => string.IsNullOrEmpty(this._hurtKey) ? AnimationState.HURT_ANIM : _hurtKey;
    public string HURT_CRIT_ANIM => string.IsNullOrEmpty(this._hurtCritKey) ? AnimationState.HURT_CRIT_ANIM : _hurtCritKey;
    public string DIE_ANIM => string.IsNullOrEmpty(this._dieKey) ? AnimationState.DIE_ANIM : _dieKey;

    public string DISAPPEAR => string.IsNullOrEmpty(this._disAppearKey) ? AnimationState.DISAPPEAR : _disAppearKey;

    public void ShowAnimation(string state, bool loop = false)
    {
        this.CAnimation?.SetAnimation(trackIndex: 0, animationName: state, loop: loop);
    }
    public void ShowIdle()
    {
        ShowAnimation(state: IDLE_ANIM, loop: true);
    }
    public void ShowAppear()
    {
        ShowAnimation(state: APPEAR_ANIM);
        CAnimation?.AddAnimation(trackIndex: 0, animationName: IDLE_ANIM, loop: true, delay: 0);
    }
    public void ShowAttack()
    {
        ShowAnimation(state: ATTACK_ANIM);
        CAnimation?.AddAnimation(trackIndex: 0, animationName: IDLE_ANIM, loop: true, delay: 0);
    }
    public void ShowAttacked(bool isDead, Action cb)
    {
        ShowAnimation(state: HURT_ANIM);
        if(isDead) {
            ShowDead(cb: cb);
        }
        else
            CAnimation?.AddAnimation(trackIndex: 0, animationName: IDLE_ANIM, loop: true, delay: 0);
    }
    public void ShowAttackedCrit()
    {
        ShowAnimation(state: HURT_CRIT_ANIM);
    }
    public void ShowDead(Action cb)
    {
        CAnimation.Complete += OnSpineAnimationEnd;

        CAnimation.AddAnimation(0, DIE_ANIM, false, delay:0);
        void OnSpineAnimationEnd(TrackEntry trackEntry) //, Spine.Event e
        {
            //Spine.Animation dieAnimation = SkeletonGraphic.SkeletonData.FindAnimation(AnimationState.DIE_ANIM);
            if (trackEntry.Animation.Name.Equals(AnimationState.DIE_ANIM))
            {
                cb?.Invoke();
            }
        }
    }
    private string ConvertConstrantToDefine(string outsideInput)
    {
        if (outsideInput.Equals(AnimationState.HURT_ANIM))
            return this.HURT_ANIM;
        if (outsideInput.Equals(AnimationState.ATTACK_ANIM))
            return this.ATTACK_ANIM;
        if (outsideInput.Equals(AnimationState.IDLE_ANIM))
            return this.IDLE_ANIM;
        if (outsideInput.Equals(AnimationState.APPEAR_ANIM))
            return this.APPEAR_ANIM;
        if (outsideInput.Equals(AnimationState.DIE_ANIM))
            return this.DIE_ANIM;
        if (outsideInput.Equals(AnimationState.DISAPPEAR))
            return this.DISAPPEAR;
        if (outsideInput.Equals(AnimationState.HURT_CRIT_ANIM))
            return this.HURT_CRIT_ANIM;
        return IDLE_ANIM;
    }
    public void ShowAnimationWithCallback(string animKey, Action cb)
    {
        string animKeyInDefine = ConvertConstrantToDefine(animKey);
        //Cler Appearing or Idle anim
        CAnimation.ClearTrack(0);

        CAnimation.Complete -= OnSpineAnimationEnd;
        CAnimation.Complete += OnSpineAnimationEnd;
        CAnimation?.AddAnimation(trackIndex: 0, animationName: animKeyInDefine, loop: false, delay: 0);
        CAnimation?.AddAnimation(trackIndex: 0, animationName: IDLE_ANIM, loop: true, delay: 0);

        void OnSpineAnimationEnd(TrackEntry trackEntry) //, Spine.Event e
        {
            Debug.Log($"Anim -{trackEntry.Animation.Name} ended");
            //Spine.Animation dieAnimation = SkeletonGraphic.SkeletonData.FindAnimation(AnimationState.DIE_ANIM);
            if (trackEntry.Animation.Name.Equals(animKeyInDefine))
            {
                cb?.Invoke();
            }
            CAnimation.Complete -= OnSpineAnimationEnd;

        }
    }

}
