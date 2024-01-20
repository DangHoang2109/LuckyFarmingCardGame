using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RuleSummaryDialog : BaseDialog
{
    private Transform _tfButtonShowRuleSummary;
    private System.Action cbComplete;
    public override void ClickCloseDialog()
    {
        _tfButtonShowRuleSummary = InGameUIManager.Instance._tfButtonSummaryRule;
        base.ClickCloseDialog();
    }
    public override void OnHide()
    {
        base.OnHide();
    }
    protected override void AnimationShow()
    {
        panel.transform.localPosition = Vector3.zero;
        base.AnimationShow();
    }
    protected override void AnimationHide()
    {
        if(_tfButtonShowRuleSummary != null)
        {
            Sequence seq = DOTween.Sequence();
            seq.Join(this.panel.DOMove(_tfButtonShowRuleSummary.position, this.transitionTime).SetEase(Ease.Linear));
            seq.Join(this.canvasGroup.DOFade(0, this.transitionTime).SetEase(Ease.Linear));
            seq.Join(this.panel.DOScale(0, this.transitionTime).SetEase(Ease.Linear));
            seq.OnComplete(this.OnCompleteHide);
        }
        else
            base.AnimationHide();
    }
    protected override void OnCompleteHide()
    {
        this.cbComplete?.Invoke();
        base.OnCompleteHide();
        cbComplete = null;
    }
    public static RuleSummaryDialog ShowDialog(System.Action cbComplete = null)
    {
        RuleSummaryDialog d = GameManager.Instance.OnShowDialog<RuleSummaryDialog>("Dialogs/RuleSummaryDialog");
        d.cbComplete -= cbComplete; 
        d.cbComplete += cbComplete;
        return d;
    }
}
