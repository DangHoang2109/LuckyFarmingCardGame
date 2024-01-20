using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialHighlightNoTap : TutorialHighLight
{
    private bool IsCompeteTutorial
    {
        get
        {
            return TutorialManager.Instance.IsCompleteTutorial();
        }
    }

    private RectTransform _rect;
    public RectTransform rect
    {
        get
        {
            if (this._rect == null)
            {
                this._rect = this.GetComponent<RectTransform>();
            }

            return this._rect;
        }
    }
#if UNITY_EDITOR
    private void OnValidate()
    {
        this._rect = this.GetComponent<RectTransform>();
    }
#endif
    private void OnEnable()
    {
        TutorialManager.Instance.RegisterHighlight(this.step, this);
        TutorialManager.tutorialCallback += this.OnTutorialCallback;
        //Cosina.DataManagement.SaveManager.Instance.RegisterCallbackOnDataLoaded(() =>
        //{
        //    this.CheckCompleteTutorial();
        //});
    }

    private void OnDisable()
    {
        TutorialManager.tutorialCallback -= this.OnTutorialCallback;
    }
    private void OnTutorialCallback(int step)
    {

    }
}