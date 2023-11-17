using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VFXProjectile : VFXBaseObject
{
    public PoolPathType myPriorityPath;
    public bool isReadyForNext = false;


    public override void Init(Action<VFXBaseObject> _onCompleteAnime = null)
    {
        isReadyForNext = false;
        base.Init(_onCompleteAnime);
        AppendAnimation(this.SetReady);
    }
    private void SetReady(VFXBaseObject obj)
    {
        isReadyForNext =true;   
    }
    public override Sequence DoAnimation(Transform _desPos, float delay = 0, PoolPathType _pathType = PoolPathType.STRAIGHT_SCATTERED, float _scaleCurve = 50, float _moveDuration = 0.5F, List<Transform> _midPoint = null, AnimationCurve _curveX = null, AnimationCurve _curveY = null)
    {
        _pathType = this.myPriorityPath;

        if (_pathType == PoolPathType.STRAIGHT_FIRE)
        {
            return DoAnimationGoStraightFire(_desPos, _moveDuration : moveDuration , delay);
        }
        return base.DoAnimation(_desPos, delay, _pathType, _scaleCurve, _moveDuration, _midPoint, _curveX, _curveY);
    }
    public Sequence DoAnimationGoStraightFire(Transform _desPos, float _moveDuration = 0.05F, float delay = 0)
    {
        Sequence seq = DOTween.Sequence();

        seq.Join(this.transform.DOScale(1, 0.3f).SetDelay(delay).SetEase(Ease.OutBack));
        seq.Join(itemRect.DOMove(_desPos.position, _moveDuration).SetDelay(delay).SetEase(Ease.InCirc).OnStart(()=> { isReadyForNext = true; }));
        
        seq.OnComplete(() =>
        {
            isReadyForNext = true;
            onCompleteAnime?.Invoke(this);
        });

        return seq;
    }
}