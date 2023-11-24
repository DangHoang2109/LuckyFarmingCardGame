using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
public class ShrineItem : MonoBehaviour
{
    public CSkeletonAnimatorBase _animator;
    public CanvasGroup _canvasGroup;
    public float duration = 0.25f;

    public void Show(System.Action cb = null)
    {
        StartCoroutine(ShowUp(cb));
    }
    private IEnumerator ShowUp(System.Action cb = null)
    {
        yield return new WaitForEndOfFrame();
        _canvasGroup.alpha = 0f;
        this.transform.localScale = new Vector3(0.5f, 0.5f);
        Sequence seq = DOTween.Sequence();
        seq.Join(this.transform.DOScale(1f, 0.25f));
        seq.Append(this._canvasGroup.DOFade(1f, duration));
        seq.AppendInterval(2f);
        seq.OnComplete(() =>
        {
            cb?.Invoke();
            this._animator?.ShowAnimation(state: AnimationState.IDLE_ANIM);
        });
    }

    public void Hide(System.Action cb = null)
    {
        this._animator.ShowAnimationWithCallback(state: AnimationState.DISAPPEAR, cb: cb);
    }
}
