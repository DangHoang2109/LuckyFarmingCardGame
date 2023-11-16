using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;
using TMPro;
using System;

public class InGameBagCardProgressAnim : MonoBehaviour
{
    public BaseCardVisual _cardVisual;
    public CanvasGroup _canvasGroup;
    public Image _fillProgress;
    public TextMeshProUGUI _tmpFill;

    private float currentVal, _newValue, _maxGoal;
    private Tween progressTween, textTween;
    private Sequence moveTween;
    private float currentFill, _targetFill;
    [SerializeField] private float duration = 0.5f;
    [SerializeField] private float durationMove = 1f;
    private Transform _tfFrom, _tfTo;
    public bool _readyForNext = false;
    [ContextMenu("Test")]
    //private void Test()
    //{
    //    SetData(0, 2, 2);
    //    DoAnim();
    //}
    public void SetData(int current, int targetValue, int maxGoal, Transform _tfFrom, Transform _tfTo)
    {
        _readyForNext = false;

        this.currentVal = current;
        this._newValue = targetValue;
        this._maxGoal = maxGoal;

        this.currentFill = (float)currentVal / _maxGoal;
        this._targetFill = (float)_newValue / _maxGoal;
        this._tfTo = _tfTo; this._tfFrom = _tfFrom;

        _fillProgress.fillAmount = this.currentFill;
        _tmpFill.text = $"{(int)this.currentVal}/{(int)this._maxGoal}";

    }
    public void SetCardInfo(int cardID)
    {
        InGameCardConfig cardConfig = InGameCardConfigs.Instance.GetCardConfig(cardID);
        if (cardConfig != null)
        {
            this._cardVisual.DisplayArtwork(cardConfig._sprCardArtwork);
        }
    }
    public Sequence DoAnim()
    {
        DOTween.Kill(this.GetInstanceID());

        Sequence seq = DOTween.Sequence();
        seq.SetId(this.GetInstanceID());

        this.gameObject.SetActive(true);
        this._canvasGroup.alpha = 1;
        Transform tf = this.transform;

        tf.position = _tfFrom.position; //new Vector3(-320, -40);
        //
        seq.Join(tf.DOLocalMoveX(_tfTo.position.x, durationMove).SetEase(Ease.InBack).OnComplete(() =>
        {
            _readyForNext = true;
        }));

        seq.Append(tf.DOLocalMoveY(_tfTo.position.y, duration + 0.2f));

        seq.Join(AnimateTextValue(this._newValue));
        seq.Join(AnimateProgress(this._targetFill));

        seq.Append(this._canvasGroup.DOFade(0f, duration + 0.2f));

        return seq;
    }
    public Tween AnimateMove()
    {
        Debug.Log("Move");
        if (this.moveTween != null)
        {
            moveTween.Kill();
        }
        Transform tf = this.transform;
        //tf.transform.localPosition = new Vector3(-320, -40);

        moveTween = DOTween.Sequence();
        moveTween.Join(tf.DOLocalMoveX(0, durationMove).SetEase(Ease.InBack).OnComplete(() =>
        {
            _readyForNext = true;
        }));
        moveTween.Append(tf.DOLocalMoveY(58, duration + 0.2f));
        moveTween.Append(this._canvasGroup.DOFade(0f, duration + 0.2f));

        moveTween.OnComplete(() =>
        {
            Debug.Log("Complete Mode");

            moveTween = null;
        });

        return moveTween;
    }

    public Tween AnimateProgress(float _targetFill)
    {
        if (this.progressTween != null)
        {
            progressTween.Kill();
        }

        progressTween = DOTween.To(() => this.currentFill, x => this.currentFill = x, _targetFill, duration).SetEase(Ease.Linear).OnUpdate(() =>
        {
            _fillProgress.fillAmount = this.currentFill;
        }).OnComplete(() =>
        {
            progressTween = null;
            CheckFullBarEffect();
        });

        return progressTween;
    }

    public Tween AnimateTextValue(float _targetValue)
    {
        if (this.textTween != null)
        {
            textTween.Kill();
        }

        textTween = DOTween.To(() => this.currentVal, x => this.currentVal = x, _targetValue, duration).SetEase(Ease.Linear).OnUpdate(() =>
        {
            _tmpFill.text = $"{(int)this.currentVal}/{(int)this._maxGoal}";
        }).OnComplete(() =>
        {
            textTween = null;
        });

        return textTween;
    }

    private void CheckFullBarEffect()
    {
        //show the fx when the bar is full
    }
}
