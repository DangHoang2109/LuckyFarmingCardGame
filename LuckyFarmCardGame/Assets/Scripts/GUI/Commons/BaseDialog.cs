using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.Events;

[RequireComponent(typeof(CanvasGroup))]
public class BaseDialog : MonoBehaviour
{
    [Tooltip("should be at root object")]
    public CanvasGroup canvasGroup;
    protected object data;
    public Transform panel;
    protected float transitionTime = 0.2f;

    #region event

    public System.Action OnShowing;
    public System.Action OnShowed;
    public System.Action OnClosing;
    public System.Action OnClosed;
    #endregion

    protected UnityAction callbackShow;

#if UNITY_EDITOR
    protected virtual void OnValidate()
    {
        this.canvasGroup = this.GetComponent<CanvasGroup>();
    }
#endif

    public virtual void OnShow(object data = null, UnityAction callback = null)
    {
        this.gameObject.SetActive(true);
        this.data = data;
        this.callbackShow = callback;
        this.AnimationShow();
        
        this.OnShowing?.Invoke();
    }
    protected virtual void AnimationShow()
    {
        this.panel.localScale = Vector3.zero;
        if (this.canvasGroup != null)
        {
            this.canvasGroup.alpha = 0;

        }
        Sequence seq = DOTween.Sequence();
        seq.Join(this.panel.DOScale(1f, this.transitionTime).SetEase(Ease.OutBack).OnComplete(this.OnCompleteShow));
        if (this.canvasGroup != null)
        {
            seq.Join(this.canvasGroup.DOFade(1, this.transitionTime));
        }
        SoundManager.Instance.Play("snd_panel");
    }
    protected virtual void OnCompleteShow()
    {
        if (this.callbackShow != null)
        {
            var bk = this.callbackShow;
            this.callbackShow = null;
            bk.Invoke();
        }
        
        this.OnShowed?.Invoke();
    }
    public virtual void OnHide()
    {
        this.AnimationHide();
    }
    protected virtual void AnimationHide()
    {
        Sequence seq = DOTween.Sequence();
        seq.Join(this.panel.DOScale(0.0f, this.transitionTime).SetEase(Ease.Linear).OnComplete(this.OnCompleteHide));
        if (this.canvasGroup != null)
        {
            seq.Join(this.canvasGroup.DOFade(0, this.transitionTime));
        }
        //SoundManager.Instance.Play("snd_PanelIn");
    }
    protected virtual void OnCompleteHide()
    {
        this.OnClosed?.Invoke();
        this.OnClosed = null;
        this.gameObject.SetActive(false);
    }
    public virtual void OnCloseDialog()
    {
        this.OnClosing?.Invoke();
        this.OnClosing = null;
        GameManager.Instance.OnHideDialog(this);
    }
    public virtual void ClickCloseDialog()
    {
        this.OnCloseDialog();
        SoundManager.Instance.PlayButtonClick();
    }

}
